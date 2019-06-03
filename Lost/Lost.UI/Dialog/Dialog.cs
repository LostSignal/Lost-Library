//-----------------------------------------------------------------------
// <copyright file="Dialog.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using System.Linq;
    using Lost.Analytics;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(HDCanvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(DialogSetupHelper))]
    public sealed class Dialog : MonoBehaviour
    {
        public enum ShowType
        {
            HideThenShow,
            ShowImmediate,
        }

        public enum StoreType
        {
            None,
            Soft,
            Premium,
        }

        private static readonly int HideHash = Animator.StringToHash("Hide");
        private static readonly int ShowHash = Animator.StringToHash("Show");

        #pragma warning disable 0649
        [Header("General")]
        [SerializeField] private bool showOnAwake = false;
        [SerializeField] private bool isOverlayCamera = false;

        [Tooltip("If true, then Hide() and Show() won't work while dialog is transitioning.")]
        [SerializeField] private bool dontChangeStateWhileTransitioning = true;

        [Header("Input Blocker")]
        [Tooltip("This dialog should swallow up all input so you can't click behind it.")]
        [SerializeField] private bool blockInput = true;

        [Tooltip("If you tap anywhere outside the dialog, then it will dismiss it.")]
        [SerializeField] private bool tapOutsideToDismiss;

        [Header("Back Button")]
        [SerializeField] private bool registerForBackButton = true;
        [SerializeField] private bool hideOnBackButtonPressed = true;

        [Header("Analytics")]
        [SerializeField] private bool sendAnalyticEvent = true;
        [SerializeField] private StoreType storeType;

        // Events
        [SerializeField] private UnityEvent onShow;
        [SerializeField] private UnityEvent onHide;
        [SerializeField] private UnityEvent onBackButtonPressed;

        // Set By Code
        [SerializeField, HideInInspector] private DialogStateMachine dialogStateMachine;
        [SerializeField, HideInInspector] private RectTransform contentRectTransform;
        [SerializeField, HideInInspector] private GraphicRaycaster graphicRaycaster;
        [SerializeField, HideInInspector] private InputBlocker blocker;
        [SerializeField, HideInInspector] private Animator animator;
        [SerializeField, HideInInspector] private HDCanvas hdCanvas;
        [SerializeField, HideInInspector] private Canvas canvas;
        #pragma warning restore 0649

        private bool isHibernateMonitorRunning = false;
        private bool isShowing;

        public UnityEvent OnShow
        {
            get { return this.onShow; }
        }

        public UnityEvent OnHide
        {
            get { return this.onHide; }
        }

        public UnityEvent OnBackButtonPressed
        {
            get { return this.onBackButtonPressed; }
        }

        public Canvas Canvas
        {
            get { return this.canvas; }
        }

        public bool BlockInput
        {
            get { return this.blockInput; }
        }

        public bool TapOutsideToDismiss
        {
            get { return this.tapOutsideToDismiss; }
        }

        public Animator Animator
        {
            get { return this.animator; }
        }

        public bool HideOnBackButtonPressed
        {
            get { return this.hideOnBackButtonPressed; }
        }

        public bool RegisterForBackButton
        {
            get { return this.registerForBackButton; }
        }

        public bool IsShowing
        {
            get { return this.isShowing; }
        }

        public bool IsShown
        {
            get { return this.isShowing && this.dialogStateMachine.IsDoneShowing; }
        }

        public bool IsHidden
        {
            get { return this.isShowing == false && (this.dialogStateMachine.IsDoneHiding || this.dialogStateMachine.IsInitialized); }
        }

        public bool IsTransitioning
        {
            get { return this.IsShown == false && this.IsHidden == false; }
        }

        public bool IsOverlayCamera
        {
            get
            {
                return this.isOverlayCamera;
            }

            set
            {
                this.isOverlayCamera = value;
                this.UpdateRenderMode();
            }
        }

        public IEnumerator ShowAndWait()
        {
            this.Show();

            while (this.IsHidden == false)
            {
                yield return null;
            }
        }

        public void Show()
        {
            // makes sure that we point to a valid camera
            if (!this.canvas.worldCamera)
            {
                this.canvas.worldCamera = Camera.main;
            }

            // early out if we're not suppose to change state while transitioning
            if (this.dontChangeStateWhileTransitioning && this.IsTransitioning)
            {
                return;
            }

            if (this.isShowing == false)
            {
                if (this.animator == null)
                {
                    Debug.LogErrorFormat(this, "Dialog {0} has a null animator.  Did you forget to call base.Awake()?", this.gameObject.name);
                }

                this.isShowing = true;
                this.animator.SetBool(ShowHash, true);
                this.SetActive(true);
                this.onShow?.Invoke();

                DialogManager.Instance.AddDialog(this);

                if (this.sendAnalyticEvent)
                {
                    if (this.storeType == StoreType.Soft)
                    {
                        AnalyticsEvent.StoreOpened(Lost.Analytics.StoreType.Soft);
                    }
                    else if (this.storeType == StoreType.Premium)
                    {
                        AnalyticsEvent.StoreOpened(Lost.Analytics.StoreType.Premium);
                    }

                    AnalyticsEvent.ScreenVisit(this.gameObject.name);
                }
            }
        }

        public void Hide()
        {
            this.HideThenShow(null);
        }

        public void HideThenShow(Dialog dialog)
        {
            // early out if we're not suppose to change state while transitioning
            if (this.dontChangeStateWhileTransitioning && this.IsTransitioning)
            {
                return;
            }

            if (this.isShowing)
            {
                this.StartHibernateMonitorCoroutine(dialog);
                this.isShowing = false;
                this.animator.SetBool(ShowHash, false);
                this.onHide?.Invoke();

                DialogManager.Instance.RemoveDialog(this);
            }
            else if (dialog != null)
            {
                dialog.Show();
            }
        }

        public void BackButtonPressed()
        {
            if (this.HideOnBackButtonPressed)
            {
                this.Hide();
            }

            this.onBackButtonPressed?.Invoke();
        }

        public void Toggle()
        {
            if (this.isShowing)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }

        public void SetCamera(Camera camera)
        {
            this.canvas.worldCamera = camera;
        }

        public void SetSortingLayerAndOrder(string layerName, int order = 0)
        {
            this.canvas.sortingLayerName = layerName;
            this.canvas.sortingOrder = order;
        }

        public void InitializeFields()
        {
            this.AssertGetComponent<HDCanvas>(ref this.hdCanvas);
            this.AssertGetComponent<Canvas>(ref this.canvas);
            this.AssertGetComponent<Animator>(ref this.animator);
            this.AssertGetComponent<GraphicRaycaster>(ref this.graphicRaycaster);

            if (this.dialogStateMachine == null)
            {
                this.dialogStateMachine = this.animator.GetBehaviour<DialogStateMachine>();
            }

            if (this.contentRectTransform == null || this.contentRectTransform.gameObject.name != "Content")
            {
                Transform contentTransform = this.gameObject.transform.Find("Content");
                this.contentRectTransform = contentTransform != null ? contentTransform.GetComponent<RectTransform>() : null;
            }

            if ((this.blocker == null || this.blocker.gameObject.name != "Blocker") && (this.blockInput || this.tapOutsideToDismiss))
            {
                GameObject blockerObject = this.gameObject.GetChild("Blocker");
                this.blocker = blockerObject != null ? blockerObject.GetComponent<InputBlocker>() : null;
            }

            this.UpdateRenderMode();
        }

        private void UpdateRenderMode()
        {
            this.canvas.renderMode = this.isOverlayCamera ? RenderMode.ScreenSpaceOverlay : RenderMode.ScreenSpaceCamera;
        }

        private void Awake()
        {
            this.InitializeFields();

            // making the Animator is setup correctly
            Debug.AssertFormat(this.dialogStateMachine != null, this, "Dialog {0} doesn't have a DialogStateMachine behavior attached to it's Animator.", this.gameObject.name);
            Debug.AssertFormat(this.animator.HasState(0, ShowHash), this, "Dialog {0}'s Animator doesn't have a \"Show\" state.", this.gameObject.name);
            Debug.AssertFormat(this.animator.HasState(0, HideHash), this, "Dialog {0}'s Animator doesn't have a \"Hide\" state.", this.gameObject.name);

            var showingParameter = this.animator.parameters.FirstOrDefault(x => x.nameHash == ShowHash);
            Debug.AssertFormat(showingParameter != null, this, "Dialog {0}'s Animator doesn't have a \"Show\" Bool parameter.", this.gameObject.name);

            // making sure the content is setup correctly
            Debug.AssertFormat(this.contentRectTransform != null, this, "Dialog {0} dosen't contain a Content object with a RectTransform.", this.gameObject.name);

            // making sure the input blocker is setup correctly
            if (this.blockInput || this.tapOutsideToDismiss)
            {
                Debug.AssertFormat(this.blocker != null, this, "Dialog {0} doesn't have a Blocker child with an InputBlocker component.", this.gameObject.name);
            }

            if (this.showOnAwake)
            {
                this.Show();
            }
            else
            {
                this.SetActive(false);
            }
        }

        private void Start()
        {
            // this needs to happen after all Awakes are called
            if (this.tapOutsideToDismiss)
            {
                this.blocker.OnClick.AddListener(this.Hide);
                this.contentRectTransform.gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        private void Reset()
        {
            this.InitializeFields();
        }

        private void StartHibernateMonitorCoroutine(Dialog dialog)
        {
            if (this.isHibernateMonitorRunning == false)
            {
                this.isHibernateMonitorRunning = true;
                this.StartCoroutine(this.HibernateMonitorCoroutine(dialog));
            }
        }

        private IEnumerator HibernateMonitorCoroutine(Dialog dialog)
        {
            while (this.IsHidden == false)
            {
                yield return null;
            }

            if (dialog != null)
            {
                dialog.Show();
                yield return null;
            }

            if (this != dialog)
            {
                this.SetActive(false);
            }

            this.isHibernateMonitorRunning = false;
        }

        private void SetActive(bool active)
        {
            this.enabled = active;
            this.hdCanvas.enabled = active;
            this.animator.enabled = active;
            this.graphicRaycaster.enabled = active;
            this.contentRectTransform.gameObject.SetActive(active);

            if (this.blocker != null)
            {
                this.blocker.gameObject.SetActive(active);
            }
        }
    }
}

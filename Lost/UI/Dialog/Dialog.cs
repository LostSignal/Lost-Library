//-----------------------------------------------------------------------
// <copyright file="Dialog.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(HDCanvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class Dialog : MonoBehaviour
    {
        private static readonly int HideHash = Animator.StringToHash("Hide");
        private static readonly int ShowHash = Animator.StringToHash("Show");
    
        public enum ShowType
        {
            HideThenShow,
            ShowImmediate,
        }

        #pragma warning disable 0649
        [Tooltip("This dialog should swallow up all input so you can't click behind it.")]
        [SerializeField] private bool blockInput = true;

        [Tooltip("If you tap anywhere outside the dialog, then it will dismiss it.")]
        [SerializeField] private bool tapOutsideToDismiss;

        [Tooltip("If true, then Hide() and Show() won't work while dialog is transitioning.")]
        [SerializeField] private bool dontChangeStateWhileTransitioning;

        [Header("Back Button")]
        [SerializeField] private bool registerForBackButton = true;
        [SerializeField] private bool hideOnBackButtonPressed = true;
        [SerializeField] private Dialog showDialogOnBackButtonPressed;
        [SerializeField] private ShowType showType;
        #pragma warning restore 0649
        
        private bool isHibernateMonitorRunning = false;
        private DialogStateMachine dialogStateMachine;
        private RectTransform contentRectTransform;
        private GraphicRaycaster graphicRaycaster;
        private InputBlocker blocker;
        private Animator animator;
        private HDCanvas hdCanvas;
        private Canvas canvas;
        private bool isShowing;
        
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

        public bool IsHidding
        {
            get { return this.isShowing == false; }
        }

        public bool IsHidden
        {
            get { return this.isShowing == false && this.dialogStateMachine.IsDoneHiding; }
        }

        public bool IsTransitioning
        {
            get { return this.IsShown == false && this.IsHidden == false;  }
        }
        
        public IEnumerator ShowAndWait()
        {
            this.Show();
            
            while (this.IsHidden == false)
            {
                yield return null;
            }
        }

        public virtual void Show()
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
                this.OnShow();

                DialogManager.Instance.AddDialog(this);
            }
        }

        public virtual void Hide()
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
                this.OnHide();

                DialogManager.Instance.RemoveDialog(this);
            }
        }

        public virtual void OnBackButtonPressed()
        {
            if (this.HideOnBackButtonPressed)
            {
                if (this.showType == ShowType.HideThenShow)
                {
                    this.HideThenShow(this.showDialogOnBackButtonPressed);
                }
                else if (this.showType == ShowType.ShowImmediate)
                {
                    if (this.showDialogOnBackButtonPressed != null)
                    {
                        this.showDialogOnBackButtonPressed.Show();
                    }
                }
                else
                {
                    Debug.LogErrorFormat("Dialog.OnBackButtonPressed found unknown ShowType {0}", this.showType);
                }
            }
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

        public void SetSortingLayerAndOrder(string layerName, int order = 0)
        {
            this.canvas.sortingLayerName = layerName;
            this.canvas.sortingOrder = order;
        }

        protected virtual void Awake()
        {
            this.hdCanvas = this.GetComponent<HDCanvas>();
            this.canvas = this.GetComponent<Canvas>();
            this.animator = this.GetComponent<Animator>();
            this.dialogStateMachine = this.animator.GetBehaviour<DialogStateMachine>();
            this.graphicRaycaster = this.GetComponent<GraphicRaycaster>();

            // cleaning up the editor UI a little bit
            this.hdCanvas.hideFlags = HideFlags.HideInInspector;
            this.graphicRaycaster.hideFlags = HideFlags.HideInInspector;

            // making sure the animator is set up properly
            Debug.AssertFormat(this.dialogStateMachine != null, this, "Dialog {0} doesn't have a DialogStateMachine behavior attached.", this.gameObject.name);
            Debug.AssertFormat(this.animator.HasState(0, ShowHash), this, "Dialog {0} doesn't have a \"Show\" state.", this.gameObject.name);
            Debug.AssertFormat(this.animator.HasState(0, HideHash), this, "Dialog {0} doesn't have a \"Hide\" state.", this.gameObject.name);

            var showingParameter = this.animator.parameters.FirstOrDefault(x => x.nameHash == ShowHash);
            Debug.Assert(showingParameter != null, "Dialog doesn't have a \"Show\" Bool parameter.", this);

            // making sure the content object exists
            Transform contentTransform = this.gameObject.transform.Find("Content");
            Debug.Assert(contentTransform != null, "Dialog doesn't contain a Content child object.", this);

            this.contentRectTransform = contentTransform.GetComponent<RectTransform>();
            Debug.Assert(this.contentRectTransform != null, "Dialog's Content child object doesn't contain a RectTransform component.", this);

            // NOTE [bgish]:  Determining if we should create the input blocker.  Also, it's a
            //                coroutine because unity doesn't like creating object in the Awake
            if (this.blockInput || this.tapOutsideToDismiss)
            {
                CoroutineRunner.Start(this.CreateInputBlocker());
            }

            // default everything to inactive and wait for someone to call Show()
            this.SetActive(false);
        }

        private IEnumerator CreateInputBlocker()
        {
            yield return new WaitForEndOfFrame();

            // NOTE [bgish]: Must pass in InputBlocker, or else it will have a Standard Transform and the RequireComponent attributes wont work when adding the InputBlocker later
            GameObject blockerObject = this.gameObject.GetOrCreateChild("Blocker", typeof(InputBlocker));
            this.blocker = blockerObject.GetComponent<InputBlocker>();
            this.blocker.gameObject.transform.SetAsFirstSibling();

            if (this.tapOutsideToDismiss)
            {
                this.blocker.OnClick.AddListener(this.Hide);
                this.contentRectTransform.gameObject.AddComponent<GraphicRaycaster>();
            }
            
            this.blocker.gameObject.SetActive(this.enabled);
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
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

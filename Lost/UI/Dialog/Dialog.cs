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
        private static readonly int HiddenHash = Animator.StringToHash("Hidden");
        private static readonly int ShownHash = Animator.StringToHash("Shown");
        private static readonly int ShowHash = Animator.StringToHash("Show");
        
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
            get { return this.isShowing && this.dialogStateMachine.IsInShownState; }
        }

        public bool IsHidding
        {
            get { return this.isShowing == false; }
        }

        public bool IsHidden
        {
            get { return this.isShowing == false && this.dialogStateMachine.IsInHiddenState; }
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
            // early out if we're not suppose to change state while transitioning
            if (this.dontChangeStateWhileTransitioning && this.IsTransitioning)
            {
                return;
            }

            if (this.isShowing == false)
            {
                this.isShowing = true;
                this.animator.SetBool(ShowHash, true);
                this.SetActive(true);
                this.OnShow();

                DialogManager.Instance.AddDialog(this);
            }
        }

        public virtual void Hide()
        {
            // early out if we're not suppose to change state while transitioning
            if (this.dontChangeStateWhileTransitioning && this.IsTransitioning)
            {
                return;
            }

            if (this.isShowing)
            {
                this.StartHibernateMonitorCoroutine();
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
                this.Hide();
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
            Debug.AssertFormat(this.dialogStateMachine != null, this, "Dialog {0} doesn't have a DialogStateMachine behaviour attached.", this.gameObject.name);
            Debug.AssertFormat(this.animator.HasState(0, ShownHash), this, "Dialog {0} doesn't have a \"Shown\" state.", this.gameObject.name);
            Debug.AssertFormat(this.animator.HasState(0, HiddenHash), this, "Dialog {0} doesn't have a \"Hidden\" state.", this.gameObject.name);

            var showingParameter = this.animator.parameters.FirstOrDefault(x => x.nameHash == ShowHash);
            Debug.Assert(showingParameter != null, "Dialog doesn't have a \"Show\" Bool parameter.", this);
            
            // making sure the content object exists
            Transform contentTransform = this.gameObject.transform.FindChild("Content");
            Debug.Assert(contentTransform != null, "Dialog doesn't contain a Content child object.", this);

            this.contentRectTransform = contentTransform.GetComponent<RectTransform>();
            Debug.Assert(this.contentRectTransform != null, "Dialog's Content child object doesn't contain a RectTransform component.", this);

            // making sure the blocker object exists
            if (this.blockInput || this.tapOutsideToDismiss)
            {
                GameObject blockerObject = this.gameObject.GetOrCreateChild("Blocker", typeof(InputBlocker));
                this.blocker = blockerObject.GetComponent<InputBlocker>();
                this.blocker.gameObject.transform.SetAsFirstSibling();

                if (this.tapOutsideToDismiss)
                {
                    this.blocker.OnClick.AddListener(this.Hide);
                    this.contentRectTransform.gameObject.AddComponent<GraphicRaycaster>();
                }
            }

            // default everything to inactive and wait for someone to call Show()
            this.SetActive(false);
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        private void StartHibernateMonitorCoroutine()
        {
            if (this.isHibernateMonitorRunning == false)
            {
                this.isHibernateMonitorRunning = true;
                this.StartCoroutine(this.HibernateMonitorCoroutine());
            }
        }

        private IEnumerator HibernateMonitorCoroutine()
        {
            while (this.IsHidden == false)
            {
                yield return null;
            }

            this.SetActive(false);
            this.isHibernateMonitorRunning = false;
        }

        private void SetActive(bool active)
        {
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

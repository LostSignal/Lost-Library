//-----------------------------------------------------------------------
// <copyright file="Dialog.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(HDCanvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class Dialog : MonoBehaviour
    {
        private static readonly int HideHash = Animator.StringToHash("Hide");
        private static readonly int ShowHash = Animator.StringToHash("Show");

        #pragma warning disable 0649
        [Tooltip("This dialog should swallow up all input so you can't click behind it.")]
        [SerializeField] private bool blockInput = true;

        [Tooltip("If you tap anywhere outside the dialog, then it will dismiss it.")]
        [SerializeField] private bool tapOutsideToDismiss;
        #pragma warning restore 0649
        
        private bool isHibernateMonitorRunning = false;
        private DialogStateMachine dialogStateMachine;
        private RectTransform contentRectTransform;
        private GraphicRaycaster graphicRaycaster;
        private InputBlocker blocker;
        private Animator animator;
        private HDCanvas hdCanvas;
        
        public bool IsShowing
        {
            get { return this.dialogStateMachine.IsShowing; }
        }

        public bool IsShown
        {
            get { return this.dialogStateMachine.IsShown; }
        }

        public bool IsHidden
        {
            get { return this.dialogStateMachine.IsHidden; }
        }

        public IEnumerator ShowAndWait()
        {
            this.Show();

            // waiting for it to start showing
            while (this.IsShowing == false)
            {
                yield return null;
            }

            // while we are showing... keep waiting
            while (this.IsHidden == false)
            {
                yield return null;
            }
        }

        public virtual void Show()
        {
            if (this.dialogStateMachine.IsShowing == false)
            {
                // making sure everything's turned on
                this.SetActive(true);
                
                // setting the show trigger
                this.animator.SetTrigger(ShowHash);

                this.OnShow();
            }
        }

        public virtual void Hide()
        {
            if (this.dialogStateMachine.IsHidding == false)
            {
                this.StartHibernateMonitorCoroutine();
                
                // setting the hide trigger
                this.animator.SetTrigger(HideHash);

                this.OnHide();
            }
        }

        public void Toggle()
        {
            if (this.dialogStateMachine.IsShowing)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }

        protected virtual void Awake()
        {            
            this.hdCanvas = this.GetComponent<HDCanvas>();
            this.animator = this.GetComponent<Animator>();
            this.dialogStateMachine = this.animator.GetBehaviour<DialogStateMachine>();
            this.graphicRaycaster = this.GetComponent<GraphicRaycaster>();

            // cleaning up the editor UI a little bit
            this.hdCanvas.hideFlags = HideFlags.HideInInspector;
            this.graphicRaycaster.hideFlags = HideFlags.HideInInspector;
            
            // TODO [bgish]: would be nice if we could also test if Hide and Show Triggers exist on this.animator as well
            // making sure the animator is set up properly
            Debug.Assert(this.dialogStateMachine != null, "Dialog doesn't have a DialogStateMachine behaviour attached.", this);
            Debug.Assert(this.animator.HasState(0, ShowHash), "Dialog doesn't have a Show state.", this);
            Debug.Assert(this.animator.HasState(0, HideHash), "Dialog doesn't have a Hide state.", this);

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
            while (this.dialogStateMachine.IsHidden == false)
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

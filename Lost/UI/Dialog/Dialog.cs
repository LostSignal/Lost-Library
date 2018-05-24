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

    #if USE_TEXTMESH_PRO
    using Text = TMPro.TextMeshProUGUI;
    #else
    using Text = UnityEngine.UI.Text;
    #endif

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(HDCanvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(DialogSetupHelper))]
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
        [SerializeField] private bool showOnAwake = false;

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

        public virtual void SetCamera(Camera camera)
        {
            this.canvas.worldCamera = camera;
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
                GameObject blockerObject = this.gameObject.GetChild("Blocker");
                Debug.AssertFormat(blockerObject != null, "Dialog {0} needs a Blocker, but no object of that name exists.", this.name);

                this.blocker = blockerObject.GetComponent<InputBlocker>();
                Debug.AssertFormat(this.blocker != null, "Dialog {0} has a Blocker object, but no InputBlocker component.", this.name);
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

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected Image DebugCreateImage(GameObject parent, string objectName, Color color, Vector3 localPosition)
        {
            var itemDescriptionText = new GameObject(objectName, typeof(Image));
            itemDescriptionText.transform.SetParent(parent.transform);
            itemDescriptionText.transform.Reset();
            itemDescriptionText.transform.localPosition = localPosition;

            var imageComponent = itemDescriptionText.GetComponent<Image>();
            imageComponent.color = color;

            return imageComponent;
        }

        protected Text DebugCreateText(GameObject parent, string objectName, string text, Vector3 localPosition)
        {
            var itemDescriptionText = new GameObject(objectName, typeof(RectTransform), typeof(Text));
            itemDescriptionText.transform.SetParent(parent.transform);
            itemDescriptionText.transform.Reset();
            itemDescriptionText.transform.localPosition = localPosition;

            var textComponent = itemDescriptionText.GetComponent<Text>();
            textComponent.text = text;
            textComponent.color = Color.black;

            return textComponent;
        }

        protected Button DebugCreateButton(GameObject parent, string objectName, string textObjectName, string text, Vector3 localPosition)
        {
            var button = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
            button.transform.SetParent(parent.transform);
            button.transform.Reset();
            button.transform.localPosition = localPosition;
            button.GetComponent<Image>().color = Color.grey;
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 50);

            var cancelButtonText = new GameObject(textObjectName, typeof(RectTransform), typeof(Text));
            cancelButtonText.transform.SetParent(button.transform);
            cancelButtonText.transform.Reset();
            cancelButtonText.GetComponent<Text>().text = text;
            cancelButtonText.GetComponent<Text>().color = Color.black;

            return button.GetComponent<Button>();
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

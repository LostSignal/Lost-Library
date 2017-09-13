//--------------------------------------------------------------------s---
// <copyright file="ConnectToServer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    
    #if USE_TEXTMESH_PRO
    using Text = TMPro.TextMeshProUGUI;
    #else
    using Text = UnityEngine.UI.Text;
    #endif

    public class ConnectToServer : SingletonDialogResource<ConnectToServer>
    {
        #pragma warning disable 0649
        [Header("Connect To Server")]
        [SerializeField] private Text body;
        #pragma warning restore 0649

        private Camera cameraCache;

        public void SetupDefault()
        {
            var content = this.transform.Find("Content").gameObject;
            content.GetComponent<RectTransform>().FitToParent();

            this.body = this.DebugCreateText(content, "BodyText", "Body", Vector3.zero);
        }

        public void Show(string bodyText)
        {
            Debug.Assert(string.IsNullOrEmpty(bodyText) == false, "ConnectToServer.Show was given invalid text", this);
            this.body.text = bodyText;
            this.Show();
        }

        public void UpdateBodyText(string bodyText)
        {
            Debug.Assert(string.IsNullOrEmpty(bodyText) == false, "ConnectToServer.UpdateBodyText was given invalid text", this);
            this.body.text = bodyText;
        }

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(this.body != null, "ConnectToServer didn't specify body text", this);
        }

        private void Update()
        {
            if (!this.cameraCache)
            {
                this.cameraCache = Camera.main;
                this.Canvas.worldCamera = this.cameraCache;
            }
        }
    }
}

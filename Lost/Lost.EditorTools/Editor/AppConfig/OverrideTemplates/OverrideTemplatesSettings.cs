//-----------------------------------------------------------------------
// <copyright file="OverrideTemplatesSettings.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(80)]
    public class OverrideTemplatesSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private TextAsset monoBehaviour;
        [SerializeField] private TextAsset playableAsset;
        [SerializeField] private TextAsset playableBehaviour;
        [SerializeField] private TextAsset stateMachineBehaviour;
        [SerializeField] private TextAsset subStateMachineBehaviour;
        [SerializeField] private TextAsset editorTestScript;

        [SerializeField, HideInInspector]
        private bool hasBeenInitialized;
        #pragma warning restore 0649

        public override string DisplayName => "Override Template Files";
        public override bool IsInline => false;

        public TextAsset MonoBehavoiur
        {
            get { return this.monoBehaviour; }
        }

        public TextAsset PlayableAsset
        {
            get { return this.playableAsset; }
        }

        public TextAsset PlayableBehaviour
        {
            get { return this.playableBehaviour; }
        }

        public TextAsset StateMachineBehaviour
        {
            get { return this.stateMachineBehaviour; }
        }

        public TextAsset SubStateMachineBehaviour
        {
            get { return this.subStateMachineBehaviour; }
        }

        public TextAsset EditorTestScript
        {
            get { return this.editorTestScript; }
        }

        private void OnEnable()
        {
            if (this.hasBeenInitialized == false)
            {
                this.monoBehaviour = EditorUtil.GetAssetByGuid<TextAsset>("5ec2f7fdcef1e6f45b2c1a7510be3eaa");
                this.playableAsset = EditorUtil.GetAssetByGuid<TextAsset>("e4d5fd6d65c83d24da92fbd00d7f5499");
                this.playableBehaviour = EditorUtil.GetAssetByGuid<TextAsset>("6ccc7dcc8373b7f4197de5cd7d7e7a16");
                this.stateMachineBehaviour = EditorUtil.GetAssetByGuid<TextAsset>("fed9948eb87d1be48ae323bd48cf729f");
                this.subStateMachineBehaviour = EditorUtil.GetAssetByGuid<TextAsset>("09afd0c31b0565e4a8a74ecb68ceef24");
                this.editorTestScript = EditorUtil.GetAssetByGuid<TextAsset>("c31e8a34fb6708144809d22dffdc73f6");

                this.hasBeenInitialized = true;
            }
        }
    }
}

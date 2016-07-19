//-----------------------------------------------------------------------
// <copyright file="StyledText.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using TMPro;
    using UnityEngine;
    
    [ExecuteInEditMode]
    public class StyledText : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private StyleDefinition styleDefinition;
        [SerializeField] private LocText locText;
        [SerializeField] private TextAlignmentOptions alignment;
        [SerializeField] private bool enableWordWrapping;
        #pragma warning restore 0649

        private TextMeshProUGUI textComponentCache;

        //// TODO this way we don't need colons in the loc DB
        //// private bool appendColon;

        public StyleDefinition StyleDefinition
        {
            get { return this.styleDefinition; }
        }

        public LocText LocText
        {
            get { return this.locText; }
        }

        public TextAlignmentOptions Alignment
        {
            get { return this.alignment; }
        }

        public bool EnableWordWrapping
        {
            get { return this.enableWordWrapping; }
        }
        
        /// <summary>
        /// Invalidating and updating the text part of the text component.
        /// </summary>
        public void UpdateTextValue()
        {
            if (this.locText != null && this.textComponentCache != null)
            {
                this.locText.InvalidateCache();
                this.textComponentCache.text = this.locText.Text;
            }
        }

        /// <summary>
        /// Updates the Text component to all the values of the FontStyle data.
        /// </summary>
        public void UpdateFontStyleValues()
        {
            if (this.styleDefinition != null && this.textComponentCache != null)
            {
                var definitionTextComponent = this.styleDefinition.GetComponentsInChildren<TextMeshProUGUI>(true)[0];

                this.textComponentCache.color = definitionTextComponent.color;
                this.textComponentCache.characterSpacing = definitionTextComponent.characterSpacing;
                this.textComponentCache.enableAutoSizing = definitionTextComponent.enableAutoSizing;
                this.textComponentCache.enableCulling = definitionTextComponent.enableCulling;
                this.textComponentCache.enableKerning = definitionTextComponent.enableKerning;
                this.textComponentCache.enableVertexGradient = definitionTextComponent.enableVertexGradient;
                this.textComponentCache.extraPadding = definitionTextComponent.extraPadding;
                this.textComponentCache.faceColor = definitionTextComponent.faceColor;
                this.textComponentCache.font = definitionTextComponent.font;
                this.textComponentCache.fontSharedMaterial = definitionTextComponent.fontSharedMaterial;
                this.textComponentCache.fontSize = definitionTextComponent.fontSize;
                this.textComponentCache.fontSizeMin = definitionTextComponent.fontSizeMin;
                this.textComponentCache.fontSizeMax = definitionTextComponent.fontSizeMax;
                this.textComponentCache.fontStyle = definitionTextComponent.fontStyle;
                this.textComponentCache.horizontalMapping = definitionTextComponent.horizontalMapping;
                this.textComponentCache.isOverlay = definitionTextComponent.isOverlay;
                this.textComponentCache.lineSpacing = definitionTextComponent.lineSpacing;
                this.textComponentCache.outlineColor = definitionTextComponent.outlineColor;
                this.textComponentCache.outlineWidth = definitionTextComponent.outlineWidth;
                this.textComponentCache.overrideColorTags = definitionTextComponent.overrideColorTags;
                this.textComponentCache.verticalMapping = definitionTextComponent.verticalMapping;

                this.textComponentCache.alignment = this.alignment;
                this.textComponentCache.enableWordWrapping = this.enableWordWrapping;
            }
        }
        
        private void Awake()
        {
            this.textComponentCache = this.GetOrAddComponent<TextMeshProUGUI>();

            //// TODO DO NOT UNCOMMENT the HideFlags.DontSave, it will crash unity if the component 
            ////      somehow was saved to disk.  Need to write up a Unity Bug sometime.
            this.textComponentCache.hideFlags = HideFlags.HideInInspector;  // | HideFlags.DontSave;
            this.UpdateFontStyleValues();
            this.UpdateTextValue();
        }
        
        private void OnEnable()
        {
            ObjectTracker.Register<StyledText>(this);
        }
        
        private void OnDisable()
        {
            ObjectTracker.Deregister<StyledText>(this);
        }

        #if UNITY_EDITOR
        private void Update()
        {
            this.UpdateTextValue();
            this.UpdateFontStyleValues();
        }
        #endif
    }
}

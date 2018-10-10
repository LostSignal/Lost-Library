//--------------------------------------------------------------------s---
// <copyright file="DebugMenu.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Text;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;

    public enum Corner
    {
        UpperLeft,
        UpperRight,
        LowerLeft,
        LowerRight
    }

    public class DebugMenu : SingletonDialogResource<DebugMenu>
    {
        #pragma warning disable 0649
        [Header("Camera")]
        [SerializeField] private string sortingLayerName = "DebugMenu";

        [Header("Menu Items")]
        [SerializeField] private GameObject debugMenu;
        [SerializeField] private DebugMenuItem debugMenuItemPrefab;
        [SerializeField] private RectTransform debugMenuItemsHolder;

        [Header("Text Overlay")]
        [SerializeField] private TMP_Text upperLeftText;
        [SerializeField] private TMP_Text upperRightText;
        [SerializeField] private TMP_Text lowerLeftText;
        [SerializeField] private TMP_Text lowerRightText;
        #pragma warning restore 0649

        // fps related variables
        private readonly int fpsUpdateTicks = 10;
        private int fpsCurrentTickCount = 0;
        private float fpsDeltaTime = 0.0f;
        private Corner fpsCorner = Corner.UpperLeft;
        private bool showFps = false;

        private Camera cameraCache = null;

        #region Menu Item Related Methods

        public void ShowMenu()
        {
            this.debugMenu.SetActive(true);
        }

        public void HideMenu()
        {
            this.debugMenu.SetActive(false);
        }

        public void AddItem(string name, UnityAction customAction)
        {
            var newItem = Pooler.Instantiate<DebugMenuItem>(this.debugMenuItemPrefab);
            newItem.transform.SetParent(this.debugMenuItemsHolder);
            newItem.transform.Reset();
            newItem.Initialize(name, customAction, this.HideMenu);
        }

        public void RemoveItem(string name)
        {
            foreach (var menuItem in this.debugMenuItemsHolder.GetComponentsInChildren<DebugMenuItem>())
            {
                if (menuItem.Name == name)
                {
                    Pooler.Destroy(menuItem.gameObject);
                }
            }
        }

        public override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            this.HideMenu();
        }

        #endregion

        #region FPS Related Methods

        public void ShowFPS()
        {
            this.showFps = true;
        }

        public void HideFPS()
        {
            this.showFps = false;
            this.SetText(this.fpsCorner, string.Empty);
        }

        public void ToggleFPS()
        {
            if (this.showFps)
            {
                this.HideFPS();
            }
            else
            {
                this.ShowFPS();
            }
        }

        public void SetFpsCornerAndColor(Corner fpsCorner, Color fpsColor)
        {
            this.fpsCorner = fpsCorner;
            this.SetCornerColor(fpsCorner, fpsColor);
        }

        #endregion

        #region Text Overlay Related Methods

        public void SetText(Corner corner, string text)
        {
            switch (corner)
            {
                case Corner.UpperLeft:
                    this.upperLeftText.text = text;
                    break;

                case Corner.UpperRight:
                    this.upperRightText.text = text;
                    break;

                case Corner.LowerLeft:
                    this.lowerLeftText.text = text;
                    break;

                case Corner.LowerRight:
                    this.lowerRightText.text = text;
                    break;

                default:
                    Debug.LogErrorFormat(this, "TextOverlay found unknown Corner type '{0}'", corner.ToString());
                    break;
            }
        }

        public void SetText(Corner corner, string text, Color color)
        {
            this.SetText(corner, text);
            this.SetCornerColor(corner, color);
        }

        public void SetCornerColor(Corner corner, Color color)
        {
            switch (corner)
            {
                case Corner.UpperLeft:
                    this.upperLeftText.color = color;
                    break;

                case Corner.UpperRight:
                    this.upperRightText.color = color;
                    break;

                case Corner.LowerLeft:
                    this.lowerLeftText.color = color;
                    break;

                case Corner.LowerRight:
                    this.lowerRightText.color = color;
                    break;

                default:
                    Debug.LogErrorFormat(this, "TextOverlay found unknown Corner type '{0}'", corner.ToString());
                    break;
            }
        }

        #endregion

        public override void SetCamera(Camera camera)
        {
            this.cameraCache = camera;
            base.SetCamera(camera);
        }

        protected override void Awake()
        {
            base.Awake();
            this.HideMenu();
            this.Show();

            // setting the canvas sorting layer
            if (SortingLayer.NameToID(this.sortingLayerName) == -1)
            {
                Debug.LogErrorFormat(this, "Trying to use DebugMenu without creating a Sorting Layer of name \"{0}\".  The debug will not work unless create that layer.", this.sortingLayerName);
            }
            else
            {
                this.Canvas.sortingLayerName = this.sortingLayerName;
            }
        }

        private void Update()
        {
            if (!this.cameraCache)
            {
                this.cameraCache = Camera.main;
                this.Canvas.worldCamera = this.cameraCache;
            }

            if (this.showFps == false)
            {
                return;
            }

            this.fpsDeltaTime += Time.deltaTime;

            this.fpsCurrentTickCount++;

            if (this.fpsCurrentTickCount == this.fpsUpdateTicks)
            {
                float msec = (this.fpsDeltaTime / (float)this.fpsUpdateTicks) * 1000.0f;
                float fps = 1.0f / (this.fpsDeltaTime / (float)this.fpsUpdateTicks);
                
                // building the new string
                BetterStringBuilder.New()
                    .Append((int)msec)
                    .Append(".")
                    .Append(this.GetDecimalPoint(msec))
                    .Append(" ms (")
                    .Append((int)fps)
                    .Append(".")
                    .Append(this.GetDecimalPoint(fps))
                    .Append(" fps)")
                    .Set(this.GetCornerText(this.fpsCorner));
                
                this.fpsCurrentTickCount = 0;
                this.fpsDeltaTime = 0.0f;
            }
        }

        private TMP_Text GetCornerText(Corner corner)
        {
            switch (corner)
            {
                case Corner.LowerLeft:
                    return this.lowerLeftText;
                case Corner.LowerRight:
                    return this.lowerRightText;
                case Corner.UpperLeft:
                    return this.upperLeftText;
                case Corner.UpperRight:
                    return this.upperRightText;
                default:
                    return null;
            }
        }

        private int GetDecimalPoint(float num)
        {
            return (int)((num - (int)num) * 10.0f);
        }
    }
}

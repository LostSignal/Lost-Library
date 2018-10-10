//-----------------------------------------------------------------------
// <copyright file="VirtualizedScrollRect.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    [Serializable]
    [RequireComponent(typeof(ScrollRect))]
    public abstract class VirtualizedScrollRect<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        // TODO [bgish]: Can probably combine these
        public enum GrowType
        {
            Vertically,
            Horizontally,
        }

        public enum GrowDirection
        {
            Vertical,
            Horizontal,
            Both,
        }

        public delegate void OnShowItemDelegate(T monoBehaviour, int index);
        public event OnShowItemDelegate OnShowItem;

        #pragma warning disable 0649
        [SerializeField] private GrowType growType;
        [SerializeField] private Vector2 cellSize;
        [SerializeField] private Vector2 cellSpacing;

        [Header("Manual Content Fitter Values")]
        [SerializeField] private GrowDirection growDirection;
        [SerializeField] private float bottomPadding;
        [SerializeField] private float rightPadding;

        [SerializeField] private T dataPrefab;
        #pragma warning restore 0649
                
        private RectTransform itemContainer;
        private ScrollRect scrollRect;

        private Vector3 itemContainerStartPosition;
        private Vector3 cellStartPosition;
        private Vector3 upperRightBounds;
        private Vector3 lowerLeftBounds;

        private List<T> inactivePool = new List<T>();
        private List<T> activePool = new List<T>();

        public Vector3 contentOffset;

        private int activeTopRow = -1;
        private int activeBottomRow = -1;
        private int activeLeftColumn = -1;
        private int activeRightColumn = -1;

        private float horizontalCenteringOffset = 0.0f;
        private float verticalCenteringOffset = 0.0f;
        private int columnCount;
        private int rowCount;
        private int count;

        private bool isInitialized = false;

        public void SetCount(int count)
        {
            this.count = count;
            this.Initialize();
            this.CalculateRowAndColumnCount();

            if (this.growType == GrowType.Vertically)
            {
                float height = ((this.cellSize.y + this.cellSpacing.y) * this.rowCount) - this.cellSpacing.y;
                this.itemContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
            else
            {
                float width = ((this.cellSize.x + this.cellSpacing.x) * this.columnCount) - this.cellSpacing.x;
                this.itemContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            }

            if (Application.isPlaying)
            {
                this.Resize();
            }

            this.ReplaceAllVisibleTiles();
        }

        #if UNITY_EDITOR
        public void OnSceneGUI()
        {
            if (Application.isPlaying)
            {
                return;
            }

            this.ForceInitialize();
            this.CalculateRowAndColumnCount();

            Matrix4x4 viewportLocalToWorld = this.itemContainer.localToWorldMatrix;
            Vector3 lowerLeftWorldPosition = viewportLocalToWorld.MultiplyPoint(this.lowerLeftBounds);
            Vector3 upperRightWorldPosition = viewportLocalToWorld.MultiplyPoint(this.upperRightBounds);
            this.DrawRect(lowerLeftWorldPosition, upperRightWorldPosition);

            for (int i = 0; i < this.columnCount; i++)
            {
                for (int j = 0; j < this.rowCount; j++)
                {
                    Vector3 cellPosition = new Vector3(this.GetColumnX(i), this.GetRowY(j));

                    Matrix4x4 localToWorld = this.itemContainer.localToWorldMatrix;
                    Vector3 lowerLeftCellPosition = localToWorld.MultiplyPoint(cellPosition - (Vector3)(this.cellSize / 2.0f));
                    Vector3 upperRightCellPosition = localToWorld.MultiplyPoint(cellPosition + (Vector3)(this.cellSize / 2.0f));
                    this.DrawRect(lowerLeftCellPosition, upperRightCellPosition);
                }
            }
        }

        private void DrawRect(Vector3 lowerLeft, Vector3 upperRight)
        {
            Vector3 upperLeft = new Vector3(lowerLeft.x, upperRight.y);
            Vector3 lowerRight = new Vector3(upperRight.x, lowerLeft.y);

            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawLine(lowerLeft, upperLeft);
            UnityEditor.Handles.DrawLine(upperLeft, upperRight);
            UnityEditor.Handles.DrawLine(upperRight, lowerRight);
            UnityEditor.Handles.DrawLine(lowerRight, lowerLeft);
        }
        #endif
        
        protected virtual void Awake()
        {
            this.Initialize();

            // TODO [bgish]: Old ManualContentFitter code, still needed?
            if (this.growDirection == GrowDirection.Horizontal || this.growDirection == GrowDirection.Both)
            {
                Debug.LogWarningFormat("ManualContentFitter has not been tested using GrowDirection {0}", this.growDirection.ToString());
            }
        }

        private Vector3 GetUpperLeftCornerOfItemContainer()
        {
            Vector3[] itemContainerWorldCorners = new Vector3[4];
            this.itemContainer.GetWorldCorners(itemContainerWorldCorners);
            return this.scrollRect.viewport.worldToLocalMatrix.MultiplyPoint(itemContainerWorldCorners[1]);
        }

        private void ReplaceAllVisibleTiles()
        {
            this.GetActiveRowsAndColumns(out this.activeTopRow, out this.activeBottomRow, out this.activeLeftColumn, out this.activeRightColumn);

            // putting all active into the deactive pool (aka, moving them slightly off screen)
            Vector3 offscreenLocation = new Vector3(this.GetColumnX(this.activeRightColumn + 2), this.GetRowY(this.activeBottomRow + 2));

            for (int i = 0; i < this.activePool.Count; i++)
            {
                this.activePool[i].transform.localPosition = offscreenLocation;
                this.inactivePool.Add(this.activePool[i]);
            }

            this.activePool.Clear();

            for (int i = this.activeTopRow; i <= this.activeBottomRow; i++)
            {
                for (int j = activeLeftColumn; j <= activeRightColumn; j++)
                {
                    Vector2 position = new Vector2(this.GetColumnX(j), this.GetRowY(i));
                    this.ShowItem(i, j, position);
                }
            }
        }

        private void GetActiveRowsAndColumns(out int topRow, out int bottomRow, out int leftColumn, out int rightColumn)
        {
            // calculating top/bottom rows
            topRow = Mathf.CeilToInt((this.contentOffset.y - (this.upperRightBounds.y - this.cellStartPosition.y)) / (this.cellSize.y + this.cellSpacing.y));
            bottomRow = Mathf.FloorToInt((this.contentOffset.y + (this.cellStartPosition.y - this.lowerLeftBounds.y)) / (this.cellSize.y + this.cellSpacing.y));

            topRow = Mathf.Max(topRow, 0);
            bottomRow = Mathf.Min(bottomRow, this.rowCount - 1);

            // TODO [bgish]:  We're assuming that all columns are always present.  These really need to be calculated.
            // calculating left/right columns
            leftColumn = 0;
            rightColumn = this.columnCount - 1;
        }
        
        private void ShowItem(int row, int column, Vector2 localPosition)
        {
            int index = column + (row * this.columnCount);

            if (index >= this.count)
            {
                return;
            }

            T newItem = null;

            if (this.inactivePool.Count == 0)
            {
                newItem = GameObject.Instantiate<T>(this.dataPrefab, this.itemContainer);
            }
            else
            {
                int lastIndex = this.inactivePool.Count - 1;
                newItem = this.inactivePool[lastIndex];
                this.inactivePool.RemoveAt(lastIndex);
            }

            this.activePool.Add(newItem);
            newItem.transform.Reset();
            newItem.transform.localPosition = localPosition;

            if (this.OnShowItem != null)
            {
                this.OnShowItem(newItem, index);
            }
        }

        private void ForceInitialize()
        {
            this.isInitialized = false;
            this.Initialize();
        }

        private void Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }

            this.isInitialized = true;

            this.scrollRect = this.GetComponent<ScrollRect>();
            this.itemContainer = this.scrollRect.content;

            Vector3[] itemContainerCorners = new Vector3[4];
            this.itemContainer.GetLocalCorners(itemContainerCorners);
            this.itemContainerStartPosition = this.GetUpperLeftCornerOfItemContainer();

            this.CalculateCellStartAndBounds();

            if (this.growType == GrowType.Horizontally)
            {
                Debug.LogWarningFormat("GridVirtualizer has not been tested using GrowType {0}", this.growType.ToString());
            }
        }

        private void CalculateCellStartAndBounds()
        {
            // calculating cell start position
            Vector3[] containerCorners = new Vector3[4];
            this.itemContainer.GetLocalCorners(containerCorners);

            Vector3 startOffset = new Vector3(this.cellSize.x / 2.0f, -this.cellSize.y / 2.0f);
            this.cellStartPosition = containerCorners[1] + startOffset;

            // calculating cell bounds
            Vector3[] viewportCorners = new Vector3[4];
            this.scrollRect.viewport.GetWorldCorners(viewportCorners);

            Vector3 viewportLowerLeft = this.itemContainer.transform.worldToLocalMatrix.MultiplyPoint(viewportCorners[0]);
            Vector3 viewportUpperRight = this.itemContainer.transform.worldToLocalMatrix.MultiplyPoint(viewportCorners[2]);

            Vector3 halfCell = this.cellSpacing + (this.cellSize / 2);
            this.lowerLeftBounds = viewportLowerLeft - halfCell;
            this.upperRightBounds = viewportUpperRight + halfCell;
        }

        private void CalculateRowAndColumnCount()
        {
            float containerWidth = this.itemContainer.rect.width;
            float containerHeight = this.itemContainer.rect.height;

            this.columnCount = Mathf.FloorToInt((containerWidth + this.cellSpacing.x) / (this.cellSize.x + this.cellSpacing.x));
            this.rowCount = Mathf.FloorToInt((containerHeight + this.cellSpacing.y) / (this.cellSize.y + this.cellSpacing.y));

            if (this.growType == GrowType.Vertically)
            {
                this.rowCount = Application.isPlaying ? Mathf.CeilToInt(this.count / (float)this.columnCount) : (this.rowCount + 1);
                this.horizontalCenteringOffset = (containerWidth - ((this.cellSize.x + this.cellSpacing.x) * this.columnCount) + this.cellSpacing.x) / 2.0f;
                this.verticalCenteringOffset = 0.0f;
            }
            else
            {
                this.columnCount = Application.isPlaying ? Mathf.CeilToInt(this.count / (float)this.rowCount) : (this.columnCount + 1);
                this.horizontalCenteringOffset = 0.0f;
                this.verticalCenteringOffset = (containerHeight - ((this.cellSize.y + this.cellSpacing.y) * this.rowCount) + this.cellSpacing.y) / 2.0f;
            }
        }

        private void Update()
        {
            if (this.count == 0)
            {
                return;
            }

            this.contentOffset = this.GetUpperLeftCornerOfItemContainer() - this.itemContainerStartPosition;

            int newActiveTopRow = 0;
            int newActiveBottomRow = 0;
            int newActiveLeftColumn = 0;
            int newActiveRightColumn = 0;

            this.GetActiveRowsAndColumns(out newActiveTopRow, out newActiveBottomRow, out newActiveLeftColumn, out newActiveRightColumn);

            if (newActiveTopRow == this.activeTopRow && newActiveBottomRow == this.activeBottomRow && newActiveLeftColumn == this.activeLeftColumn && newActiveRightColumn == this.activeRightColumn)
            {
                return;
            }

            this.activeTopRow = newActiveTopRow;
            this.activeBottomRow = newActiveBottomRow;
            this.activeLeftColumn = newActiveLeftColumn;
            this.activeRightColumn = newActiveRightColumn;

            bool refreshAll = true;

            // TODO [bgish]: Calculate what rows/columns were hidden/shows and only show those
            // TODO [bgish]: If the delta between rows > this.rowCount or delta between columns > this.columnCount, then destroy all children and redraw everything

            if (refreshAll)
            {
                this.ReplaceAllVisibleTiles();
            }
        }

        private float GetColumnX(int columnIndex)
        {
            return this.cellStartPosition.x + this.horizontalCenteringOffset + ((this.cellSize.x + this.cellSpacing.x) * columnIndex);
        }

        private float GetRowY(int rowIndex)
        {
            return this.cellStartPosition.y - this.verticalCenteringOffset - ((this.cellSize.y + this.cellSpacing.y) * rowIndex);
        }

        #region ManualContentFitter Code

        private void Resize()
        {
            // using a coroutine runner, becuase it has to happen even if the gameobject is disabled
            CoroutineRunner.Instance.ExecuteAtEndOfFrame(this.ResizeInternal);
        }

        private void ResizeInternal()
        {
            if (!this.itemContainer || this.itemContainer.childCount == 0)
            {
                return;
            }

            Vector3 scale = this.transform.lossyScale;
            Vector3[] corners = new Vector3[4];

            RectTransform rectTransform = (RectTransform)this.transform;
            rectTransform.GetWorldCorners(corners);

            float currentBottomY = corners[0].y / scale.y;
            float currentHeight = (corners[1].y - corners[0].y) / scale.y;

            float currentRightX = corners[3].x / scale.x;
            float currentWidth = (corners[3].x - corners[0].x) / scale.x;

            // setting the min to the first child in the list
            ((RectTransform)this.transform.GetChild(0)).GetWorldCorners(corners);
            float minBottomY = corners[0].y / scale.y;
            float maxRightX = corners[3].x / scale.x;

            // going through the rest of the children to find the actual min
            for (int i = 1; i < this.transform.childCount; i++)
            {
                ((RectTransform)this.transform.GetChild(i)).GetWorldCorners(corners);
                minBottomY = Mathf.Min(minBottomY, corners[0].y / scale.y);
                maxRightX = Mathf.Max(maxRightX, corners[3].x / scale.x);
            }

            float verticalGrowthAmmount = (currentBottomY - minBottomY) + this.bottomPadding;
            float horizontalGrowthAmmount = (maxRightX - currentRightX) + this.rightPadding;

            if (this.growDirection == GrowDirection.Vertical || this.growDirection == GrowDirection.Both)
            {
                this.itemContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentHeight + verticalGrowthAmmount);
            }

            if (this.growDirection == GrowDirection.Horizontal || this.growDirection == GrowDirection.Both)
            {
                this.itemContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth + horizontalGrowthAmmount);
            }
        }

        #endregion
    }
}

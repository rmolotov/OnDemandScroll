using System;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Extensions
{
    public static class UIExtensions 
    {
        #region ScrollRect

        public static void ScrollToTop(this ScrollRect scrollRect) => 
            scrollRect.normalizedPosition = Vector2.up;

        public static void ScrollToBottom(this ScrollRect scrollRect) => 
            scrollRect.normalizedPosition = Vector2.zero;

        #endregion

        #region GridLayoutGroup

        public static int RowCapacity(this GridLayoutGroup layoutGroup)
        {
            var w = ((RectTransform) layoutGroup.transform).rect.width
                    - layoutGroup.padding.left
                    - layoutGroup.padding.right
                    + layoutGroup.spacing.x;
            
            var c = Mathf.FloorToInt(w / (layoutGroup.cellSize.x + layoutGroup.spacing.x));
            return Mathf.Clamp(c, 1, c);
        }

        public static int RowsCount(this GridLayoutGroup layoutGroup, int itemsCount =-1)
        {
            var rowCapacity = layoutGroup.RowCapacity();
            
            if (itemsCount < 0) 
                itemsCount = layoutGroup.transform.childCount;

            return Mathf.CeilToInt(itemsCount / (float) rowCapacity - 0.001f);
        }

        public static int CellsInLastRow(this GridLayoutGroup layoutGroup, int itemsCount =-1)
        {
            if (itemsCount < 0) 
                itemsCount = layoutGroup.transform.childCount;
            
            var columns = layoutGroup.RowCapacity();
            return itemsCount % columns;
        }
        
        public static int VisibleRows(this GridLayoutGroup layoutGroup)
        {
            var scrollRect = layoutGroup.GetComponentInParent<ScrollRect>();
            if (scrollRect == null)
            {
                Debug.LogWarning("Target GridLayoutGroup has no ScrollRect in parent RectTransform");
                return -1;
            }
            
            var h = scrollRect.viewport.rect.height
                    - layoutGroup.padding.top
                    - layoutGroup.padding.bottom
                    + layoutGroup.spacing.y;
            
            return Mathf.CeilToInt(h / (layoutGroup.cellSize.y + layoutGroup.spacing.y));
        }

        public static (int, int) VisibleRowsIndexes(this GridLayoutGroup layoutGroup, int itemsCount =-1)
        {
            var scrollRect = layoutGroup.GetComponentInParent<ScrollRect>();
            if (scrollRect == null)
            {
                Debug.LogWarning("Target GridLayoutGroup has no ScrollRect in parent RectTransform");
                return (-1, -1);
            }

            var v = Mathf.Clamp01(scrollRect.normalizedPosition.y);
            
            // bound cases:
            switch (v)
            {
                case > 1 - Constants.Epsilon:
                    return (0, layoutGroup.VisibleRows());
                case < 0 + Constants.Epsilon:
                    return (
                        layoutGroup.RowsCount(itemsCount) - layoutGroup.VisibleRows(),
                        layoutGroup.RowsCount(itemsCount)
                    );
            }

            var c = (int) ((1 - v) * layoutGroup.RowsCount(itemsCount));
            var t = Mathf.Clamp(c - layoutGroup.VisibleRows() / 2, 0, c - layoutGroup.VisibleRows() / 2);
            var b = Mathf.Clamp(c + layoutGroup.VisibleRows() / 2, c + layoutGroup.VisibleRows() / 2,
                layoutGroup.RowsCount(itemsCount));

            if (t == 0) 
                b = layoutGroup.VisibleRows();
            if (b == layoutGroup.RowsCount(itemsCount)) 
                t = layoutGroup.RowsCount(itemsCount) - layoutGroup.VisibleRows();

            return (t, b);
        }
        
        public static (int, int) CalcPageBounds(this GridLayoutGroup layoutGroup, int maxItemsCount)
        {
            var columns = layoutGroup.RowCapacity();
            var (t, b) = layoutGroup.VisibleRowsIndexes();
            
            var firstInd = Math.Clamp(t * (columns), 0, maxItemsCount);
            var lastInd = Math.Clamp((b + 1) * columns, 0, maxItemsCount) - 1;

            return (firstInd, lastInd);
        }

        #endregion
    }
}
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
            
            return Mathf.FloorToInt(
                w / (layoutGroup.cellSize.x + layoutGroup.spacing.x)
            );
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

        #endregion
    }
}
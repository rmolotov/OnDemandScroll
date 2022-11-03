﻿using CodeBase.Extensions;
using UnityEngine;
using UnityEngine.UI;
using CodeBase.Infrastructure;

namespace CodeBase.UI
{
    public class MenuController : MonoBehaviour
    {
        private const float ImageSize = 0.5f;
        private const float ContainerSpacing = 0.1f;
        private const float ContainerPaddingHorizontal = 0.1f;
        private const float ContainerPaddingVertical = 0.1f;
        private const float ButtonPadding = 0.05f;

        [SerializeField] private MenuItemDetailsPopup menuItemDetailsPopup;
        [SerializeField] private GridLayoutGroup itemsContainer;
        [SerializeField] private ScrollRect scrollView;
        [SerializeField] private VerticalLayoutGroup buttonsContainer;
        
        [SerializeField] private GameObject itemSlopPrefab;
        [SerializeField] private Sprite defSprite;

        public int itemsCount;

        private void Awake()
        {
            SetLayout();
            CalcViewportSettings();
            SpawnCells();
        }

        public void ScrollToStart() => 
            scrollView.ScrollToTop();

        public void ScrollToEnd() => 
            scrollView.ScrollToBottom();

        private void SetLayout()
        {
            var screenHeight = DeviceInfoService.Instance.GetScreenHeight();

            SetGridLayout(screenHeight);
            SetButtonLayout(screenHeight);
        }

        private void CalcViewportSettings()
        {
            var cells = GetItemsCount();
            
            var columns = itemsContainer.RowCapacity();
            var rows = itemsContainer.RowsCount(cells);
            var lastRow = itemsContainer.CellsInLastRow(cells);
            
            Debug.Log($"Grid is {rows} * {columns} with {lastRow} cells in last row for {cells} cells in total");
        }

        private void SpawnCells()
        {
            for (var i = 0; i < itemsCount; i++)
            {
                var view = Instantiate(itemSlopPrefab, itemsContainer.transform).GetComponent<MenuItemView>();
                view.Construct(defSprite);
                view.OnClick += arg => menuItemDetailsPopup.InitAndShow(arg, arg);
            }
        }

        //todo: move to resources service;
        private int GetItemsCount()
        {
            return itemsCount;
        }

        private void SetGridLayout(float screenHeight)
        {
            itemsContainer.cellSize = screenHeight * ImageSize * Vector2.one;
            itemsContainer.spacing = screenHeight * ContainerSpacing * Vector2.one;
            itemsContainer.padding = new RectOffset(
                (int) (screenHeight * ContainerPaddingVertical),
                (int) (screenHeight * ContainerPaddingVertical),
                (int) (screenHeight * ContainerPaddingHorizontal),
                (int) (screenHeight * ContainerPaddingHorizontal)
            );
        }

        private void SetButtonLayout(float screenHeight)
        {
            ((RectTransform) buttonsContainer.transform).anchoredPosition = ButtonPadding * screenHeight * new Vector2(-1, 1);
            buttonsContainer.spacing = ButtonPadding * screenHeight;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.AssetsManagement;
using CodeBase.Extensions;

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
        [SerializeField] private Button scrollToEndButton;
        [SerializeField] private GameObject itemSlopPrefab;

        private List<string> _spritesAssets;
        private int _columns, _rows, _lastRow, _visibleRows;
        private int _topCount, _bottomCount;
        private int _keepTopCount, _keepBottomCount;

        private int _firstView, _lastView;

        private float _prevPosY;
        private bool _spawnDown, _spawnUp;
        

        private async void Awake()
        {
            //TODO: move to bootstrap scene, addressables/res init inside!
            _spritesAssets = await IAssetsService.Instance.GetAssetsList<Sprite>();
            //_spritesAssets = _spritesAssets.Take(2).ToList(); // for test

            SetLayout();
            CalcViewportSettings();
            
            SpawnStartCells();
            //SpawnAllCells(); // for test

            scrollView.onValueChanged.AddListener(SpawnOnDemand);
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
            _columns = itemsContainer.RowCapacity();
            _rows = itemsContainer.RowsCount(_spritesAssets.Count);
            _lastRow = itemsContainer.CellsInLastRow(_spritesAssets.Count);
            _visibleRows = itemsContainer.VisibleRows();
            
            print($"Grid is {_rows} * {_columns} with {_lastRow} cells in last row and {_visibleRows} visibleRows for {_spritesAssets.Count} cells in total");
        }

        private MenuItemView SpawnCell(string spriteName)
        {         
            var view = Instantiate(itemSlopPrefab, itemsContainer.transform).GetComponent<MenuItemView>();

            view.Construct(spriteName);
            view.OnClick += _ => menuItemDetailsPopup.InitAndShow(spriteName, spriteName);
            
            view.transform.name = spriteName;

            return view;
        }

        private void SpawnAllCells()
        {
            foreach (var t in _spritesAssets) 
                SpawnCell(t);
        }

        private void SpawnStartCells()
        {
            _keepTopCount = _topCount = Mathf.Min(_spritesAssets.Count, _columns * (_visibleRows+1));
            
            for (var i = 0; i < _topCount; i++) 
                SpawnCell(_spritesAssets[i]);

            if (_topCount == _spritesAssets.Count) return;

            var remainder = Math.Max(_spritesAssets.Count - _topCount - _lastRow, 0);
            _keepBottomCount = _bottomCount = Math.Min(remainder, _columns * (_visibleRows + 1)) + _lastRow;
            
            for (var i = _bottomCount; i > 0; i--) 
                SpawnCell(_spritesAssets[^i]);

            (_firstView, _lastView) = itemsContainer.CalcPageBounds(_spritesAssets.Count);
            print($"scroll init {_firstView}, {_lastView}");

            scrollToEndButton.interactable = _topCount + _bottomCount == _spritesAssets.Count;
        }

        private void SpawnOnDemand(Vector2 updatedPos)
        {
            var (t, b) = itemsContainer.VisibleRowsIndexes();
            
            //scroll down
            if (updatedPos.y < _prevPosY && Mathf.Clamp((b + 1) * _columns, 0, _spritesAssets.Count) > _lastView + 1)
            {
                _prevPosY = updatedPos.y;

                if (!_spawnUp)
                    ScrollDown();
                else
                    _spawnUp = false;
                
                return;
            }

            //scroll up
            if (updatedPos.y > _prevPosY && Mathf.Clamp(t * _columns, 0, _spritesAssets.Count) < _firstView)
            {
                _prevPosY = updatedPos.y;

                if (!_spawnDown)
                    ScrollUp();
                else
                    _spawnDown = false;
            }

            // else just upd value
            _prevPosY = updatedPos.y;
        }

        private void ScrollUp()
        {
            (_firstView, _lastView) = itemsContainer.CalcPageBounds(_spritesAssets.Count);
            print($"scroll up {_firstView}, {_lastView}");
            
            ReleaseSprites(_lastView + 1, _lastView + 1 + _columns);

            if (_firstView < _spritesAssets.Count - _bottomCount && _firstView > _topCount)
                SpawnNewLine(_firstView, _columns);
            else
                ReConstructViews(_firstView, _firstView + _columns);
        }

        private void ScrollDown()
        {
            (_firstView, _lastView) = itemsContainer.CalcPageBounds(_spritesAssets.Count);
            print($"scroll down {_firstView}, {_lastView}");

            ReleaseSprites(_firstView - _columns, _firstView);

            if (_lastView > _topCount && _lastView < _spritesAssets.Count - _bottomCount)
                SpawnNewLine(_topCount, _columns, true);
            else
                ReConstructViews(_lastView + 1 - _columns, _lastView + 1);
        }

        private void SpawnNewLine(int start, int count, bool isUnder = false)
        {
            for (var i = isUnder ? 0 : start; i < count; i++)
                SpawnCell(_spritesAssets[start + i])
                    .transform.SetSiblingIndex(start + i);

            if (isUnder)
            {
                _topCount += _columns;
                _spawnDown = true;
            }
            else
            {
                _bottomCount += _columns;
                _spawnUp = true;
            }
            _prevPosY = scrollView.normalizedPosition.y;
            
            //enable fast scroll to end only when all of views are ready
            scrollToEndButton.interactable = itemsContainer.transform.childCount == _spritesAssets.Count;
        }

        private void ReConstructViews(int startIndex, int endIndex)
        {
            for (var i = startIndex; i < endIndex && IsIndexInBounds(i); i++)
                itemsContainer.transform.GetChild(i)
                    .GetComponent<MenuItemView>()
                    .Construct(_spritesAssets[i]);

            bool IsIndexInBounds(int ind) => 
                ind >= 0 && ind < itemsContainer.transform.childCount;
        }

        private void ReleaseSprites(int startIndex, int endIndex)
        {
            if (startIndex <= _keepTopCount || startIndex >= _spritesAssets.Count - _keepBottomCount)
                return;

            for (var i = startIndex; i < endIndex; i++)
            {
                itemsContainer.transform.GetChild(i).GetComponent<Image>().sprite = null;
                IAssetsService.Instance.ReleaseSprite(_spritesAssets[i]);
            }
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
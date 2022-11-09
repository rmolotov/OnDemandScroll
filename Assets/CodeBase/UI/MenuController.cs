﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        [SerializeField] private GameObject itemSlopPrefab;

        private List<string> _spritesAssets;
        private int _columns, _rows, _lastRow, _visibleRows;
        private int _topCount, _bottomCount;

        private int _firstView, _lastView;

        private float _prevPosY;
        private bool _spawnDown, _spawnUp;
        

        private async void Awake()
        {
            //TODO: move to bootstrap scene, addressables/res init inside!
            _spritesAssets = await GetSpritesAssets();
            
            //_spritesAssets = _spritesAssets.Take(2).ToList();

            SetLayout();
            CalcViewportSettings();
            
            SpawnStartCells();
            //SpawnAllCells();

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
            
            Debug.Log($"Grid is {_rows} * {_columns} with {_lastRow} cells in last row and {_visibleRows} visibleRows for {_spritesAssets.Count} cells in total");
        }

        private void SpawnAllCells()
        {
            foreach (var t in _spritesAssets) 
                SpawnCell(t);
        }

        private MenuItemView SpawnCell(string spriteName)
        {         
            var view = Instantiate(itemSlopPrefab, itemsContainer.transform).GetComponent<MenuItemView>();

            view.Construct(spriteName);
            view.OnClick += _ => menuItemDetailsPopup.InitAndShow(spriteName, spriteName);
            
            view.transform.name = spriteName;

            return view;
        }

        private void SpawnStartCells()
        {
            _topCount = Mathf.Min(_spritesAssets.Count, _columns * (_visibleRows + 1));
            
            for (var i = 0; i < _topCount; i++) 
                SpawnCell(_spritesAssets[i]);

            if (_topCount == _spritesAssets.Count) return;

            var remainder = Math.Max(_spritesAssets.Count - _topCount - _lastRow, 0);
            _bottomCount = Math.Min(remainder, _columns * (_visibleRows + 1)) + _lastRow;
            
            for (var i = _bottomCount; i > 0; i--) 
                SpawnCell(_spritesAssets[^i]);

            var (t, b) = itemsContainer.VisibleRowsIndexes();
            _firstView = Math.Clamp((t - 1) * (_columns), 0, _spritesAssets.Count);
            _lastView = Math.Clamp((b + 1) * _columns, 0, _spritesAssets.Count) -1;
            print($"scroll init {_firstView}, {_lastView}");
        }
        
        private async Task<List<string>> GetSpritesAssets()
        {
            //TODO: move to to bootstrap scene;
            return await IAssetsService.Instance.GetAssetsList<Sprite>();
        }

        private void SpawnOnDemand(Vector2 updatedPos)
        {
            var (t, b) = itemsContainer.VisibleRowsIndexes();
            
            //scroll down
            if (updatedPos.y < _prevPosY && Mathf.Clamp((b + 1) * _columns, 0, _spritesAssets.Count) > _lastView + 1)
            {
                _prevPosY = updatedPos.y;
                
                if (_spawnUp)
                {
                    _spawnUp = false;
                    return;
                }
                
                CalcPageBounds(t, b);
                print($"scroll down {_firstView}, {_lastView}");

                //release sprites above:
                for (int i = _firstView-_columns; i < _firstView; i++)
                {
                    itemsContainer.transform.GetChild(i).GetComponent<Image>().sprite = null;
                    IAssetsService.Instance.ReleaseSprite(_spritesAssets[i]);
                }

                //spawn cells under:
                if (_lastView > _topCount && _lastView < _spritesAssets.Count - _bottomCount)
                {
                    for (var i = 0; i < _columns; i++)
                        SpawnCell(_spritesAssets[_topCount + i])
                            .transform.SetSiblingIndex(_topCount + i);

                    _topCount += _columns;
                    _prevPosY = scrollView.normalizedPosition.y;
                    _spawnDown = true;
                }
                // or re-init existing
                else
                {
                    for (var i = _lastView+1; i < _lastView+1+_columns && i < itemsContainer.transform.childCount; i++)
                    {
                        itemsContainer.transform.GetChild(i)
                            .GetComponent<MenuItemView>()
                            .Construct(_spritesAssets[i]);
                    }  
                }
                
                return;
            }

            //scroll up
            if (updatedPos.y > _prevPosY && Mathf.Clamp((t) * _columns, 0, _spritesAssets.Count) < _firstView)
            {
                _prevPosY = updatedPos.y;
                
                if (_spawnDown)
                {
                    _spawnDown = false;
                    return;
                }

                CalcPageBounds(t, b);
                print($"scroll up {_firstView}, {_lastView}");

                //release sprites under:
                for (var i = _lastView+1; i < _lastView+1+_columns; i++)
                {
                    itemsContainer.transform.GetChild(i).GetComponent<Image>().sprite = null;
                    IAssetsService.Instance.ReleaseSprite(_spritesAssets[i]);
                }
                
                //spawn cells above:
                if (false)
                {
                    //TODO
                    _spawnUp = true;
                }
                // or re-init existing
                else
                {
                    for (var i = _firstView - _columns; i < _firstView && i >= 0; i++)
                    {
                        itemsContainer.transform.GetChild(i)
                            .GetComponent<MenuItemView>()
                            .Construct(_spritesAssets[i]);
                    }  
                }
            }

            _prevPosY = updatedPos.y;
        }

        private void CalcPageBounds(int t, int b)
        {
            _firstView = Math.Clamp((t) * (_columns), 0, _spritesAssets.Count);
            _lastView = Math.Clamp((b + 1) * _columns, 0, _spritesAssets.Count) - 1;
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
        
        private IEnumerator WaitForFramesCoroutine(int framesToWait = 1)
        {
            //TODO: move to separated i-service
            for (var i = 0; i < framesToWait; i++)
                yield return null;
        }
    }
}
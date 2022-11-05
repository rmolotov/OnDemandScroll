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

        private async void Awake()
        {
            //TODO: move to bootstrap scene, addressables init inside!
            _spritesAssets = await GetSpritesAssets();
            
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
            var columns = itemsContainer.RowCapacity();
            var rows = itemsContainer.RowsCount(_spritesAssets.Count);
            var lastRow = itemsContainer.CellsInLastRow(_spritesAssets.Count);
            var visibleRows = itemsContainer.VisibleRows();
            
            Debug.Log($"Grid is {rows} * {columns} with {lastRow} cells in last row and {visibleRows} visibleRows for {_spritesAssets.Count} cells in total");
        }

        private void SpawnCells()
        {
            for (var i = 0; i < _spritesAssets.Count; i++)
            {
                var spriteName = _spritesAssets[i];
                var view = Instantiate(itemSlopPrefab, itemsContainer.transform).GetComponent<MenuItemView>();
                
                view.Construct(spriteName);
                view.OnClick += _ => menuItemDetailsPopup.InitAndShow(spriteName, spriteName);
            }
        }

        //TODO: move to to bootstrap scene;
        private async Task<List<string>> GetSpritesAssets()
        {
            return await AddressablesService.Instance.GetAddressablesList<Sprite>();
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
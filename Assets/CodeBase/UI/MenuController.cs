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
        [SerializeField] private VerticalLayoutGroup buttonsContainer;

        private void Awake()
        {
            SetLayout();
            
            //for debug layout and interaction
            foreach (var menuItemView in itemsContainer.GetComponentsInChildren<MenuItemView>())
                menuItemView.OnClick += arg => menuItemDetailsPopup.InitAndShow(arg, arg);
        }
        
        private void SetLayout()
        {
            var screenHeight = DeviceInfoService.Instance.GetScreenHeight();

            SetGridLayout(screenHeight);
            SetButtonLayout(screenHeight);
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
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.AssetsManagement;

namespace CodeBase.UI
{
    public class MenuItemDetailsPopup : PopupBase
    {
        private const float ImageSize = 0.8f;
        private const float TextSize = 0.08f;
        private const float ButtonPadding = 0.05f;
        
        [SerializeField] private Image itemImage;
        [SerializeField] private RectTransform textContainer;

        
        public override async void InitAndShow<T>(T data, string text = "")
        {
            var spriteName = data as string;
            await LoadSprite(spriteName);
            
            SetLayout();

            base.InitAndShow(data, text);
        }

        private async Task LoadSprite(string spriteName)
        {
            var sprite = await IAssetsService.Instance.GetSpriteById(spriteName);
            itemImage.sprite = sprite;
        }

        private void SetLayout()
        {
            var screenHeight = DeviceInfoService.Instance.GetScreenHeight();

            SetImageSize(screenHeight);
            SetTextSize(screenHeight);
            SetButtonLayout(screenHeight);
        }

        private void SetButtonLayout(float screenHeight)
        {
            ((RectTransform) okButton.transform).anchoredPosition = ButtonPadding * screenHeight * new Vector2(1, -1);
        }

        private void SetTextSize(float screenHeight)
        {
            popupText.rectTransform.sizeDelta = TextSize * screenHeight * Vector2.one;
            textContainer.sizeDelta = (.5f * (1 - ImageSize)) * screenHeight * Vector2.one;
        }

        private void SetImageSize(float screenHeight)
        {
            itemImage.rectTransform.sizeDelta = ImageSize * screenHeight * Vector2.one;
        }
    }
}
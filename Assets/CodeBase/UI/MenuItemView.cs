using System;
using UnityEngine;
using UnityEngine.UI;
using CodeBase.Infrastructure.AssetsManagement;

namespace CodeBase.UI
{
    [RequireComponent(typeof(Button))]
    public class MenuItemView : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private Button itemButton;
        
        public event Action<string> OnClick;

        
        public async void Construct(string spriteName)
        {
            itemImage.sprite = await IAssetsService.Instance.GetSpriteById(spriteName);
            itemButton.onClick.AddListener(
                () => OnClick?.Invoke(spriteName)
            );
        }
    }
}
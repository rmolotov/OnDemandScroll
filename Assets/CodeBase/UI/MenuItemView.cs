using System;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    [RequireComponent(typeof(Button))]
    public class MenuItemView : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private Button itemButton;
        
        public event Action<string> OnClick;

        public void Construct(Sprite sprite)
        {
            itemImage.sprite = sprite;
            itemButton.onClick.AddListener(
                () => OnClick?.Invoke(sprite.name)
            );
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public class MenuItemDetailsPopup : PopupBase
    {
        [SerializeField] private Image itemImage;
        public override void InitAndShow<T>(T data, string text = "")
        {
            var spriteName = data as string;
            itemImage.sprite = Resources.Load<Sprite>(spriteName);
            
            base.InitAndShow(data, text);
        }
    }
}
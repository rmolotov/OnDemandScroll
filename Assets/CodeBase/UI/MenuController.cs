using UnityEngine;

namespace CodeBase.UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private MenuItemDetailsPopup menuItemDetailsPopup;
        [SerializeField] private RectTransform itemsContainer;

        private void Awake()
        {
            //for debug layout and interaction
            foreach (var menuItemView in itemsContainer.GetComponentsInChildren<MenuItemView>())
                menuItemView.OnClick += arg => menuItemDetailsPopup.InitAndShow(arg, arg);
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    [RequireComponent(typeof(Button))]
    public class MenuItemView : MonoBehaviour
    {
        [SerializeField] private string spriteName;
        
        public event Action<string> OnClick;

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() => OnClick?.Invoke(spriteName));
        }
    }
}
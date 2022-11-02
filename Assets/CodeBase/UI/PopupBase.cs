using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace CodeBase.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class PopupBase : MonoBehaviour
    {
        [SerializeField] protected Button okButton;
        [SerializeField] protected TextMeshProUGUI popupText;

        [SerializeField] private CanvasGroup canvasGroup;

        private void Awake() => 
            SetVisible(false);

        public virtual void InitAndShow<T>(T data, string text = "")
        {
            SetupPopupText(text);
            SetupPopupButton();
            
            SetVisible(true);
        }

        protected virtual void Close()
        {
            okButton.onClick.RemoveAllListeners();
            SetVisible(false);
        }

        protected void SetVisible(bool state)
        {
            canvasGroup.blocksRaycasts = state;
            var endValue = state 
                ? 1f 
                : 0f;
            
            canvasGroup
                .DOFade(endValue, 0.25f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => { });
        }

        private void SetupPopupButton()
        {
            okButton.onClick.AddListener(() =>
            {
                //SoundService.PlayEffectSingle("SFX_TapInterface");
                //VibrationService.Haptic(HapticPatterns.PresetType.Success);
            });
            okButton.onClick.AddListener(Close);
        }

        private void SetupPopupText(string text)
        {
            if (popupText && !string.IsNullOrEmpty(text))
                popupText.text = text;
        }
    }
}
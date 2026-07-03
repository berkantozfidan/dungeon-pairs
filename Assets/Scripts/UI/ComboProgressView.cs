using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonPairs.UI
{
    public class ComboProgressView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image[] parts;
        [SerializeField] private Color inactiveColor = new Color(1f, 1f, 1f, 0.1f);
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private float fadeDuration = 0.2f;

        private Tween tween;

        private void Awake()
        {
            HideInstant();
        }

        public void Show(int comboLength)
        {

            if (canvasGroup != null)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.alpha = 0f;

                tween?.Kill();
                tween = DOTween.To(
                                () => canvasGroup.alpha,
                                value => canvasGroup.alpha = value,
                                1f,
                                fadeDuration);
            }

            for (int i = 0; i < parts.Length; i++)
            {
                bool isUsedSlot = i < comboLength;
                parts[i].gameObject.SetActive(isUsedSlot);
                parts[i].color = inactiveColor;
                parts[i].transform.localScale = Vector3.one;
            }
        }

        public void SetProgress(int completedParts)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                if (!parts[i].gameObject.activeSelf)
                {
                    continue;
                }

                bool isActive = i < completedParts;
                parts[i].color = isActive ? activeColor : inactiveColor;

                if (isActive)
                {
                    parts[i].transform
                        .DOPunchScale(Vector3.one * 0.15f, 0.2f, 6, 0.7f);
                }
            }
        }

        public void Complete()
        {
            transform
                .DOPunchScale(Vector3.one * 0.2f, 0.25f, 8, 0.8f)
                .OnComplete(Hide);
        }

        public void Fail()
        {
            transform
                .DOShakePosition(0.25f, new Vector3(12f, 0f, 0f), 12, 0f)
                .OnComplete(Hide);
        }

        public void Hide()
        {
            if (canvasGroup == null)
            {
                gameObject.SetActive(false);
                return;
            }

            tween?.Kill();
            tween = DOTween.To(
                            () => canvasGroup.alpha,
                            value => canvasGroup.alpha = value,
                            0f,
                            fadeDuration)
                        .OnComplete(() =>
                                    {
                                        canvasGroup.interactable = false;
                                        canvasGroup.blocksRaycasts = false;
                                    });
        }

        private void HideInstant()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        private void OnDestroy()
        {
            tween?.Kill();
        }
    }
}
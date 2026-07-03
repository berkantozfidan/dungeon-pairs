using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace DungeonPairs.UI
{
    public class BattleTransitionView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private CanvasGroup finalFade;
        [SerializeField] private float fadeDuration = 0.35f;
        [SerializeField] private float holdDuration = 0.25f;

        private void Awake()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
            }
        }

        public IEnumerator PlayTransition(Action onHidden)
        {
            if (canvasGroup == null)
            {
                onHidden?.Invoke();
                yield break;
            }

            canvasGroup.blocksRaycasts = true;

            yield return canvasGroup
                .DOFade(1f, fadeDuration)
                .WaitForCompletion();

            onHidden?.Invoke();

            yield return new WaitForSeconds(holdDuration);


            yield return canvasGroup
                .DOFade(0f, fadeDuration)
                .WaitForCompletion();

            canvasGroup.blocksRaycasts = false;
        }
        public IEnumerator PlayFinalFade(Action onComplete)
        {
            finalFade.gameObject.SetActive(true);
            finalFade.alpha = 0f;

            yield return finalFade
                .DOFade(1f, 1.5f)
                .SetEase(Ease.InOutQuad)
                .WaitForCompletion();

            yield return new WaitForSeconds(0.5f);

            onComplete?.Invoke();
        }
    }
}
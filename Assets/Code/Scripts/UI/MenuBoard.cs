using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootingRangeGame.UI
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class MenuBoard : MonoBehaviour
    {
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private float transitionDuration;
        [SerializeField] private float transitionDistance;

        private CanvasGroup[] groups;

        private int targetIndex = 0;
        private int currentIndex = 0;
        private bool transitionActive;

        public CanvasGroup CurrentGroup => currentIndex >= 0 ? groups[currentIndex] : null;
        public CanvasGroup TargetGroup => currentIndex >= 0 ? groups[targetIndex] : null;

        private void Awake()
        {
            groups = GetComponentsInChildren<CanvasGroup>();

            foreach (var group in groups)
            {
                group.alpha = 0.0f;
                group.interactable = false;
                group.blocksRaycasts = false;
            }
        }

        public void TransitionTo(int targetIndex)
        {
            this.targetIndex = targetIndex;
            StartCoroutine(TransitionRoutine());
        }

        private void EnableGroup(CanvasGroup group, bool state)
        {
            group.interactable = state;
            group.blocksRaycasts = state;
        }

        private IEnumerator TransitionRoutine()
        {
            if (transitionActive) yield break;
            transitionActive = true;

            while (currentIndex != targetIndex)
            {
                float percent;

                IEnumerator animate(Func<float, float> direction)
                {
                    percent = 0.0f;
                    while (percent < 1.0f)
                    {
                        var t = animationCurve.Evaluate(direction(percent));

                        CurrentGroup.alpha = 1.0f - t;
                        CurrentGroup.transform.localPosition = Vector3.down * (t * transitionDistance);

                        percent += Time.deltaTime / transitionDuration;
                        yield return null;
                    }
                }

                EnableGroup(CurrentGroup, false);
                yield return animate(v => v);
                currentIndex = targetIndex;
                EnableGroup(CurrentGroup, false);
                yield return animate(v => 1.0f - v);
            }

            EnableGroup(CurrentGroup, true);
            transitionActive = false;
        }
    }
}
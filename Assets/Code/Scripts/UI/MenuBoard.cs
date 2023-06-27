using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using ShootingRangeGame.Session;
using TMPro;
using UnityEngine;

namespace ShootingRangeGame.UI
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class MenuBoard : MonoBehaviour
    {
        public const int MainMenu = 0;
        public const int Countdown = 1;
        public const int Session = 2;
        public const int PostGame = 3;
        public const int Options = 4;

        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private float transitionDuration;
        [SerializeField] private float transitionDistance;

        private CanvasGroup[] groups;
        private MenuActions menuActions;
        private TMP_Text countdownText;

        private int targetIndex = 0;
        private int currentIndex = 0;
        private bool transitionActive;

        public CanvasGroup CurrentGroup => currentIndex >= 0 ? groups[currentIndex] : null;
        public CanvasGroup TargetGroup => currentIndex >= 0 ? groups[targetIndex] : null;

        private void Awake()
        {
            groups = GetComponentsInChildren<CanvasGroup>(true);

            foreach (var group in groups)
            {
                group.gameObject.SetActive(true);
                group.alpha = 0.0f;
                group.interactable = false;
                group.blocksRaycasts = false;
            }

            countdownText = groups[Countdown].GetComponentInChildren<TMP_Text>(true);

            TransitionTo(0);
        }

        private void OnEnable()
        {
            MenuActions.StartCountdownEvent += OnStartCountdown;
            MenuActions.CountdownEvent += OnCountdown;
            MenuActions.EndCountdownEvent += OnEndCountdown;

            GameSession.EndSessionEvent += OnSessionEnd;
        }

        private void OnDisable()
        {
            MenuActions.StartCountdownEvent -= OnStartCountdown;
            MenuActions.CountdownEvent -= OnCountdown;
            MenuActions.EndCountdownEvent -= OnEndCountdown;

            GameSession.EndSessionEvent -= OnSessionEnd;
        }

        private void OnSessionEnd()
        {
            TransitionTo(PostGame);
        }

        private void OnStartCountdown(float timer)
        {
            TransitionTo(Countdown);
            countdownText.text = ((int)timer).ToString();
        }

        private void OnCountdown(float timer)
        {
            var i = (int)timer;
            countdownText.text = (i + 1).ToString();
        }

        private void OnEndCountdown()
        {
            IEnumerator routine()
            {
                countdownText.text = "Go!";
                yield return new WaitForSeconds(1.0f);
                TransitionTo(Session);
            }

            StartCoroutine(routine());
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

            do
            {
                float percent;

                IEnumerator animate(Func<float, float> direction)
                {
                    if (!CurrentGroup) yield break;

                    percent = 0.0f;
                    while (percent < 1.0f)
                    {
                        var t = animationCurve.Evaluate(direction(percent));

                        CurrentGroup.alpha = t;
                        CurrentGroup.transform.localPosition = Vector3.down * (1.0f - t) * transitionDistance;

                        percent += Time.deltaTime / transitionDuration;
                        yield return null;
                    }

                    CurrentGroup.alpha = direction(1.0f);
                    CurrentGroup.transform.localPosition = Vector3.down * (1.0f - direction(1.0f)) * transitionDistance;
                }

                EnableGroup(CurrentGroup, false);
                yield return StartCoroutine(animate(v => 1.0f - v));
                currentIndex = targetIndex;
                EnableGroup(CurrentGroup, false);
                yield return StartCoroutine(animate(v => v));
            } while (currentIndex != targetIndex);


            EnableGroup(CurrentGroup, true);
            transitionActive = false;
        }

        private void OnValidate()
        {
            NameChildren();
        }

        public void NameChildren()
        {
            var rx = new Regex(@"[a-z].*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
            var children = GetComponentsInChildren<CanvasGroup>();
            for (var i = 0; i < children.Length; i++)
            {
                var child = children[i];
                var name = rx.Match(child.name).Value;
                child.name = $"{i} - {name}";
            }
        }
    }
}
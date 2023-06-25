using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShootingRangeGame
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class PauseMenu : MonoBehaviour
    {
        [SerializeField] private float fadeSpeed = 6.0f;
        [SerializeField] private float offset = 150.0f;

        private CanvasGroup group;
        private bool open;
        private float transition;

        private void Start()
        {
            group = GetComponent<CanvasGroup>();
            
            open = false;
            transition = 0.0f;
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                open = !open;
            }
            
            transition += ((open ? 1.0f : 0.0f) - transition) * fadeSpeed * Time.deltaTime;
            
            group.interactable = open;
            group.blocksRaycasts = open;
            group.alpha = transition * transition * transition;
            ((RectTransform)group.transform).anchoredPosition = Vector2.down * (1.0f - transition) * offset;
        }
    }
}

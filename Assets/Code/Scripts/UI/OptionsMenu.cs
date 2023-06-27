using System;
using ShootingRangeGame.Saves;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShootingRangeGame
{
    public class OptionsMenu : MonoBehaviour
    {
        [SerializeField] private Transform container;

        [Space] [SerializeField] private AudioMixer mixer;

        [Header("Prefabs")] [SerializeField] private GameObject sliderPrefab;

        private void Start()
        {
            SetupMixer();
        }

        private float LinearToDecibels(float v) => Mathf.Log(Mathf.Clamp(v, 0.0001f, 1.0f)) * 20.0f;

        private void SetupMixer()
        {
            void SetupVolumeSlider(string key, float saveValue, Action<float> setSaveValue)
            {
                SetupSlider(key, v =>
                {
                    mixer.SetFloat(key, LinearToDecibels(v / 100.0f));
                    setSaveValue(v);
                    SaveManager.Save();
                }, saveValue, 0.0f, 100.0f);
            }

            SetupVolumeSlider("Master Volume", SaveManager.GetOrLoad().masterVolume, v => SaveManager.GetOrLoad().masterVolume = v);
            SetupVolumeSlider("Effect Volume", SaveManager.GetOrLoad().effectVolume, v => SaveManager.GetOrLoad().effectVolume = v);
            SetupVolumeSlider("Soundtrack Volume", SaveManager.GetOrLoad().soundtrackVolume, v => SaveManager.GetOrLoad().soundtrackVolume = v);
        }

        private void SetupSlider(string name, UnityAction<float> callback, float initialValue = 0.0f, float lower = 0.0f, float upper = 1.0f)
        {
            var transform = Instantiate(sliderPrefab, container).transform;

            var title = transform.GetChild(0).GetComponentInChildren<TMP_Text>();

            var slider = transform.GetChild(1).GetComponentInChildren<Slider>();
            var inputField = transform.GetChild(1).GetComponentInChildren<TMP_InputField>();

            if (title) title.text = name;

            if (slider)
            {
                slider.onValueChanged.AddListener(callback);
                if (inputField)
                {
                    slider.onValueChanged.AddListener(v => inputField.text = v.ToString());
                }

                slider.minValue = lower;
                slider.maxValue = upper;
                slider.wholeNumbers = true;
                slider.value = initialValue;
            }
        }
    }
}
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
        [SerializeField] private string[] volumeParameters;

        [Header("Prefabs")] [SerializeField] private GameObject sliderPrefab;

        private void Start()
        {
            SetupMixer();
        }

        private void SetupMixer()
        {
            foreach (var parameter in volumeParameters)
            {
                SetupSlider(parameter, v => mixer.SetFloat(parameter, v / 100.0f), 0.0f, 100.0f);
            }
        }

        private void SetupSlider(string name, UnityAction<float> callback, float lower = 0.0f, float upper = 1.0f)
        {
            var transform = Instantiate(sliderPrefab, container).transform;

            var title = transform.GetChild(0).GetComponentInChildren<TMP_Text>();

            var slider = transform.GetChild(1).GetComponentInChildren<Slider>();
            var inputField = transform.GetChild(1).GetComponentInChildren<TMP_InputField>();
            
            if (title) title.text = name;
            
            if (slider)
            {
                slider.onValueChanged.AddListener(callback);
                if (inputField) slider.onValueChanged.AddListener(v => inputField.text = v.ToString());
                
                slider.minValue = lower;
                slider.maxValue = upper;
                slider.wholeNumbers = true;
            }

            // if (inputField)
            // {
            //     inputField.onSubmit.AddListener(input =>
            //     {
            //         if (!int.TryParse(input, out var v)) return;
            //         callback(v);
            //
            //         if (slider) slider.value = v;
            //     });
            // }
        }
    }
}
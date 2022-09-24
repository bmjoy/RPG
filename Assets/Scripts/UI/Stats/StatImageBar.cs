using RPGEngine.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGEngine.UI.Stats
{
    public class StatImageBar : StatDisplay
    {
        [SerializeField] private GameObjectFloatGameEvent statMaxValueChanged;
        [SerializeField] private Image statValueImage;
        [SerializeField] private TMP_Text statValueText;

        private bool _hasImage;
        private bool _hasText;
        
        private float _value;
        private float _max;
        private void Awake()
        {
            _hasImage = statValueImage;
            _hasText = statValueText;
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            if (statMaxValueChanged) statMaxValueChanged.RegisterListener(OnStatMaxValueChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (statMaxValueChanged) statMaxValueChanged.UnregisterListener(OnStatMaxValueChanged);
        }

        protected override void OnStatValueChanged(GameObject sender, float statValue)
        {
            //Debug.LogError($"{name}: {sender.name}-{(target != null ? target.name : "null")}");
            if (sender != target) return;
            _value = statValue;
            SetImageFill();
            SetText();
        }

        private void OnStatMaxValueChanged(GameObject sender, float statValue)
        {
            if (sender != target) return;
            _max = statValue;
            SetImageFill();
            SetText();
        }

        private float GetFillPercentage()
        {
            return _max > 0.01f ? _value / _max : 0;
        }

        private void SetImageFill()
        {
            if (_hasImage) statValueImage.fillAmount = GetFillPercentage();
        }

        private void SetText()
        {
            if (_hasText) statValueText.text = $"{_value:F0} / {_max:F0}";
        }
    }
}
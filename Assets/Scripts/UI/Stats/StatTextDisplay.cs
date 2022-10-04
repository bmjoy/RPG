using TMPro;
using UnityEngine;

namespace RPGEngine.UI.Stats
{
    public class StatTextDisplay : StatDisplay
    {
        [SerializeField] private TMP_Text statValueText;
        
        private bool _hasText;

        protected override void Awake()
        {
            base.Awake();
            _hasText = statValueText;
        }

        protected override void OnStatValueChanged(GameObject sender, float value)
        {
            if (_hasText) statValueText.text = $"{value}";
        }
    }
}
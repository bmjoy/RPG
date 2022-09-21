using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGEngine.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TMP_Text healthValueText;
        
        private Health _health;
        private bool _hasSlider;
        private bool _hasText;
        private bool _hasHealth;

        private void Awake()
        {
            _health = GameObject.FindWithTag("Player").GetComponent<Health>();
            _hasHealth = _health;
            _hasSlider = healthSlider;
            _hasText = healthValueText;
        }

        private void Update()
        {
            UpdatePlayerHealth();
        }

        private void UpdatePlayerHealth()
        {
            if (!_hasHealth) return;
            if (_hasSlider) healthSlider.value = _health.GetPercentage();
            if (_hasText) healthValueText.text = $"{_health.Value}/{_health.Max}";
        }
    }
}
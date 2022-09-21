using RPGEngine.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGEngine.Attributes
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TMP_Text healthValueText;
        
        private Health _health;
        private bool _hasHealth;
        private Fighter _player;
        private bool _hasPlayer;
        private bool _hasSlider;
        private bool _hasText;
        private bool _hasTitleText;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            _hasPlayer = _player;
            _hasSlider = healthSlider;
            _hasText = healthValueText;
            _hasTitleText = titleText;
        }

        private void Update()
        {
            if (!_hasPlayer) return;
            _health = _player.GetTarget(out _hasHealth);
            ShowHealth();
            UpdateEnemyHealth();
        }

        private void UpdateEnemyHealth()
        {
            if (!_hasHealth) return;
            if (_hasTitleText) titleText.text = _health.name;
            if (_hasSlider) healthSlider.value = _health.GetPercentage();
            if (_hasText) healthValueText.text = $"{_health.Value}/{_health.Max}";
        }

        private void ShowHealth()
        {
            if (_hasTitleText && titleText.gameObject.activeInHierarchy != _hasHealth)
                titleText.gameObject.SetActive(_hasHealth);
            if (_hasSlider && healthSlider.gameObject.activeInHierarchy != _hasHealth)
                healthSlider.gameObject.SetActive(_hasHealth);
        }
    }
}
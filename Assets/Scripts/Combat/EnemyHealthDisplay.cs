using RPGEngine.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGEngine.Attributes
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject[] healthObjects;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Image healthImage;
        [SerializeField] private TMP_Text healthValueText;
        
        private Health _health;
        private bool _hasHealth;
        private Fighter _player;
        private bool _hasPlayer;
        private bool _hasHealthObjects;
        private bool _hasHealthImage;
        private bool _hasText;
        private bool _hasTitleText;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            _hasPlayer = _player;
            _hasHealthImage = healthImage;
            _hasText = healthValueText;
            _hasTitleText = titleText;
            _hasHealthObjects = healthObjects is { Length: > 0 };
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
            if (_hasHealthImage) healthImage.fillAmount = _health.GetPercentage();
            if (_hasText) healthValueText.text = $"{_health.Value}/{_health.Max}";
        }

        private void ShowHealth()
        {
            foreach (GameObject healthObject in healthObjects)
                healthObject.SetActive(_hasHealth);
        }
    }
}
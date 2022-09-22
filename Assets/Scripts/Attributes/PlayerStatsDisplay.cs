using RPGEngine.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGEngine.Attributes
{
    public class PlayerStatsDisplay : MonoBehaviour
    {
        #region Health Fields

        [SerializeField] private Image healthImage;
        [SerializeField] private TMP_Text healthValueText;
        
        private Health _health;
        private bool _hasHealthImage;
        private bool _hasText;
        private bool _hasHealth;

        #endregion

        #region Experince Fields

        [SerializeField] private Image experienceValueImage;
        [SerializeField] private TMP_Text experienceValueText;
        [SerializeField] private TMP_Text levelValueText;
        
        private Experience _experienceStat;
        private bool _hasExperienceStat;
        private bool _hasExperienceValueImage;
        private bool _hasExperienceText;
        private bool _hasLevelText;

        #endregion

        private BaseStats _baseStats;
        private bool _hasBaseStats;

        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (!player) return;

            _baseStats = player.GetComponent<BaseStats>();
            _hasBaseStats = _baseStats;
            
            _experienceStat = player.GetComponent<Experience>();
            _hasExperienceStat = _experienceStat;
            _hasExperienceValueImage = experienceValueImage;
            _hasExperienceText = experienceValueText;
            _hasLevelText = levelValueText;

            _health = player.GetComponent<Health>();
            _hasHealth = _health;
            _hasHealthImage = healthImage;
            _hasText = healthValueText;
        }

        private void OnEnable()
        {
            if (_hasExperienceStat) _experienceStat.OnExperienceGained += UpdatePlayerExperience;
            if (_hasBaseStats) _baseStats.OnLevelChanged += UpdateLevel;
        }

        private void OnDisable()
        {
            if (_hasExperienceStat) _experienceStat.OnExperienceGained -= UpdatePlayerExperience;
            if (_hasBaseStats) _baseStats.OnLevelChanged -= UpdateLevel;
        }

        private void Update()
        {
            UpdatePlayerHealth();
        }

        private void UpdatePlayerHealth()
        {
            if (!_hasHealth) return;
            if (_hasHealthImage) healthImage.fillAmount = _health.GetPercentage();
            if (_hasText) healthValueText.text = $"{_health.Value:F0}/{_health.Max:F0}";
        }

        private void UpdatePlayerExperience()
        {
            if (!_hasExperienceStat) return;
            if (_hasExperienceValueImage) experienceValueImage.fillAmount = _experienceStat.GetPercentage();
            if (_hasExperienceText) experienceValueText.text = $"{_experienceStat.Value:F0}/{_experienceStat.ExperienceToLevel:F0}";
        }

        private void UpdateLevel(int level)
        {
            UpdatePlayerExperience();
            if (!_hasBaseStats) return;
            if (_hasLevelText) levelValueText.text = $"{level}";
        }
    }
}
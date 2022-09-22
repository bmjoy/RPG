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

        private void Awake()
        {
            _experienceStat = GameObject.FindWithTag("Player").GetComponent<Experience>();
            _hasExperienceStat = _experienceStat;
            _hasExperienceValueImage = experienceValueImage;
            _hasExperienceText = experienceValueText;
            _hasLevelText = levelValueText;

            _health = GameObject.FindWithTag("Player").GetComponent<Health>();
            _hasHealth = _health;
            _hasHealthImage = healthImage;
            _hasText = healthValueText;
        }

        private void Update()
        {
            UpdatePlayerHealth();
            UpdatePlayerExperience();
        }

        private void UpdatePlayerHealth()
        {
            if (!_hasHealth) return;
            if (_hasHealthImage) healthImage.fillAmount = _health.GetPercentage();
            if (_hasText) healthValueText.text = $"{_health.Value}/{_health.Max}";
        }

        private void UpdatePlayerExperience()
        {
            if (!_hasExperienceStat) return;
            if (_hasExperienceValueImage) experienceValueImage.fillAmount = _experienceStat.GetPercentage();
            if (_hasExperienceText) experienceValueText.text = $"{_experienceStat.Value}/{_experienceStat.ExperienceToNextLevel:0}";
            if (_hasLevelText) levelValueText.text = $"{_experienceStat.CurrentLevel}";
        }
    }
}
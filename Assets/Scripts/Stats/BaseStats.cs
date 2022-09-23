using System;
using UnityEngine;

namespace RPGEngine.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField, Range(1, 99)] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression;
        [SerializeField] private GameObject levelUpEffect;

        private Experience _experience;
        private bool _hasExperience;
        private int _currentLevel;

        public int CurrentLevel => _currentLevel;
        public event Action OnLevelChanged;

        private void Awake()
        {
            _experience = GetComponent<Experience>();
            _hasExperience = _experience;
            
            _currentLevel = CalculateLevel();
        }

        private void OnEnable()
        {
            if (_hasExperience) _experience.OnExperienceGained += UpdateLevel;
        }

        private void OnDisable()
        {
            if (_hasExperience) _experience.OnExperienceGained -= UpdateLevel;
        }

        private void UpdateLevel()
        {
            var newLevel = CalculateLevel();
            if (newLevel <= _currentLevel) return;
            _currentLevel = newLevel;
            OnLevelChanged?.Invoke();
            LevelUpEffect();
        }

        private void LevelUpEffect()
        {
            if (levelUpEffect) 
                Instantiate(levelUpEffect, transform);
        }

        public float GetStatValue(Stat stat)
        {
            try
            {
                return progression[characterClass, stat].Calculate(_currentLevel);;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return 0;
            }
        }

        private int CalculateLevel()
        {
            if (!_hasExperience) return startingLevel;
            var level = startingLevel;
            var experienceToNextLevel = GetExperienceNeeded(level);
            while(experienceToNextLevel < _experience.Value)
            {
                level++;
                experienceToNextLevel = GetExperienceNeeded(level);
            }

            return level;
        }

        private float GetExperienceNeeded(int level)
        {
            try
            {
                return progression[characterClass, Stat.ExperienceToLevel].Calculate(level);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
            }
            return Mathf.Infinity;
        }
    }
}
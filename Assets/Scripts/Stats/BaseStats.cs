using System.Collections;
using RPGEngine.Core;
using UnityEngine;

namespace RPGEngine.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField, Range(1, 99)] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression;
        [SerializeField] private GameObjectFloatGameEvent onExperienceChanged;
        [SerializeField] private GameObjectFloatGameEvent onExperienceMaxChanged;
        [SerializeField] private GameObjectFloatGameEvent onLevelChanged;
        [SerializeField] private GameObjectGameEvent onLevelUp;
        [SerializeField] private GameObject levelUpEffect;

        private Experience _experience;
        private float _experienceToNextLevel;
        private bool _hasExperience;
        private int _currentLevel;

        private void Awake()
        {
            _experience = GetComponent<Experience>();
            _hasExperience = _experience;
        }

        private IEnumerator Start()
        {
            _currentLevel = CalculateLevel();
            yield return null;
            if (onExperienceMaxChanged) onExperienceMaxChanged.Invoke(gameObject, _experienceToNextLevel);
            if (onLevelChanged) onLevelChanged.Invoke(gameObject, _currentLevel);
        }

        private void OnEnable()
        {
            if (onExperienceChanged) onExperienceChanged.RegisterListener(UpdateLevel);
        }

        private void OnDisable()
        {
            if (onExperienceChanged) onExperienceChanged.UnregisterListener(UpdateLevel);
        }

        private void UpdateLevel(GameObject sender, float amount)
        {
            if (sender != gameObject) return;
            var newLevel = CalculateLevel();
            if (newLevel <= _currentLevel) return;
            _currentLevel = newLevel;
            if (onLevelUp) onLevelUp.Invoke(gameObject);
            if (onLevelChanged) onLevelChanged.Invoke(gameObject, newLevel);
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
                return progression[characterClass, stat].Calculate(_currentLevel);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                return 0;
            }
        }

        private int CalculateLevel()
        {
            if (!_hasExperience) return startingLevel;
            var level = startingLevel;
            _experienceToNextLevel = GetExperienceNeeded(level);
            while(_experienceToNextLevel < _experience.Value)
            {
                level++;
                _experienceToNextLevel = GetExperienceNeeded(level);
            }
            
            if (onExperienceMaxChanged) onExperienceMaxChanged.Invoke(gameObject, _experienceToNextLevel);

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
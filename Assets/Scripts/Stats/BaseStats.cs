using UnityEngine;

namespace RPGEngine.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField, Range(1, 99)] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression;

        private Experience _experience;
        private bool _hasExperience;

        private void Awake()
        {
            _experience = GetComponent<Experience>();
            _hasExperience = _experience;
        }

        public float GetStatValue(Stat stat)
        {
            try
            {
                return progression[characterClass, stat].Calculate(GetLevel());;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                return 0;
            }
        }

        public int GetLevel()
        {
            if (!_hasExperience) return startingLevel;
            int level = startingLevel;
            float experienceToNextLevel = GetExperienceNeeded(level);
            while(experienceToNextLevel < _experience.Value)
            {
                level++;
                experienceToNextLevel = GetExperienceNeeded(level);
            }

            _experience.ExperienceToNextLevel = experienceToNextLevel;

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
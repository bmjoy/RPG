using UnityEngine;

namespace RPGEngine.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField, Range(1, 99)] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression;

        public float GetHealth()
        {
            return progression.GetHealth(characterClass, startingLevel);
        }

        public float GetExperience()
        {
            return 10;
        }
    }
}
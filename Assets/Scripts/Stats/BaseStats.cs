using UnityEngine;

namespace RPGEngine.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField, Range(1, 99)] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression;

        public float GetStatValue(Stat stat)
        {
            try
            {
                return progression[characterClass, stat].Calculate(startingLevel);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
                return 0;
            }
        }
    }
}
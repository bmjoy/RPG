using UnityEngine;

namespace RPGEngine.Attributes
{
    public class Experience : MonoBehaviour
    {
        [SerializeField] private float value;

        public void GainExperience(float amount) => value += amount;
    }
}
// Health.cs
// 07-05-2022
// James LaFritz

using Unity.Mathematics;
using UnityEngine;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float max = 100f;
        private float m_value;

        private void Awake()
        {
            m_value = max;
        }


        public void TakeDamage(float damage)
        {
            m_value = math.max(m_value - damage, 0);
            if (m_value < 0)
            {
                m_value = 0;
            }

            Debug.Log($"{name} takes {damage} damage. Health is now {m_value}");
        }
    }
}
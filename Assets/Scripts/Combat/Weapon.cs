using UnityEngine;
using UnityEngine.Events;

namespace RPGEngine.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private UnityEvent onHit;
        
        public void OnHit()
        {
            onHit?.Invoke();
        }
    }
}
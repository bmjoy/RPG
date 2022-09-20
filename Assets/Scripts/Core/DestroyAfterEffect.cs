using UnityEngine;

namespace RPGEngine.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (_particleSystem && _particleSystem.IsAlive()) return;
            Destroy(gameObject);
        }
    }
}
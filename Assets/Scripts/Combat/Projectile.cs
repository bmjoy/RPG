using RPGEngine.Attributes;
using UnityEngine;

namespace RPGEngine.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private bool isHoming;
        [SerializeField] private float timeToLive = 6f;
        
        private Health _target;
        private float _damage;

        private void Start()
        {
            Destroy(gameObject, timeToLive);
        }

        private void Update()
        {
            if (isHoming) GetAimLocation();

            transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
        }

        private void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();
            if (health)
            {
                if (health.IsDead) return;
                _target.TakeDamage(_damage);
            }
            Destroy(gameObject);
        }

        public void SetTarget(Health target, float damage)
        {
            _target = target;
            _damage = damage;
            GetAimLocation();
        }

        private void GetAimLocation()
        {
            if (!_target || _target.IsDead) return;
            
            CapsuleCollider targetCapsuleCollider = _target.GetComponent<CapsuleCollider>();
            if (!targetCapsuleCollider) transform.LookAt(_target.transform.position);
            transform.LookAt(_target.transform.position + targetCapsuleCollider.center);
        }
    }
}
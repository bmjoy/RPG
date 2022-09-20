using RPGEngine.Attributes;
using UnityEngine;

namespace RPGEngine.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        
        private Health _target;
        private float _damage;

        private void Update()
        {
            if (!_target) return;
            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != _target) return;
            _target.TakeDamage(_damage);
            Destroy(gameObject);
        }

        public void SetTarget(Health target, float damage)
        {
            _target = target;
            _damage = damage;
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsuleCollider = _target.GetComponent<CapsuleCollider>();
            if (!targetCapsuleCollider) return _target.transform.position;
            return _target.transform.position + targetCapsuleCollider.center;
        }
    }
}
using RPGEngine.Attributes;
using UnityEngine;

namespace RPGEngine.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private bool isHoming;
        [SerializeField] private float timeToLive = 6f;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private float hitEffectTime = 4f;

        private Health _target;
        private float _damage;

        private void Start()
        {
            Destroy(gameObject, timeToLive);
        }

        private void Update()
        {
            if (isHoming) LookAtTarget();

            transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(tag)) return;
            Health health = other.GetComponent<Health>();
            if (health)
            {
                if (health.IsDead) return;
                health.TakeDamage(_damage);
            }

            if (hitEffect)
            {
                GameObject hit = Instantiate(hitEffect, GetAimLocation(other.transform), Quaternion.identity);
                Destroy(hit, hitEffectTime);
            }
            Destroy(gameObject);
        }

        public void SetTarget(Health target, float damage)
        {
            _target = target;
            _damage = damage;
            LookAtTarget();
        }

        private void LookAtTarget()
        {
            if (!_target || _target.IsDead) return;
            transform.LookAt(GetAimLocation(_target.transform));
        }

        private Vector3 GetAimLocation(Transform target)
        {
            if (!target) return target.position;
            CapsuleCollider targetCapsuleCollider = target.GetComponent<CapsuleCollider>();
            if (!targetCapsuleCollider) return target.transform.position;
            return target.transform.position + targetCapsuleCollider.center;
        }
    }
}
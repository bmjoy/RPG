using RPGEngine.Attributes;
using RPGEngine.Core;
using UnityEngine;

namespace RPGEngine.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private bool isHoming;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private GameObject[] destroyOnHit;
        [SerializeField] private float lifeAfterImpact = 1;

        private Health _target;
        private float _damage;
        private GameObjectFloatGameEvent _dealDamage;
        private BoolGameEvent _gamePausedEvent;
        private bool _gamePaused;

        private void OnEnable()
        {
            if (_gamePausedEvent) _gamePausedEvent.RegisterListener(OnPauseGame);
        }

        private void OnDisable()
        {
            
            if (_gamePausedEvent) _gamePausedEvent.UnregisterListener(OnPauseGame);
        }

        private void Update()
        {
            if (_gamePaused) return;
            if (isHoming) LookAtTarget();

            transform.Translate(Vector3.forward * (moveSpeed * Time.deltaTime));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(tag)) return;
            if (_dealDamage) _dealDamage.Invoke(other.gameObject, _damage);

            moveSpeed = 0f;

            if (hitEffect) Instantiate(hitEffect, GetAimLocation(other.transform), Quaternion.identity);

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }
            
            Destroy(gameObject, lifeAfterImpact);
        }

        public void SetTarget(Health target, GameObjectFloatGameEvent dealDamage, float damage,
            BoolGameEvent gamePausedEvent, bool isPaused)
        {
            _gamePausedEvent = gamePausedEvent;
            if (_gamePausedEvent) _gamePausedEvent.RegisterListener(OnPauseGame);
            _target = target;
            _dealDamage = dealDamage;
            _damage = damage;
            LookAtTarget();
            OnPauseGame(isPaused);
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

        private void OnPauseGame(bool paused)
        {
            _gamePaused = paused;
        }
    }
}
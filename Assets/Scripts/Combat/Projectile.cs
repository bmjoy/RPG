using RPGEngine.Attributes;
using UnityEngine;

namespace RPGEngine.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        
        private Health target;

        private void Update()
        {
            if (!target) return;
            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        public void SetTarget(Health target)
        {
            this.target = target;
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsuleCollider = target.GetComponent<CapsuleCollider>();
            if (!targetCapsuleCollider) return target.transform.position;
            return target.transform.position + targetCapsuleCollider.center;
        }
    }
}
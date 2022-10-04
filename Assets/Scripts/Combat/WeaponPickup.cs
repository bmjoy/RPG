using System.Collections;
using RPGEngine.Attributes;
using RPGEngine.Core;
using RPGEngine.Movement;
using UnityEngine;

namespace RPGEngine.Combat
{
    [RequireComponent(typeof(Collider))]
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        #region Inspector Fields

        [SerializeField] private WeaponConfig weaponConfig;
        [SerializeField] private float healthToRestore;
        [SerializeField] private float respawnTime = 5;

        #endregion

        #region Properties

        #endregion

        private Collider _collider;

        #region Unity Methods

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            Pickup(other.gameObject);
        }

        #endregion

        #region Implemtation IRaycastable

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        public bool HandleRaycast(MonoBehaviour callingBehavior, RaycastHit hit)
        {
            Mover mover = callingBehavior.GetComponent<Mover>();
            if (!mover) return false;
            if (Input.GetMouseButtonDown(0)) mover.StartMoveAction(hit.point);
            
            return true;
        }

        #endregion

        #region Private Methods

        private void Pickup(GameObject subject)
        {
            if (!subject) return;

            StartCoroutine(HideForSeconds(respawnTime));

            if (healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            
            if (!weaponConfig) return;
            Fighter fighter = subject.GetComponent<Fighter>();
            if (fighter == null) return;
            fighter.EquipWeapon(weaponConfig);
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowHidePickup(false);
            yield return new WaitForSeconds(seconds);
            ShowHidePickup(true);
        }

        private void ShowHidePickup(bool shouldShow)
        {
            _collider.enabled = shouldShow;
            
            foreach (Transform child in transform)
                child.gameObject.SetActive(shouldShow);
        }

        #endregion
    }
}
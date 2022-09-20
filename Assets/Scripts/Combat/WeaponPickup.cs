using System.Collections;
using UnityEngine;

namespace RPGEngine.Combat
{
    [RequireComponent(typeof(Collider))]
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private float respawnTime = 5;

        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            Fighter fighter = other.GetComponent<Fighter>();
            if (fighter == null) return;
            fighter.EquipWeapon(weapon);
            StartCoroutine(HideForSeconds(respawnTime));
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
    }
}
using UnityEngine;

namespace RPGEngine.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            Fighter fighter = other.GetComponent<Fighter>();
            if (fighter == null) return;
            fighter.EquipWeapon(weapon);
        }
    }
}
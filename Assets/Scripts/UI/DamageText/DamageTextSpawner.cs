using RPGEngine.Core;
using UnityEngine;

namespace RPGEngine.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText damageText;

        [SerializeField] private GameObjectFloatGameEvent receivedDamage;

        private GameObject _gameObject;

        private void Start()
        {
            _gameObject = gameObject.transform.parent.gameObject;
            if (!_gameObject) _gameObject = gameObject;
        }

        private void OnEnable()
        {
            if (receivedDamage) receivedDamage.RegisterListener(OnReceivedDamage);
        }

        private void OnReceivedDamage(GameObject toReceiveDamage, float amount)
        {
            if (_gameObject != toReceiveDamage) return;
            if (damageText)
            {
                Instantiate(damageText, transform).DamageAmount = amount;
            }
            
        }
    }
}
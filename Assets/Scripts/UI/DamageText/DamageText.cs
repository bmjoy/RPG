using TMPro;
using UnityEngine;

namespace RPGEngine.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        private TMP_Text _text;
        public float DamageAmount
        {
            set
            {
                if (_text) _text.text = $"{value:N0}";
            }
        }

        private void Awake()
        {
            _text = GetComponentInChildren<TMP_Text>();
        }
    }
}
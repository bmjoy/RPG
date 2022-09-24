using RPGEngine.Core;
using TMPro;
using UnityEngine;

namespace RPGEngine.UI.Stats
{
    public class InfoPanel : MonoBehaviour
    {
        [SerializeField] private GameObjectGameEvent targetChangeEvent;
        [SerializeField] private string targetTag = "Enemy";
        [SerializeField] private StatDisplay[] showObjects;
        [SerializeField] private TMP_Text titleText;

        private bool _hasTitleText;

        private void Awake()
        {
            _hasTitleText = titleText;
        }

        private void Start()
        {
            if (!targetChangeEvent && targetTag == "Player")
                OnTargetChanged(GameObject.Find("Player"));
        }

        private void OnEnable()
        {
            if (targetChangeEvent) targetChangeEvent.RegisterListener(OnTargetChanged);
        }

        private void OnDisable()
        {
            if (targetChangeEvent) targetChangeEvent.UnregisterListener(OnTargetChanged);
        }

        private void OnTargetChanged(GameObject target)
        {
            var shouldShow = target && target.CompareTag(targetTag);
            foreach (StatDisplay showObject in showObjects)
            {
                showObject.gameObject.SetActive(shouldShow);
                if (shouldShow) showObject.Target = target;
            }
            if (!_hasTitleText) return;
            titleText.text = shouldShow ? target.name : "Enemy";
            titleText.gameObject.SetActive(shouldShow);
        }
    }
}
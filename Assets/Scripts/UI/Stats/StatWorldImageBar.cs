using RPGEngine.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RPGEngine.UI.Stats
{
    public class StatWorldImageBar : MonoBehaviour
    {
        [SerializeField] protected GameObject target;
        [SerializeField] private GameObjectFloatGameEvent statChangedEvent;
        [SerializeField] private GameObjectFloatGameEvent statMaxValueChanged;
        [SerializeField] private Image statValueImage;
        [SerializeField] private Canvas rootCanvas;

        private bool _hasImage;
        private bool _hasRootCanvas;
        
        private float _value;
        private float _max;
        
        private void Awake()
        {
            _hasImage = statValueImage;
            _hasRootCanvas = rootCanvas;
            if (statChangedEvent) statChangedEvent.RegisterListener(OnStatValueChanged);
            if (statMaxValueChanged) statMaxValueChanged.RegisterListener(OnStatMaxValueChanged);
        }

        private void OnDestroy()
        {
            if (statChangedEvent) statChangedEvent.UnregisterListener(OnStatValueChanged);
            if (statMaxValueChanged) statMaxValueChanged.UnregisterListener(OnStatMaxValueChanged);
        }

        private void OnStatValueChanged(GameObject sender, float statValue)
        {
            //Debug.LogError($"{name}: {sender.name}-{(target != null ? target.name : "null")}");
            if (sender != target) return;
            _value = statValue;
            SetImageFill();
        }

        private void OnStatMaxValueChanged(GameObject sender, float statValue)
        {
            if (sender != target) return;
            _max = statValue;
            SetImageFill();
        }

        private void ShowHide(bool shouldShow)
        {
            if (_hasRootCanvas) rootCanvas.enabled = shouldShow;
        }

        private float GetFillPercentage()
        {
            return _max > 0.01f ? _value / _max : 0;
        }

        private void SetImageFill()
        {
            if (Mathf.Approximately(_value, 0) || Mathf.Approximately(_value, _max))
            {
                ShowHide(false);
                return;
            }
            ShowHide(true);
            if (_hasImage) statValueImage.fillAmount = GetFillPercentage();
        }
    }
}
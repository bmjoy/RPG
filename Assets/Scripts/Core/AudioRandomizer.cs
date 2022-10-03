using UnityEngine;

namespace RPGEngine.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioRandomizer : MonoBehaviour
    {
        [SerializeField] private GameObjectFloatGameEvent response;
        [SerializeField] private GameObject target;
        [SerializeField] private AudioClip[] clips;

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            if (response) response.RegisterListener(OnGameEvent);
        }

        private void OnGameEvent(GameObject arg1, float arg2)
        {
            if (clips == null || clips.Length < 1) return;
            if (arg1 != target) return;
            var index = Random.Range(0, clips.Length);
            _source.PlayOneShot(clips[index]);
        }
    }
}
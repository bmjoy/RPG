using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

        private void OnDisable()
        {
            if (response) response.UnregisterListener(OnGameEvent);
        }

        private void OnGameEvent(GameObject arg1, float arg2)
        {
            if (arg1 != target) return;

            Play();
        }

        public void Play()
        {
            if (clips == null || clips.Length < 1) return;
            var index = Random.Range(0, clips.Length);
            _source.PlayOneShot(clips[index]);
        }
    }
}
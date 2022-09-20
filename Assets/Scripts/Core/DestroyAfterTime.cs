using System;
using UnityEngine;

namespace RPGEngine.Core
{
    public class DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] private float timeToLive = 6f;

        private void Awake()
        {
            Destroy(gameObject, timeToLive);
        }
    }
}
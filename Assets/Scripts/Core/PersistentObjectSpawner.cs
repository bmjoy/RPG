// PeristentObjectSpawner.cs
// 07-13-2022
// James LaFritz

using UnityEngine;

namespace RPGEngine.Core
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// spawns a <a href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</a> Prefab that persists between scenes.
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject persistentObjectPrefab;

        private static bool _hasSpawned;

        private void Awake()
        {
            if (persistentObjectPrefab == null)
            {
                Debug.LogError("PersistentObjectSpawner: No persistent object prefab assigned.");
                return;
            }

            if (_hasSpawned) return;

            _hasSpawned = true;
            SpawnPersistentObjects();
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}
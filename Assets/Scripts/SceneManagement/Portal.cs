// Portal.cs
// 07-12-2022
// James LaFritz

using System.Collections;
using RPGEngine.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPGEngine.SceneManagement
{
    [System.Serializable]
    public struct DestinationPortal
    {
        [Scene] public int scene;
        [PortalIndex(Scene = "scene")] public int portal;
    }

    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// Teleports a player to another scene. You do not have to set the spawn point if the portal contains a Game Object with the name "SpawnPoint".
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Playables.PlayableDirector.html">UnityEngine.Playables.PlayableDirector</a>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    public class Portal : MonoBehaviour
    {
        [SerializeField] private int portalIndex;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private DestinationPortal destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;
        [SerializeField] private BoolGameEvent gamePausedEvent;

        public int Index
        {
            get => portalIndex;
            set => portalIndex = value;
        }

        #region Component References

        #region Required

        #endregion

        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            if (spawnPoint != null) return;
            spawnPoint = transform.Find("SpawnPoint");

            if (spawnPoint != null) return;
            Debug.LogError("Portals require a spawn point to spawn the player at.");
            gameObject.SetActive(false);
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerEnter.html"/>
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        #endregion

        private IEnumerator Transition()
        {
            if (destination.scene < 0)
            {
                Debug.LogWarning("Scene To Load is not Set");
                yield break;
            }

            if (SceneManager.sceneCountInBuildSettings < destination.scene)
            {
                Debug.LogWarning($"Build Settings does not contain a scene at {destination.scene}");
                yield break;
            }

            Fader fader = FindObjectOfType<Fader>();
            bool hasFader = fader;

            transform.parent = null;
            DontDestroyOnLoad(gameObject);

            // Remove Control
            if (gamePausedEvent) gamePausedEvent.Invoke(true);

            if (hasFader) yield return fader.FadeOut(fadeOutTime);

            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            bool hasSaving = savingWrapper;
            if (hasSaving) savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(destination.scene);

            // Remove Control
            if (gamePausedEvent) gamePausedEvent.Invoke(true);

            if (hasSaving) savingWrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            yield return new WaitForSeconds(fadeWaitTime);

            if (hasSaving) savingWrapper.Save();

            if (hasFader) fader.FadeIn(fadeInTime);

            // Restore Control
            if (gamePausedEvent) gamePausedEvent.Invoke(false);

            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            if (destination.portal == -1)
            {
                Debug.LogWarning("No Portal set.");
                return null;
            }

            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.portalIndex != destination.portal) continue; 
                return portal;
            }

            Debug.LogWarning($"Could not find a Portal with an index of {destination.portal} in scene {SceneManager.GetActiveScene().name}");
            return null;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            if (!otherPortal) return;

            GameObject player = GameObject.FindWithTag("Player");
            if (!player) return;
            NavMeshAgent playerNaveMesh = player.GetComponent<NavMeshAgent>();
            bool hasNavMesh = playerNaveMesh && playerNaveMesh.isActiveAndEnabled;

            if (hasNavMesh) playerNaveMesh.enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            if (hasNavMesh) playerNaveMesh.enabled = true;
        }
    }
}
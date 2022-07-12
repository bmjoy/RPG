// Portal.cs
// 07-12-2022
// James LaFritz

using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
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
        [SerializeField] private int portalIndex = 0;
        [SerializeField] private int sceneBuildIndex = -1;
        [SerializeField] private int spawnPortalIndex = -1;
        [SerializeField] private Transform spawnPoint;

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
            if (sceneBuildIndex < 0)
            {
                Debug.LogWarning("Scene To Load is not Set");
                yield break;
            }

            if (SceneManager.sceneCountInBuildSettings < sceneBuildIndex)
            {
                Debug.LogWarning($"Build Settings does not contain a scene at {sceneBuildIndex}");
                yield break;
            }

            DontDestroyOnLoad(gameObject);
            yield return SceneManager.LoadSceneAsync(sceneBuildIndex);
            yield return null;
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            Destroy(gameObject);
        }

        private Portal GetOtherPortal()
        {
            if (spawnPortalIndex == -1)
            {
                Debug.LogWarning("No Portal set.");
                return null;
            }

            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.portalIndex != spawnPortalIndex) continue; 
                return portal;
            }

            Debug.LogWarning($"Could not find a Portal with an index of {spawnPortalIndex} in scene {SceneManager.GetActiveScene().name}");
            return null;
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            if (otherPortal == null) return;

            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;
            NavMeshAgent playerNaveMesh = player.GetComponent<NavMeshAgent>();
            bool hasNavMesh = playerNaveMesh != null && playerNaveMesh.isActiveAndEnabled;

            if (hasNavMesh) playerNaveMesh.enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            if (hasNavMesh) playerNaveMesh.enabled = true;
        }
    }
}
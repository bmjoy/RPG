// FollowTarget.cs
// 07-03-2022
// James LaFritz

using UnityEngine;

namespace RPG.Core
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that follows a target.
    /// to move a game object to a targets position.
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    public class FollowTarget : MonoBehaviour
    {
        /// <value>The target to follow.</value>>
        [Tooltip("The target to follow.")] [SerializeField]
        private Transform target;

        #region Unity Methods

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            // ReSharper disable Unity.InefficientPropertyAccess
            if (target != null) return;
            Debug.LogWarning($"{gameObject.name} requires a(n) {nameof(target)} in order to work", gameObject);
            // Disable the GameObject.
            enabled = false;
            // ReSharper restore Unity.InefficientPropertyAccess
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            if (target == null) return;

            transform.position = target.position;
        }

        #endregion
    }
}
// FollowTarget.cs
// 07-03-2022
// James LaFritz

using UnityEngine;

namespace RPGEngine.Core
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that follows a target.
    /// to move a game object to a targets position.
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    public class FollowTarget : MonoBehaviour
    {
        #region Inspector Fields

        /// <value>The target to follow.</value>>
        [Tooltip("The target to follow.")] [SerializeField]
        private Transform target;

        #endregion

        #region Unity Messages

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
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html"/>
        /// </summary>
        private void LateUpdate()
        {
            if (!target) return;

            transform.position = target.position;
        }

        #endregion
    }
}
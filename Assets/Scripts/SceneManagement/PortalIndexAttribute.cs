// PortalIndexAttribute.cs
// 07-12-2022
// James LaFritz

using UnityEngine;

namespace RPGEngine.SceneManagement
{
    /// <summary>
    /// Displays a dropdown list of available portals (must be used with a 'integer' typed field).
    /// Must have an integer field for the index of the scene to select a portal from.
    /// <example>
    /// <code>
    /// [Scene] public int sceneInt;
    /// [PortalIndex(PortalIndex(scene = "sceneInt"))] public int portalIndex;
    /// </code>
    /// </example>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/PropertyAttribute.html"/>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class PortalIndexAttribute : PropertyAttribute
    {
        /// <summary>
        /// The name of the serialized Scene property to use.
        /// Must be an integer.
        /// </summary>
        public string Scene;
    }
}
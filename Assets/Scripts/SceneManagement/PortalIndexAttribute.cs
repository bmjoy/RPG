// PortalIndexAttribute.cs
// 07-12-2022
// James LaFritz

using UnityEngine;

namespace RPG.SceneManagement
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class PortalIndexAttribute : PropertyAttribute
    {
        /// <summary>
        /// The name of the serialized Scene property to use.
        /// </summary>
        public string scene;
    }
}
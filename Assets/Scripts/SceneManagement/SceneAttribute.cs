// SceneAttribute.cs
// 07-12-2022
// James LaFritz

using System;
using UnityEngine;

namespace RPG.SceneManagement
{
    /// <summary>
    /// Displays a dropdown list of available build settings Scenes (must be used with a 'string' or 'integer' typed field).
    /// <example>
    /// <code>
    /// [Scene] public string sceneString;
    /// [Scene] public int sceneInt;
    /// </code>
    /// </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SceneAttribute : PropertyAttribute { }
}
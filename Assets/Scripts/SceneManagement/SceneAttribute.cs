// SceneAttribute.cs
// 07-12-2022
// James LaFritz

using UnityEngine;

namespace RPGEngine.SceneManagement
{
    /// <summary>
    /// Displays a dropdown list of available build settings Scenes (must be used with a 'string' or 'integer' typed field).
    /// <example>
    /// <code>
    /// [Scene] public string sceneString;
    /// [Scene] public int sceneInt;
    /// </code>
    /// </example>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/PropertyAttribute.html"/>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class SceneAttribute : PropertyAttribute { }
}
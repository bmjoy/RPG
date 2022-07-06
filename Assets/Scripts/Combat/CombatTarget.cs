// CombatTarget.cs
// 07-03-2022
// James LaFritz

using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// a Player can target./// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<see cref="Health"/>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour { }
}
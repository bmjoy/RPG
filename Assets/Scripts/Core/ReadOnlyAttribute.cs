// ReadOnlyAttribute.cs
// 07-13-2022
// James LaFritz

using UnityEngine;

namespace RPG.Core
{
    /// <summary>
    /// Use this attribute to make a field appear as read only in the <i><b>Inspector</b></i>.
    /// <example>
    /// <code>[readOnly] public int healthPoints</code><para/>
    /// </example>
    /// <remarks>
    /// To make it so the field can not be change outside the <i><b>Class</b></i> make it a private field and add the <i>SerializeField</i> tag.
    /// <code>
    /// [readOnly, SerializeField] public int healthPoints;
    /// [readOnly] [SerializeField] public int healthPoints;
    /// </code>
    /// </remarks>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute { }
}
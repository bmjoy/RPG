// ShowIfBoolAttribute.cs
// 07-12-2022
// James LaFritz

using UnityEngine;

namespace RPGEngine.Core
{
    /// <summary>
    /// Show/Hide a field based on a bool value in the same script.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class ShowIfBoolAttribute : PropertyAttribute
    {
        /// <summary>
        /// The name of the serialized bool property to use.
        /// </summary>
        public readonly string BoolName;

        /// <summary>
        /// The value of the bool to show this property.
        /// </summary>
        public readonly bool Show;

        /// <summary>
        /// Show/Hide a field based on a bool value in the same script.
        /// </summary>
        /// <param name="boolName">The name of the serialized bool property to use.</param>
        /// <param name="show"> The value of the bool to show this property. Defaults to true</param>
        /// <example>
        /// <code>
        /// public bool showHideValue;
        ///
        /// // Shows this value if showHideValue = true;
        /// [ShowIfBool("showHideValue") public int showIfTrueInt;
        ///
        /// // Shows this value if showHideValue = false;
        /// [ShowIfBool("showHideValue", false) public int showIfFalseInt;
        /// </code>
        /// </example>
        public ShowIfBoolAttribute(string boolName, bool show = true)
        {
            BoolName = boolName;
            Show = show;
        }
    }
}
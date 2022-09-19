using Newtonsoft.Json;
using UnityEngine;

namespace RPGEngine.Saving
{
    /// <summary>
    /// A `System.Serializable` wrapper for the `Vector3` class.
    /// </summary>
    [System.Serializable]
    public struct SerializableVector3
    {
        [JsonProperty] private float _x;
        [JsonProperty] private float _y;
        [JsonProperty] private float _z;

        /// <summary>
        /// Create a new SerializableVector3 from a Vector3.
        /// </summary>
        /// <param name="vector">The Vector3 to create from.</param>
        public SerializableVector3(Vector3 vector)
        {
            _x = vector.x;
            _y = vector.y;
            _z = vector.z;
        }

        /// <summary>
        /// Create a Vector3 from this SerializableVector3's state.
        /// </summary>
        /// <returns>The Vector3 representation of this Serializable Vector3</returns>
        public Vector3 ToVector()
        {
            return new Vector3(_x, _y, _z);
        }
    }
}
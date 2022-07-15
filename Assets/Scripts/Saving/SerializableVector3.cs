using UnityEngine;

namespace RPGEngine.Saving
{
    /// <summary>
    /// A `System.Serializable` wrapper for the `Vector3` class.
    /// </summary>
    [System.Serializable]
    public struct SerializableVector3
    {
        private float m_x, m_y, m_z;

        /// <summary>
        /// Create a new SerializableVector3 from a Vector3.
        /// </summary>
        /// <param name="vector">The Vector3 to create from.</param>
        public SerializableVector3(Vector3 vector)
        {
            m_x = vector.x;
            m_y = vector.y;
            m_z = vector.z;
        }

        /// <summary>
        /// Create a Vector3 from this SerializableVector3's state.
        /// </summary>
        /// <returns>The Vector3 representation of this Serializable Vector3</returns>
        public Vector3 ToVector()
        {
            return new Vector3(m_x, m_y, m_z);
        }
    }
}
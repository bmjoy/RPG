using UnityEngine;

namespace RPGEngine.Core
{
    public interface IRaycastable
    {
        CursorType GetCursorType();
        public bool HandleRaycast(MonoBehaviour callingBehavior, RaycastHit hit);
    }
}
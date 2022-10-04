// PlayerController.cs
// 07-03-2022
// James LaFritz

using RPGEngine.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPGEngine.Control
{
    /// <summary>
    /// A <see cref="RPGController"/> that
    /// Controls a User Controlled Player in Game.
    /// </summary>
    public class PlayerController : RPGController
    {
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorMapping[] cursorMappings;

        #region Private Fields

        private readonly RaycastHit[] _objectHits = new RaycastHit[10];

        private CursorMapping _cursorMapping;
        
        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            if (GamePaused) return;
            if (InteractWithUI()) return;
            if (Health.IsDead)
            {
                SetCursor(CursorType.Dead);
                return;
            }
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        #endregion

        #region Private Methods

        private bool InteractWithUI()
        {
            if (!EventSystem.current.IsPointerOverGameObject()) return false;
            
            SetCursor(CursorType.UI);
            return true;
        }
        
        private bool InteractWithComponent()
        {
            var hits = Physics.RaycastNonAlloc(GetMouseFromMainCameraScreenPointToRay(), _objectHits);
            if (hits == 0) return false;
            SortObjectHits(hits);
            
            for (var i = 0; i < hits; i++)
            {
                RaycastHit hit = _objectHits[i];
                if (!Mover.CanMoveTo(hit.point)) continue;
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                if (raycastables == null) continue;
                
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (!raycastable.HandleRaycast(this, hit)) continue;
                    SetCursor(raycastable.GetCursorType());
                    return true;
                }
            }
            
            return false;
        }

        private void SortObjectHits(int hits)
        {
            float[] distance = new float[_objectHits.Length];
            for (int i = 0; i < distance.Length; i++)
            {
                distance[i] = float.MaxValue;
            }
            for (int i = 0; i < hits; i++)
            {
                distance[i] = _objectHits[i].distance;
            }
            System.Array.Sort(distance, _objectHits);
        }

        private bool InteractWithMovement()
        {
            return MoveToCursor();
        }

        private bool MoveToCursor()
        {
            var hasHIt = RaycastNavMesh(out Vector3 target);
            if (!Mover.CanMoveTo(target)) return false;
            if (hasHIt) SetCursor(CursorType.Movement);
            if (!hasHIt || !Input.GetMouseButton(0)) return hasHIt;

            Mover.StartMoveAction(target);

            return true;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = Vector3.zero;
            if(!Physics.Raycast(GetMouseFromMainCameraScreenPointToRay(), out RaycastHit hit)) return false;
            if (!NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, 1.0f, NavMesh.AllAreas))
                return false;

            target = navMeshHit.position;

            return true;
        }

        private Ray GetMouseFromMainCameraScreenPointToRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void SetCursor(CursorType type)
        {
            if (_cursorMapping.type == type) return;
            _cursorMapping = GetCursorMapping(type);
            Cursor.SetCursor(_cursorMapping.texture, _cursorMapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type) return mapping;
            }

            return cursorMappings[0];
        }

        #endregion
    }
}
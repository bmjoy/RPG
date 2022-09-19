// PatrolPath.cs
// 07-06-2022
// James LaFritz

using UnityEngine;

namespace RPGEngine.Control
{
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField] private bool loop;

        [Header("Debug Draw")]
        [SerializeField]
        private Color waypointColor = Color.gray;

        [SerializeField] private Color waypointStartColor = Color.green;
        [SerializeField] private Color waypointEndColor = Color.red;
        [SerializeField] private Color waypointPathColor = Color.gray;

        private int _currentWaypointIndex;
        private int _currentDirection = 1;

        private const float WaypointGizmoRadius = .3f;

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDrawGizmos.html"/>
        /// </summary>
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                if (i == 0)
                {
                    Gizmos.color = waypointStartColor;
                }
                else if (i == transform.childCount - 1)
                {
                    Gizmos.color = waypointEndColor;
                }
                else
                {
                    Gizmos.color = waypointColor;
                }

                Gizmos.DrawSphere(GetWaypoint(i), WaypointGizmoRadius);
                if (i == j) return;
                Gizmos.color = waypointPathColor;
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        #endregion

        public Vector3 GetWaypoint()
        {
            return GetWaypoint(_currentWaypointIndex);
        }

        public void NextIndex()
        {
            if (_currentDirection > 0)
            {
                _currentWaypointIndex = GetNextIndex(_currentWaypointIndex);
                if (!loop && _currentWaypointIndex == transform.childCount - 1)
                    ChangeDirection();
            }
            else
            {
                _currentWaypointIndex = GetPreviousIndex(_currentWaypointIndex);
                if (!loop && _currentWaypointIndex == 0)
                    ChangeDirection();
            }
        }

        private int GetNextIndex(int currentIndex)
        {
            if (currentIndex + 1 < transform.childCount) return currentIndex + 1;
            if (loop) return 0;
            return transform.childCount - 1;
        }

        private int GetPreviousIndex(int currentIndex)
        {
            if (currentIndex - 1 >= 0) return currentIndex - 1;
            if (loop) return transform.childCount - 1;
            return 0;
        }

        private Vector3 GetWaypoint(int index)
        {
            return transform.childCount < 1 ? Vector3.zero : transform.GetChild(index).position;
        }

        private void ChangeDirection()
        {
            _currentDirection *= -1;
        }
    }
}
using UnityEngine;

namespace DrivingData
{
    public enum CornerType
    {
        Straight = 1,
        ShortStraight = 2,
        TurnRight = 3,
        TurnLeft = 4
    }

    public class RaceCheckPoint : MonoBehaviour
    {
        [SerializeField] private CornerType cornerType;
        [SerializeField] private RaceCheckPoint nextCheckPoint;
        private Vector3 _cornerDirection;

        public CornerType GetCornerType => cornerType;
        public RaceCheckPoint NextCheckPoint => nextCheckPoint;
        public Vector3 GetCornerDir => _cornerDirection;

        private void Start()
        {
            if (nextCheckPoint)
            {
                _cornerDirection = (nextCheckPoint.transform.position - transform.position).normalized;
            }
            else
            {
                Debug.LogWarning("No next checkpoint has been assigned", this);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, _cornerDirection * 10);
        }
    }
}
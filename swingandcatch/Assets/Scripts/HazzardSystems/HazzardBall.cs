using System;
using System.Buffers;
using UnityEngine;

namespace TheGame.HazzardSystems
{
    [RequireComponent(typeof(HazzardMono))]
    public class HazzardBall : MonoBehaviour
    {
        public GameObject particlePrefab;
        [HideInInspector] public float speed = 4f;
        [HideInInspector] public Vector3 direction;
        [HideInInspector] public int obstacleLayerMask;
        
        public event Action<HazzardBall> onOutsideOfTheView;
        public event Action<HazzardBall> onHitObstacle;
        
        Vector3 previousPosition;
        Camera cam;

        void Awake()
        {
            cam = Camera.main;
        }

        void Update()
        {
            var pos = transform.position;
            previousPosition = pos;
            pos += direction * (speed * Time.deltaTime);
            var buffer = ArrayPool<Collider2D>.Shared.Rent(2);
            var diff = pos - previousPosition;
            int count = Physics2D.OverlapCircleNonAlloc(previousPosition + ((diff) * 0.5f), diff.magnitude, buffer, obstacleLayerMask);
            
            ArrayPool<Collider2D>.Shared.Return(buffer);

            if (count > 0)
            {
                onHitObstacle?.Invoke(this);
                return;
            }


            transform.position = pos;

            if (OutsideOfView())
            {
                onOutsideOfTheView?.Invoke(this);
                return;
            }
        }

        bool OutsideOfView()
        {
            const float DISTANCE_THRESHOLD = 1f;
            var viewportPoint = cam.WorldToViewportPoint(previousPosition);
            bool inside = viewportPoint.x > -DISTANCE_THRESHOLD && viewportPoint.x < 1f + DISTANCE_THRESHOLD && viewportPoint.y > -DISTANCE_THRESHOLD && viewportPoint.y < 1f + DISTANCE_THRESHOLD;
            return !inside;
        }
    }
}
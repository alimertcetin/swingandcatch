using System;
using TheGame.HazzardSystems;
using UnityEngine;

namespace TheGame.EnemySystems.SawBlade
{
    public class SawBladeProjectile : MonoBehaviour
    {
        public GameObject particlePrefab;
        public float speed = 4f;
        public float hitRadius = 0.3f;
     
        public Transform target { get; set; }
        public Vector3 direction { get; set; }

        public Action onOutsideOfTheView;

        Vector3 previousPosition;
        HazzardMono hazzardMono;
        
        void Update()
        {
            var pos = transform.position;
            previousPosition = pos;
            pos += direction * (speed * Time.deltaTime);
            transform.position = pos;

            if (OutsideOfView())
            {
                onOutsideOfTheView.Invoke();
                return;
            }
        }

        bool OutsideOfView()
        {
            const float DISTANCE_THRESHOLD = 1.5f;
            var viewportPoint = Camera.main.WorldToViewportPoint(previousPosition);
            bool inside = viewportPoint.x > -DISTANCE_THRESHOLD && viewportPoint.x < 1f + DISTANCE_THRESHOLD && viewportPoint.y > -DISTANCE_THRESHOLD && viewportPoint.y < 1f + DISTANCE_THRESHOLD;
            return !inside;
        }
    }
}
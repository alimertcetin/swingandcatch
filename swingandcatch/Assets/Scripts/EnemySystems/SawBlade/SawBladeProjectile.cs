using System;
using TheGame.HazzardSystems;
using UnityEngine;

namespace TheGame.EnemySystems.SawBlade
{
    [RequireComponent(typeof(HazzardMono))]
    public class SawBladeProjectile : MonoBehaviour
    {
        public GameObject particlePrefab;
        public float speed = 4f;
     
        public Transform target { get; set; }
        public Vector3 direction { get; set; }

        public Action<SawBladeProjectile> onOutsideOfTheView;

        Vector3 previousPosition;
        TrailRenderer trailRenderer;
        Camera cam;

        void Awake()
        {
            cam = Camera.main;
            trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        void OnEnable()
        {
            trailRenderer.Clear();
            trailRenderer.enabled = true;
        }

        void OnDisable()
        {
            trailRenderer.enabled = false;
        }

        void Update()
        {
            var pos = transform.position;
            previousPosition = pos;
            pos += direction * (speed * Time.deltaTime);
            transform.position = pos;

            if (OutsideOfView())
            {
                onOutsideOfTheView.Invoke(this);
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
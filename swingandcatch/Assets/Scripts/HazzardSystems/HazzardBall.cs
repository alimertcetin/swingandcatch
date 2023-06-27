﻿using System;
using System.Buffers;
using UnityEngine;

namespace TheGame.HazzardSystems
{
    [RequireComponent(typeof(HazzardMono))]
    public class HazzardBall : MonoBehaviour
    {
        [SerializeField] GameObject particlePrefab;
        [HideInInspector] public float speed = 4f;
        [HideInInspector] public Vector3 direction;
        
        public Action onOutsideOfTheView;
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
            var mask = (1 << PhysicsConstants.DefaultLayer) | (1 << PhysicsConstants.PlayerLayer) | (1 << PhysicsConstants.GroundLayer);
            int count = Physics2D.OverlapCircleNonAlloc(previousPosition + ((diff) * 0.5f), diff.magnitude, buffer, mask);

            if (count > 0)
            {
                var go = Instantiate(particlePrefab);
                go.transform.position = pos;
                onOutsideOfTheView.Invoke();
            }

            ArrayPool<Collider2D>.Shared.Return(buffer);

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
            var viewportPoint = cam.WorldToViewportPoint(previousPosition);
            bool inside = viewportPoint.x > -DISTANCE_THRESHOLD && viewportPoint.x < 1f + DISTANCE_THRESHOLD && viewportPoint.y > -DISTANCE_THRESHOLD && viewportPoint.y < 1f + DISTANCE_THRESHOLD;
            return !inside;
        }
    }
}
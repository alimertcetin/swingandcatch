using UnityEngine;
using XIV.Core;
using XIV.Core.Utils;

namespace TheGame.AnimationSystems
{
    public class MoveLocalAnimationMono : AnimationMonoBase
    {
        public Vector3 axis;
        public float moveDistance;
        public float moveSpeed;
        public EasingFunction.Ease ease;
        public EasingFunction.Function easingFunc;
        
        Vector3 movementStart;
        Vector3 movementEnd;
        Timer timer;

        void Start()
        {
            axis.Normalize();
            var localPosition = transform.localPosition;
            movementStart = localPosition;
            movementEnd = localPosition + axis * moveDistance;
            var time = Vector3.Distance(movementStart, movementEnd) / moveSpeed;
            timer = new Timer(time);
            easingFunc = EasingFunction.GetEasingFunction(ease);
        }

        // Update is called once per frame
        void Update()
        {
            timer.Update(Time.deltaTime);
            transform.localPosition = Vector3.Lerp(movementStart, movementEnd, easingFunc.Invoke(0f, 1f, timer.NormalizedTime));
            if (timer.IsDone)
            {
                timer.Restart();
                (movementStart, movementEnd) = (movementEnd, movementStart);
            }
        }

#if UNITY_EDITOR
        bool isCached;
        Vector3 cachedPos;

        void OnEnable()
        {
            isCached = true;
            cachedPos = transform.position;
        }

        void OnDisable()
        {
            isCached = false;
        }

        void OnDrawGizmosSelected()
        {
            Vector3 position = (isCached ? cachedPos : transform.position);
            var axisNormalized = transform.TransformDirection(axis.normalized);
            var movementStart = position;
            var movementEnd = position + axisNormalized * moveDistance;
            
            XIVDebug.DrawLine(movementStart, movementEnd);
            XIVDebug.DrawCircle(movementStart, 0.25f, Vector3.forward, Color.green, 5);
            XIVDebug.DrawCircle(movementEnd, 0.25f, Vector3.forward, Color.red, 5);
        }
        
#endif
    }
}
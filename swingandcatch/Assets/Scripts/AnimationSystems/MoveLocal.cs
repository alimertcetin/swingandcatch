using UnityEngine;
using XIV.Core;
using XIV.Core.Utils;

namespace TheGame.AnimationSystems
{
    public class MoveLocal : MonoBehaviour
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
            var moveDistanceHalf = moveDistance * 0.5f;
            movementStart = localPosition + axis * moveDistanceHalf;
            movementEnd = localPosition - axis * moveDistanceHalf;
            transform.localPosition = movementStart;
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
        Vector3 cachedWorldPos;

        void OnEnable()
        {
            isCached = true;
            cachedWorldPos = transform.position;
        }

        void OnDisable()
        {
            isCached = false;
        }

        void OnDrawGizmosSelected()
        {
            Vector3 position = isCached ? cachedWorldPos : transform.position;
            var moveDistanceHalf = moveDistance * 0.5f;
            var axisNormalized = axis.normalized;
            var movementStart = position + axisNormalized * moveDistanceHalf;
            var movementEnd = position - axisNormalized * moveDistanceHalf;
            
            XIVDebug.DrawLine(movementStart, movementEnd);
            XIVDebug.DrawCircle(movementStart, 0.25f, Vector3.forward, Color.green, 5);
            XIVDebug.DrawCircle(movementEnd, 0.25f, Vector3.forward, Color.red, 5);
        }
        
#endif
    }
}
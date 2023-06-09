using UnityEngine;

namespace TheGame.Animations
{
    public class RotateLocal : MonoBehaviour
    {
        public Vector3 axis;
        public float speed;

        void Start()
        {
            axis.Normalize();
        }

        // Update is called once per frame
        void Update()
        {
            transform.rotation *= Quaternion.AngleAxis(speed * Time.deltaTime, axis);
        }

        void OnDrawGizmosSelected()
        {
            var pos = transform.position;
            Debug.DrawLine(pos, pos + axis.normalized * 2f, Color.yellow);
        }
    }
}

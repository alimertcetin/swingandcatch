using TheGame.FSM;
using TheGame.PlayerSystems.States;
using TheGame.VerletRope;
using UnityEngine;
using XIV.Core;
using XIV.Core.Extensions;

namespace TheGame.PlayerSystems
{
    public class PlayerFSM : StateMachine
    {
        public float walkSpeed = 5f;
        public float runSpeed = 10f;
        public float airMovementSpeed = 7.5f;
        public float jumpHeight = 2f;
        public float jumpGravityScale = 0.5f;
        public float groundCheckDistance = 0.5f;
        public float fallGravityScale = 0.5f;
        public float climbCheckRadius = 2f;
        public float climbSpeed = 5f;

        // TODO : Fix naming. Like hasMovementInput -> hasMovementInputX
        public bool hasMovementInput => Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0f;
        public Vector3 movementInput => new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
        public bool isRunPressed => Input.GetKey(KeyCode.LeftShift);
        public bool isJumpPressed => Input.GetKey(KeyCode.Space);
        
        public Rigidbody2D rb { get; private set; }
        public Vector3 velocity { get; private set; }

        Vector3 previousPosition;
        Collider2D[] colliderBuffer = new Collider2D[2];

        protected override void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            previousPosition = transform.position;
            base.Awake();
        }

        protected override void Update()
        {
            var position = transform.position;
            velocity = position - previousPosition;
            previousPosition = position;
            base.Update();
        }

        protected override State GetInitialState()
        {
            return new PlayerStateFactory(this).GetState<PlayerGroundedState>();
        }

        public bool IsGrounded()
        {
            var t = transform;
            var pos = t.position + Vector3.down * (t.localScale.y * 0.5f);
#if UNITY_EDITOR
            XIV.Core.XIVDebug.DrawLine(pos, pos + Vector3.down * groundCheckDistance, Color.yellow);
#endif
            return Physics.Raycast(pos, Vector3.down, groundCheckDistance, 1 << PhysicsConstants.GroundLayer);
        }

        public bool GetNearestRope(out Rope rope)
        {
            rope = default;
            var position = transform.position;
            int count = Physics2D.OverlapCircleNonAlloc(position, climbCheckRadius, colliderBuffer, 1 << PhysicsConstants.RopeLayer);
            XIVDebug.DrawCircle(position, climbCheckRadius, Color.blue, 0.5f);
            if (count == 0) return false;
            rope = colliderBuffer.GetClosest(position, count).GetComponent<Rope>();
            return true;
        }
    }
}
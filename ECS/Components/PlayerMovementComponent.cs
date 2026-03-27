namespace Sober.ECS.Components
{
    public struct PlayerMovementComponent
    {
        public float MoveSpeed;
        public float JumpForce;
        public bool IsGrounded;

        public PlayerMovementComponent(float moveSpeed, float jumpForce)
        {
            MoveSpeed = moveSpeed;
            JumpForce = jumpForce;
            IsGrounded = false;
        }
    }
}
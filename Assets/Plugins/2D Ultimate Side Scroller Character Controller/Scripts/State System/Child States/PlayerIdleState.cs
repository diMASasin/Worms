using UnityEngine;

namespace UltimateCC
{
    public class PlayerIdleState : MainState
    {
        public PlayerIdleState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            rigidbody2D.gravityScale = playerData.Walk.Physics2DGravityScale;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (playerData.Physics.Contacts.Count == 0 || (playerData.Physics.IsNextToWall && !playerData.Physics.Slope.StayStill))
            {
                rigidbody2D.velocity = new(0f, -1f);
            }
            else if (playerData.Physics.IsMultipleContactWithNonWalkableSlope)
            {
                rigidbody2D.velocity = (playerData.Physics.ContactPosition.RotateAround(playerData.Physics.GroundCheckPosition, playerData.Physics.FacingDirection * 5) - playerData.Physics.GroundCheckPosition) * 1;
            }
            else if (playerData.Physics.CanSlideCorner)
            {
                rigidbody2D.velocity = new(0f, -playerData.Physics.SlideSpeedOnCorner);
            }
            else
            {
                rigidbody2D.velocity = Vector2.zero;
            }
            rigidbody2D.velocity += playerData.Physics.Platform.DampedVelocity;
            playerData.Walls.CurrentStamina = Mathf.Clamp(playerData.Walls.CurrentStamina + (Time.fixedDeltaTime * playerData.Walls.StaminaRegenPerSec), 0, playerData.Walls.MaxStamina);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void PhysicsCheck()
        {
            base.PhysicsCheck();
        }

        public override void SwitchStateLogic()
        {
            base.SwitchStateLogic();

            if (inputManager.Input_Walk != 0 && !playerData.Physics.Slope.StayStill)
            {
                stateMachine.ChangeState(player.WalkState);
            }
            else if (inputManager.Input_Jump && playerData.Physics.CanJump)
            {
                stateMachine.ChangeState(player.JumpState);
            }
            else if (!playerData.Physics.IsGrounded || (playerData.Physics.IsOnNotWalkableSlope && !playerData.Physics.Slope.StayStill && !playerData.Physics.IsMultipleContactWithNonWalkableSlope))
            {
                stateMachine.ChangeState(player.LandState);
            }
            else if (inputManager.Input_Dash && playerData.Dash.DashCooldownTimer <= 0f)
            {
                stateMachine.ChangeState(player.DashState);
            }
            else if (inputManager.Input_Crouch)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
            else if (playerData.Physics.IsNextToWall && inputManager.Input_WallGrab && playerData.Walls.CurrentStamina > 0)
            {
                stateMachine.ChangeState(player.WallGrabState);
            }
        }
    }
}

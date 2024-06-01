using UnityEngine;

namespace UltimateCC
{
    public class PlayerCrouchIdleState : MainState
    {
        public PlayerCrouchIdleState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            rigidbody2D.gravityScale = playerData.Crouch.Physics2DGravityScale;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (playerData.Physics.Contacts.Count == 0)
            {
                rigidbody2D.velocity = new(0f, -1f);
            }
            else if (playerData.Physics.IsMultipleContactWithNonWalkableSlope)
            {
                rigidbody2D.velocity = new(0f, (playerData.Physics.Slope.CurrentSlopeAngle / 90) - 1);
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

            if (!inputManager.Input_Crouch)
            {
                if (inputManager.Input_Walk == 0f)
                {
                    stateMachine.ChangeState(player.IdleState);
                }
                else if (inputManager.Input_Walk != 0)
                {
                    stateMachine.ChangeState(player.WalkState);
                }

            }
            else if (inputManager.Input_Walk != 0 && !playerData.Physics.Slope.StayStill)
            {
                stateMachine.ChangeState(player.CrouchWalkState);
            }
            else if (!playerData.Physics.IsGrounded || (playerData.Physics.IsOnNotWalkableSlope && !playerData.Physics.Slope.StayStill && !playerData.Physics.IsMultipleContactWithNonWalkableSlope))
            {
                stateMachine.ChangeState(player.LandState);
            }
            else if (inputManager.Input_Dash && playerData.Dash.DashCooldownTimer <= 0f)
            {
                stateMachine.ChangeState(player.DashState);
            }
            else if (playerData.Physics.IsNextToWall && inputManager.Input_WallGrab && playerData.Walls.CurrentStamina > 0)
            {
                stateMachine.ChangeState(player.WallGrabState);
            }
        }
    }
}

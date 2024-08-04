using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Base_States;
using UnityEngine;

namespace _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Child_States
{
    public class PlayerIdleState : MainState
    {
        public PlayerIdleState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum,
            PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
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

            if (playerData.Physics.Attacked == true)
            {
                playerData.Physics.Attacked = false;
                stateMachine.ChangeState(player.AttackedState);
            }
            else if (playerData.Physics.Contacts.Count == 0 ||
                (playerData.Physics.IsNextToWall && !playerData.Physics.Slope.StayStill))
            {
                rigidbody2D.velocity += new Vector2(0f, -9.8f * Time.fixedDeltaTime);
            }
            // else if (playerData.Physics.IsMultipleContactWithNonWalkableSlope)
            // {
            //     rigidbody2D.velocity = (playerData.Physics.ContactPosition.RotateAround(playerData.Physics.GroundCheckPosition, playerData.Physics.FacingDirection * 5) - playerData.Physics.GroundCheckPosition) * 1;
            // }
            // else if (playerData.Physics.CanSlideCorner)
            // {
            //     rigidbody2D.velocity = new(0f, -playerData.Physics.SlideSpeedOnCorner);
            // }
            else if (rigidbody2D.velocity.magnitude < 0.3f && playerData.Physics.IsGrounded &&
                     rigidbody2D.gameObject.layer != 6)
            {
                player.Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            playerData.Walls.CurrentStamina =
                Mathf.Clamp(
                    playerData.Walls.CurrentStamina + (Time.fixedDeltaTime * playerData.Walls.StaminaRegenPerSec), 0,
                    playerData.Walls.MaxStamina);
            rigidbody2D.velocity += playerData.Physics.Platform.DampedVelocity;
        }

        public override void Exit()
        {
            base.Exit();
            player.Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public override void PhysicsCheck()
        {
            base.PhysicsCheck();
        }

        public override void SwitchStateLogic()
        {
            base.SwitchStateLogic();

            if (InputHandler.Input_Walk != 0 && !playerData.Physics.Slope.StayStill)
            {
                stateMachine.ChangeState(player.WalkState);
            }
            else if (InputHandler.Input_Jump && playerData.Physics.CanJump)
            {
                stateMachine.ChangeState(player.JumpState);
            }
            // else if (!playerData.Physics.IsGrounded || (playerData.Physics.IsOnNotWalkableSlope && !playerData.Physics.Slope.StayStill && !playerData.Physics.IsMultipleContactWithNonWalkableSlope))
            // {
            //     stateMachine.ChangeState(player.LandState);
            // }
            else if (!playerData.Physics.IsGrounded)
            {
                stateMachine.ChangeState(player.FreeFallingState);
            }
            else if (InputHandler.Input_Dash && playerData.Dash.DashCooldownTimer <= 0f)
            {
                stateMachine.ChangeState(player.DashState);
            }
            else if (InputHandler.Input_Crouch)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
            else if (playerData.Physics.IsNextToWall && InputHandler.Input_WallGrab &&
                     playerData.Walls.CurrentStamina > 0)
            {
                stateMachine.ChangeState(player.WallGrabState);
            }
        }
    }
}
using UnityEngine;

namespace UltimateCC
{
    public class PlayerDashState : MainState, IMove2D
    {
        public PlayerDashState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            rigidbody2D.gravityScale = playerData.Dash.Physics2DGravityScale;
            playerData.Dash.DashCooldownTimer = playerData.Dash.DashCooldown;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            Move2D();
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
            if (localTime > playerData.Dash.DashTime)
            {
                if (inputManager.Input_Walk != 0 && playerData.Physics.IsGrounded && !playerData.Physics.IsOnNotWalkableSlope)
                {
                    stateMachine.ChangeState(player.WalkState);
                }
                else if (rigidbody2D.velocity.y == 0 && inputManager.Input_Walk == 0 && playerData.Physics.IsGrounded && (!playerData.Physics.IsOnNotWalkableSlope))
                {
                    stateMachine.ChangeState(player.IdleState);
                }
                else if (playerData.Physics.CanJump && inputManager.Input_Jump)
                {
                    stateMachine.ChangeState(player.JumpState);
                }
                else if (playerData.Physics.IsNextToWall && inputManager.Input_WallGrab)
                {
                    stateMachine.ChangeState(player.WallGrabState);
                }
                else if (playerData.Physics.IsNextToWall && !inputManager.Input_WallGrab)
                {
                    stateMachine.ChangeState(player.WallSlideState);
                }
                else
                {
                    stateMachine.ChangeState(player.LandState);
                }
            }
        }

        public void Move2D()
        {
            Vector2 XVelocity = Vector2.zero;
            XVelocity.x = playerData.Dash.DashXVelocityCurve.Evaluate(localTime / playerData.Dash.DashTime);
            XVelocity.x *= playerData.Dash.MaxSpeed * playerData.Physics.FacingDirection;
            XVelocity.y = playerData.Dash.DashYVelocityCurve.Evaluate(localTime / playerData.Dash.DashTime);
            XVelocity.y *= playerData.Dash.MaxHeight;

            rigidbody2D.velocity = XVelocity;
        }
    }
}

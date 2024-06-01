using Plugins._2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Essentials;
using UnityEngine;

namespace UltimateCC
{
    public class PlayerWallJumpState : MainState, IMove2D
    {
        float xCurveTime;
        enum Phase { SpeedUp, SlowDown, Null }
        Phase phase = Phase.Null;
        float cutJumpTime;
        Vector2 xStartVelocity;
        float xStartVelocityTime;
        public PlayerWallJumpState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            player.Rigidbody2D.velocity = new Vector2(player.Rigidbody2D.velocity.x, playerData.Walls.WallJump.MaxHeight);
            playerData.Jump.JumpBufferTimer = 0;
            rigidbody2D.gravityScale = playerData.Walls.Physics2DGravityScale;
            xCurveTime = 0f;
            cutJumpTime = 0f;
            phase = Phase.Null;
            playerData.Physics.CutJump = false;
            xStartVelocity = new(playerData.Walls.WallJump.XStartVelocity * (playerData.Physics.WallDirection != 0 ? -playerData.Physics.WallDirection : playerData.Physics.FacingDirection), 0);
            xStartVelocityTime = 0f;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (cutJumpTime == 0 && playerData.Physics.CutJump)
            {
                cutJumpTime = localTime;
                localTime = cutJumpTime * (cutJumpTime + (playerData.Walls.WallJump.JumpTime - cutJumpTime) * playerData.Walls.WallJump.TimeScaleOnCut) / playerData.Walls.WallJump.JumpTime;
            }
            Move2D();
            xCurveTime += Time.fixedDeltaTime;
            xStartVelocityTime += Time.fixedDeltaTime;
        }

        public override void Exit()
        {
            base.Exit();
            playerData.Physics.CutJump = false;
        }

        public override void PhysicsCheck()
        {
            base.PhysicsCheck();
        }

        public override void SwitchStateLogic()
        {
            base.SwitchStateLogic();

            if (localTime >= playerData.Walls.WallJump.JumpTime || (playerData.Physics.IsOnHeadBump && playerData.Physics.CanBumpHead))
            {
                stateMachine.ChangeState(player.LandState);
            }
            else if (inputManager.Input_Dash && playerData.Dash.DashCooldownTimer <= 0f)
            {
                stateMachine.ChangeState(player.DashState);
            }
        }

        public void Move2D()
        {
            Vector2 _newVelocity = Vector2.zero;
            if (localTime <= playerData.Walls.WallJump.JumpTime && !playerData.Physics.CutJump)
            {
                _newVelocity.y = playerData.Walls.WallJump.JumpVelocityCurve.Evaluate(localTime / playerData.Walls.WallJump.JumpTime);
                _newVelocity.y *= playerData.Walls.WallJump.MaxHeight * 1 / playerData.Walls.WallJump.JumpTime;
            }
            else if (localTime <= cutJumpTime + ((playerData.Walls.WallJump.JumpTime - cutJumpTime) * playerData.Walls.WallJump.TimeScaleOnCut) && playerData.Physics.CutJump)
            {
                _newVelocity.y = playerData.Walls.WallJump.JumpVelocityCurve.Evaluate(localTime / (cutJumpTime + ((playerData.Walls.WallJump.JumpTime - cutJumpTime) * playerData.Walls.WallJump.TimeScaleOnCut)));
                _newVelocity.y *= playerData.Walls.WallJump.MaxHeight * 1 / playerData.Walls.WallJump.JumpTime;
                _newVelocity.y *= 1 - playerData.Walls.WallJump.JumpCutPower;
            }
            else
            {
                stateMachine.ChangeState(player.LandState);
            }
            if (!playerData.Physics.IsOnNotWalkableSlope)
            {
                _newVelocity.x = VelocityOnx();
            }
            else
            {
                _newVelocity.x = VelocityOnx() * playerData.Physics.Slope.CurrentSlopeAngle / 100;
            }
            if (localTime < playerData.Walls.WallJump.DampingTime)
            {
                _newVelocity.x = Mathf.Sign(_newVelocity.x) != Mathf.Sign(xStartVelocity.x) ? _newVelocity.x * playerData.Walls.WallJump.OppositeSpeedMultiplierWhenDamping : _newVelocity.x;
                rigidbody2D.velocity = _newVelocity + xStartVelocity;
            }

        }

        private float VelocityOnx()
        {
            float XVelocity;
            if (inputManager.Input_Walk != 0)
            {
                if (phase != Phase.SpeedUp)
                {
                    xCurveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walls.WallJump.XSpeedUpCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Walls.WallJump.XMaxSpeed, 1, true);
                    xCurveTime *= playerData.Walls.WallJump.XSpeedUpTime;
                    phase = Phase.SpeedUp;
                }
                if (xCurveTime < playerData.Walls.WallJump.XSpeedUpTime)
                {
                    XVelocity = playerData.Walls.WallJump.XSpeedUpCurve.Evaluate(xCurveTime / playerData.Walls.WallJump.XSpeedUpTime);
                }
                else
                {
                    XVelocity = playerData.Walls.WallJump.XSpeedUpCurve.Evaluate(1f);
                }
                XVelocity *= inputManager.Input_Walk * playerData.Walls.WallJump.XMaxSpeed;
            }
            else
            {
                if (phase != Phase.SlowDown)
                {
                    xCurveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walls.WallJump.XSlowDownCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Walls.WallJump.XMaxSpeed, 1, false);
                    xCurveTime *= playerData.Walls.WallJump.XSlowDownTime;
                    phase = Phase.SlowDown;
                }
                if (xCurveTime < playerData.Walls.WallJump.XSlowDownTime)
                {
                    XVelocity = playerData.Walls.WallJump.XSlowDownCurve.Evaluate(xCurveTime / playerData.Walls.WallJump.XSlowDownTime);
                }
                else
                {
                    XVelocity = playerData.Walls.WallJump.XSlowDownCurve.Evaluate(1f);
                }
                XVelocity *= playerData.Walls.WallJump.XMaxSpeed * playerData.Physics.FacingDirection;
            }
            xStartVelocity.x = playerData.Walls.WallJump.XStartVelocityDampingCurve.Evaluate(xStartVelocityTime / playerData.Walls.WallJump.DampingTime) * Mathf.Sign(xStartVelocity.x);
            xStartVelocity.x *= playerData.Walls.WallJump.XStartVelocity;
            return XVelocity;
        }
    }
}

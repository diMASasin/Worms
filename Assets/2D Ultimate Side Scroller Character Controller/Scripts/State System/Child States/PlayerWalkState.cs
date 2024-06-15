using Plugins._2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Essentials;
using UnityEngine;

namespace UltimateCC
{
    public class PlayerWalkState : MainState, IMove1D
    {
        enum Phase { SpeedUp, SlowDown, TurnBack }
        Phase phase;
        private float curveTime;
        private float localXVelovity;
        private int turnBackStartDirection;
        public PlayerWalkState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            rigidbody2D.gravityScale = playerData.Walk.Physics2DGravityScale;
            localTime = 0f;
            curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walk.SpeedUpCurve, Mathf.Abs(rigidbody2D.velocity.x) / playerData.Walk.MaxSpeed, 1, true);
            curveTime *= playerData.Walk.SpeedUpTime;
            localXVelovity = 0f;
            phase = Phase.SpeedUp;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Move1D();
            rigidbody2D.velocity += playerData.Physics.Platform.DampedVelocity;
            curveTime += Time.fixedDeltaTime;
            playerData.Walls.CurrentStamina = Mathf.Clamp(playerData.Walls.CurrentStamina + (Time.fixedDeltaTime * playerData.Walls.StaminaRegenPerSec), 0, playerData.Walls.MaxStamina);
        }

        public override void Exit()
        {
            base.Exit();
            rigidbody2D.velocity = Vector2.zero;
        }

        public override void PhysicsCheck()
        {
            base.PhysicsCheck();
        }

        public override void SwitchStateLogic()
        {
            base.SwitchStateLogic();
            if ((phase == Phase.SlowDown && curveTime > playerData.Walk.SlowDownTime) || playerData.Physics.Slope.StayStill)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else if (InputHandler.Input_Jump && playerData.Physics.CanJump)
            {
                stateMachine.ChangeState(player.JumpState);
            }
            else if (!playerData.Physics.IsGrounded || (playerData.Physics.IsOnNotWalkableSlope && !playerData.Physics.Slope.StayStill && !playerData.Physics.IsMultipleContactWithNonWalkableSlope))
            {
                stateMachine.ChangeState(player.LandState);
            }
            else if (InputHandler.Input_Dash && playerData.Dash.DashCooldownTimer <= 0f)
            {
                stateMachine.ChangeState(player.DashState);
            }
            else if (InputHandler.Input_Crouch)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
            else if (playerData.Physics.IsNextToWall && InputHandler.Input_WallGrab && playerData.Walls.CurrentStamina > 0)
            {
                stateMachine.ChangeState(player.WallGrabState);
            }
        }

        public void Move1D()
        {
            float newVelocity;
            if (playerData.Physics.IsOnNotWalkableSlope && Mathf.Sign(playerData.Physics.ContactPosition.x - player.transform.position.x) == Mathf.Sign(playerData.Physics.FacingDirection)
                && playerData.Physics.Slope.CurrentSlopeAngle > playerData.Physics.Slope.MaxSlopeAngle)
            {
                // rigidbody2D.velocity = Vector2.zero;
            }
            else if (!playerData.Physics.IsOnNotWalkableSlope)
            {
                newVelocity = VelocityOnX();
                rigidbody2D.velocity = -1 * newVelocity * playerData.Physics.WalkSpeedDirection.normalized;
            }
            else
            {
                newVelocity = VelocityOnX();
                rigidbody2D.velocity = new Vector2(newVelocity, rigidbody2D.velocity.y);
            }
            localXVelovity = rigidbody2D.velocity.x;
        }
        private float VelocityOnX()
        {
            float XVelocity;
            if (InputHandler.Input_Walk != 0 && (localXVelovity == 0 || Mathf.Sign(InputHandler.Input_Walk) == Mathf.Sign(localXVelovity))
                && (phase != Phase.TurnBack || curveTime > playerData.Walk.TurnBackTime))
            {
                if (phase != Phase.SpeedUp)
                {
                    curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walk.SpeedUpCurve, Mathf.Abs(localXVelovity) / playerData.Walk.MaxSpeed, 1f, true);
                    curveTime *= playerData.Walk.SpeedUpTime;
                    phase = Phase.SpeedUp;
                }
                if (curveTime < playerData.Walk.SpeedUpTime)
                {
                    XVelocity = playerData.Walk.SpeedUpCurve.Evaluate(curveTime / playerData.Walk.SpeedUpTime);
                }
                else
                {
                    XVelocity = playerData.Walk.SpeedUpCurve.Evaluate(1f);
                }
                XVelocity *= InputHandler.Input_Walk * playerData.Walk.MaxSpeed;
            }
            else if (InputHandler.Input_Walk != 0 && ((localXVelovity != 0 && Mathf.Sign(InputHandler.Input_Walk) != Mathf.Sign(localXVelovity)) || phase == Phase.TurnBack))
            {
                if (phase != Phase.TurnBack)
                {
                    curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walk.TurnBackCurve, Mathf.Abs(localXVelovity) / playerData.Walk.MaxSpeed, 2f, false);
                    curveTime *= playerData.Walk.TurnBackTime;
                    phase = Phase.TurnBack;
                    turnBackStartDirection = (int)Mathf.Sign(localXVelovity);
                }
                if (curveTime < playerData.Walk.TurnBackTime)
                {
                    XVelocity = playerData.Walk.TurnBackCurve.Evaluate(curveTime / playerData.Walk.TurnBackTime);
                }
                else
                {
                    XVelocity = playerData.Walk.TurnBackCurve.Evaluate(2f);
                }
                XVelocity *= playerData.Walk.MaxSpeed * turnBackStartDirection;
            }
            else
            {
                if (phase != Phase.SlowDown)
                {
                    curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walk.SlowDownCurve, Mathf.Abs(localXVelovity) / playerData.Walk.MaxSpeed, 1f, false);
                    curveTime *= playerData.Walk.SlowDownTime;
                    phase = Phase.SlowDown;
                }
                if (curveTime < playerData.Walk.SlowDownTime)
                {
                    XVelocity = playerData.Walk.SlowDownCurve.Evaluate(curveTime / playerData.Walk.SlowDownTime);
                }
                else
                {
                    XVelocity = playerData.Walk.SlowDownCurve.Evaluate(1f);
                }
                XVelocity *= playerData.Physics.FacingDirection * playerData.Walk.MaxSpeed;
            }

            if (playerData.Physics.Contacts.Count == 0)
            {
                rigidbody2D.gravityScale = playerData.Land.MinLandSpeed / Physics2D.gravity.y;
            }
            else
            {
                rigidbody2D.gravityScale = playerData.Walk.Physics2DGravityScale;
            }
            return XVelocity;
        }
    }
}

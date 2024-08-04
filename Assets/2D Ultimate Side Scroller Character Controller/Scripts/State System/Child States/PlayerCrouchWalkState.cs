using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Essentials;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Base_States;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Child_States.Interfaces;
using UnityEngine;

namespace _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Child_States
{
    public class PlayerCrouchWalkState : MainState, IMove1D
    {
        enum Phase { SpeedUp, SlowDown, TurnBack }
        Phase phase;
        private float curveTime;
        private float localXVelovity;
        private int turnBackStartDirection;
        public PlayerCrouchWalkState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            rigidbody2D.gravityScale = playerData.Walk.Physics2DGravityScale;
            localTime = 0f;
            curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Crouch.SpeedUpCurve, rigidbody2D.velocity.x / playerData.Crouch.MaxSpeed, 1, true);
            curveTime *= playerData.Crouch.SpeedUpTime;
            localXVelovity = 0;
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
        }

        public override void PhysicsCheck()
        {
            base.PhysicsCheck();
        }

        public override void SwitchStateLogic()
        {
            base.SwitchStateLogic();
            if (!InputHandler.Input_Crouch)
            {
                if (phase == Phase.SlowDown && curveTime > playerData.Crouch.SlowDownTime)
                {
                    stateMachine.ChangeState(player.IdleState);
                }
                else if (InputHandler.Input_Walk != 0)
                {
                    stateMachine.ChangeState(player.WalkState);
                }

            }
            if (phase == Phase.SlowDown && curveTime > playerData.Crouch.SlowDownTime || playerData.Physics.Slope.StayStill)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
            else if (!playerData.Physics.IsGrounded || (playerData.Physics.IsOnNotWalkableSlope && !playerData.Physics.Slope.StayStill && !playerData.Physics.IsMultipleContactWithNonWalkableSlope))
            {
                stateMachine.ChangeState(player.LandState);
            }
            else if (InputHandler.Input_Dash && playerData.Dash.DashCooldownTimer <= 0f)
            {
                stateMachine.ChangeState(player.DashState);
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
                rigidbody2D.velocity = Vector2.zero;
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
                && (phase != Phase.TurnBack || curveTime > playerData.Crouch.TurnBackTime))
            {
                if (phase != Phase.SpeedUp)
                {
                    curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Crouch.SpeedUpCurve, Mathf.Abs(localXVelovity) / playerData.Crouch.MaxSpeed, 1, true);
                    curveTime *= playerData.Crouch.SpeedUpTime;
                    phase = Phase.SpeedUp;
                }
                if (curveTime < playerData.Crouch.SpeedUpTime)
                {
                    XVelocity = playerData.Crouch.SpeedUpCurve.Evaluate(curveTime / playerData.Crouch.SpeedUpTime);
                }
                else
                {
                    XVelocity = playerData.Crouch.SpeedUpCurve.Evaluate(1f);
                }
                XVelocity *= InputHandler.Input_Walk * playerData.Crouch.MaxSpeed;
            }
            else if (InputHandler.Input_Walk != 0 && (localXVelovity != 0 && Mathf.Sign(InputHandler.Input_Walk) != Mathf.Sign(localXVelovity) || phase == Phase.TurnBack))
            {
                if (phase != Phase.TurnBack)
                {
                    curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Crouch.TurnBackCurve, Mathf.Abs(localXVelovity) / playerData.Crouch.MaxSpeed, 2f, false);
                    curveTime *= playerData.Crouch.TurnBackTime;
                    phase = Phase.TurnBack;
                    turnBackStartDirection = (int)Mathf.Sign(localXVelovity);
                }
                if (curveTime < playerData.Crouch.TurnBackTime)
                {
                    XVelocity = playerData.Crouch.TurnBackCurve.Evaluate(curveTime / playerData.Crouch.TurnBackTime);
                }
                else
                {
                    XVelocity = playerData.Crouch.TurnBackCurve.Evaluate(2f);
                }
                XVelocity *= playerData.Crouch.MaxSpeed * turnBackStartDirection;
            }
            else
            {
                if (phase != Phase.SlowDown)
                {
                    curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Crouch.SlowDownCurve, Mathf.Abs(localXVelovity) / playerData.Crouch.MaxSpeed, 1, false);
                    curveTime *= playerData.Crouch.SlowDownTime;
                    phase = Phase.SlowDown;
                }
                if (curveTime < playerData.Crouch.SlowDownTime)
                {
                    XVelocity = playerData.Crouch.SlowDownCurve.Evaluate(curveTime / playerData.Crouch.SlowDownTime);
                }
                else
                {
                    XVelocity = playerData.Crouch.SlowDownCurve.Evaluate(1f);
                }
                XVelocity *= playerData.Crouch.MaxSpeed * playerData.Physics.FacingDirection;
            }

            if (playerData.Physics.Contacts.Count == 0)
            {
                rigidbody2D.gravityScale = playerData.Land.MinLandSpeed / Physics2D.gravity.y;
            }
            else
            {
                rigidbody2D.gravityScale = playerData.Crouch.Physics2DGravityScale;
            }

            return XVelocity;
        }
    }
}

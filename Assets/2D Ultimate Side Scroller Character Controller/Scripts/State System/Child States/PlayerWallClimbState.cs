using Plugins._2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Essentials;
using UnityEngine;

namespace UltimateCC
{
    public class PlayerWallClimbState : MainState, IMove1D
    {
        enum Phase { SpeedUp, SlowDown, TurnBack, Null }
        Phase phase;
        private float curveTime;
        private int turnBackStartDirection;
        public PlayerWallClimbState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            rigidbody2D.gravityScale = playerData.Walls.Physics2DGravityScale;
            localTime = 0f;
            curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walls.WallClimb.SpeedUpCurve, rigidbody2D.velocity.x / playerData.Walls.WallClimb.MaxSpeed, 1, true);
            curveTime *= playerData.Walls.WallClimb.SpeedUpTime;
            phase = Phase.Null;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Move1D();
            if (playerData.Physics.Contacts.Count == 0)
            {
                rigidbody2D.velocity += new Vector2(playerData.Physics.WallDirection, 0f);
            }
            rigidbody2D.velocity += playerData.Physics.Platform.DampedVelocity;
            curveTime += Time.fixedDeltaTime;
            TimeBasedStaminaDrain();
        }

        public override void PhysicsCheck()
        {
            base.PhysicsCheck();
        }

        public override void SwitchStateLogic()
        {
            base.SwitchStateLogic();
            if (InputHandler.Input_Jump && playerData.Physics.CanWallJump && (playerData.Walls.CurrentStamina > 0 || playerData.Walls.AllowJumpWhenExhausted))
            {
                stateMachine.ChangeState(player.WallJumpState);
            }
            else if ((phase == Phase.SlowDown && curveTime > playerData.Walls.WallClimb.SlowDownTime && !InputHandler.Input_WallGrab) || playerData.Walls.CurrentStamina == 0)
            {
                stateMachine.ChangeState(player.WallSlideState);
            }
            else if (phase == Phase.SlowDown && curveTime > playerData.Walls.WallClimb.SlowDownTime)
            {
                stateMachine.ChangeState(player.WallGrabState);
            }
            else if (!playerData.Physics.IsNextToWall)
            {
                stateMachine.ChangeState(player.LandState);
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public void Move1D()
        {
            float newVelocity;
            newVelocity = VelocityOnY();
            rigidbody2D.velocity = newVelocity * Vector2.up;
        }
        private float VelocityOnY()
        {
            float YVelocity;
            if (InputHandler.Input_WallClimb != 0 && (rigidbody2D.velocity.y == 0 || Mathf.Sign(InputHandler.Input_WallClimb) == Mathf.Sign(rigidbody2D.velocity.y))
                && (phase != Phase.TurnBack || curveTime > playerData.Walls.WallClimb.TurnBackTime))
            {
                if (phase != Phase.SpeedUp)
                {
                    curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walls.WallClimb.SpeedUpCurve, Mathf.Abs(rigidbody2D.velocity.y) / playerData.Walls.WallClimb.MaxSpeed, 1f, true);
                    curveTime *= playerData.Walls.WallClimb.SpeedUpTime;
                    phase = Phase.SpeedUp;
                }
                if (curveTime < playerData.Walls.WallClimb.SpeedUpTime)
                {
                    YVelocity = playerData.Walls.WallClimb.SpeedUpCurve.Evaluate(curveTime / playerData.Walls.WallClimb.SpeedUpTime);
                }
                else
                {
                    YVelocity = playerData.Walls.WallClimb.SpeedUpCurve.Evaluate(1f);
                }
                YVelocity *= InputHandler.Input_WallClimb * playerData.Walls.WallClimb.MaxSpeed;
            }
            else if (InputHandler.Input_WallClimb != 0 && ((rigidbody2D.velocity.y != 0 && Mathf.Sign(InputHandler.Input_WallClimb) != Mathf.Sign(rigidbody2D.velocity.y)) || phase == Phase.TurnBack))
            {
                if (phase != Phase.TurnBack)
                {
                    curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walls.WallClimb.TurnBackCurve, Mathf.Abs(rigidbody2D.velocity.y) / playerData.Walls.WallClimb.MaxSpeed, 2f, false);
                    curveTime *= playerData.Walls.WallClimb.TurnBackTime;
                    phase = Phase.TurnBack;
                    turnBackStartDirection = (int)Mathf.Sign(rigidbody2D.velocity.y);
                }
                if (curveTime < playerData.Walls.WallClimb.TurnBackTime)
                {
                    YVelocity = playerData.Walls.WallClimb.TurnBackCurve.Evaluate(curveTime / playerData.Walls.WallClimb.TurnBackTime);
                }
                else
                {
                    YVelocity = playerData.Walls.WallClimb.TurnBackCurve.Evaluate(2f);
                }
                YVelocity *= playerData.Walls.WallClimb.MaxSpeed * turnBackStartDirection;
            }
            else
            {
                if (phase != Phase.SlowDown)
                {
                    curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walls.WallClimb.SlowDownCurve, Mathf.Abs(rigidbody2D.velocity.y) / playerData.Walls.WallClimb.MaxSpeed, 1f, false);
                    curveTime *= playerData.Walls.WallClimb.SlowDownTime;
                    phase = Phase.SlowDown;
                }
                if (curveTime < playerData.Walls.WallClimb.SlowDownTime)
                {
                    YVelocity = playerData.Walls.WallClimb.SlowDownCurve.Evaluate(curveTime / playerData.Walls.WallClimb.SlowDownTime);
                }
                else
                {
                    YVelocity = playerData.Walls.WallClimb.SlowDownCurve.Evaluate(1f);
                }
                YVelocity *= playerData.Walls.WallClimb.MaxSpeed * Mathf.Sign(rigidbody2D.velocity.y);
            }
            ClimbAmountStaminaDrain(YVelocity);
            return YVelocity;
        }

        private void ClimbAmountStaminaDrain(float YVelocity)
        {
            if (playerData.Walls.ExhaustTrigger == PlayerData.WallMovementVariables.ExhaustTriggerType.ClimbAmountBased)
            {
                playerData.Walls.CurrentStamina = Mathf.Clamp(playerData.Walls.CurrentStamina - (YVelocity * Time.fixedDeltaTime * playerData.Walls.StaminaDrainPerTrigger), 0, playerData.Walls.MaxStamina);
            }
        }

        private void TimeBasedStaminaDrain()
        {
            if (playerData.Walls.ExhaustTrigger == PlayerData.WallMovementVariables.ExhaustTriggerType.TimeBased)
            {
                playerData.Walls.CurrentStamina = Mathf.Clamp(playerData.Walls.CurrentStamina - (Time.fixedDeltaTime * playerData.Walls.StaminaDrainPerTrigger), 0, playerData.Walls.MaxStamina);
            }
        }
    }
}

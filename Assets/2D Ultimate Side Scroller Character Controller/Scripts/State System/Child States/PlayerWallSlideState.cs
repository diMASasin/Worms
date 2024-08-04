using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Essentials;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Base_States;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Child_States.Interfaces;
using UnityEngine;

namespace _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Child_States
{
    public class PlayerWallSlideState : MainState, IMove1D
    {
        enum Phase { SpeedUp, SlowDown, TurnBack, Null }
        Phase phase;
        private float curveTime;
        private int turnBackStartDirection;
        public PlayerWallSlideState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            rigidbody2D.gravityScale = playerData.Walls.Physics2DGravityScale;
            localTime = 0f;
            curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walls.WallSlide.SpeedUpCurve, rigidbody2D.velocity.x / playerData.Walls.WallSlide.MaxSpeed, 1, true);
            curveTime *= playerData.Walls.WallSlide.SpeedUpTime;
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
            else if (!playerData.Physics.IsNextToWall || (InputHandler.Input_Walk != 0 && Mathf.Sign(InputHandler.Input_Walk) != Mathf.Sign(playerData.Physics.WallDirection)))
            {
                stateMachine.ChangeState(player.LandState);
            }
            else if (InputHandler.Input_WallGrab && playerData.Walls.CurrentStamina > 0 && phase == Phase.SlowDown && playerData.Walls.WallSlide.SlowDownTime < curveTime)
            {
                stateMachine.ChangeState(player.WallGrabState);
            }
            if ((((playerData.Physics.IsGrounded && !playerData.Physics.IsOnNotWalkableSlope)) && (InputHandler.Input_Walk == 0 || Mathf.Sign(InputHandler.Input_Walk) == Mathf.Sign(playerData.Physics.WallDirection))) || playerData.Physics.IsMultipleContactWithNonWalkableSlope)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else if ((playerData.Physics.IsGrounded && !playerData.Physics.IsOnNotWalkableSlope))
            {
                stateMachine.ChangeState(player.WalkState);
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public void Move1D()
        {
            float newVelocity = VelocityOnY();
            rigidbody2D.velocity = newVelocity * Vector2.up;
        }
        private float VelocityOnY()
        {
            float YVelocity = 0;
            if (rigidbody2D.velocity.y > 0 || (phase == Phase.TurnBack && curveTime <= playerData.Walls.WallSlide.TurnBackTime))
            {
                if (phase != Phase.TurnBack)
                {
                    curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walls.WallSlide.TurnBackCurve, Mathf.Abs(rigidbody2D.velocity.y) / playerData.Walls.WallSlide.MaxSpeed, 2f, false);
                    curveTime *= playerData.Walls.WallSlide.TurnBackTime;
                    phase = Phase.TurnBack;
                    turnBackStartDirection = -1;
                }
                if (curveTime < playerData.Walls.WallSlide.TurnBackTime)
                {
                    YVelocity = playerData.Walls.WallSlide.TurnBackCurve.Evaluate(curveTime / playerData.Walls.WallSlide.TurnBackTime);
                }
                else
                {
                    YVelocity = playerData.Walls.WallSlide.TurnBackCurve.Evaluate(2f);
                }
                YVelocity *= playerData.Walls.WallSlide.MaxSpeed * turnBackStartDirection;
            }
            else if (!InputHandler.Input_WallGrab || playerData.Walls.CurrentStamina == 0)
            {
                if (phase != Phase.SpeedUp)
                {
                    curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walls.WallSlide.SpeedUpCurve, Mathf.Abs(rigidbody2D.velocity.y) / playerData.Walls.WallSlide.MaxSpeed, 1f, true);
                    curveTime *= playerData.Walls.WallSlide.SpeedUpTime;
                    phase = Phase.SpeedUp;
                }
                if (curveTime < playerData.Walls.WallSlide.SpeedUpTime)
                {
                    YVelocity = playerData.Walls.WallSlide.SpeedUpCurve.Evaluate(curveTime / playerData.Walls.WallSlide.SpeedUpTime);
                }
                else
                {
                    YVelocity = playerData.Walls.WallSlide.SpeedUpCurve.Evaluate(1f);
                }
                YVelocity *= playerData.Walls.WallSlide.MaxSpeed;
            }
            else if (InputHandler.Input_WallGrab && playerData.Walls.CurrentStamina > 0)
            {
                if (phase != Phase.SlowDown)
                {
                    curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walls.WallSlide.SlowDownCurve, Mathf.Abs(rigidbody2D.velocity.y) / playerData.Walls.WallSlide.MaxSpeed, 1f, false);
                    curveTime *= playerData.Walls.WallSlide.SlowDownTime;
                    phase = Phase.SlowDown;
                }
                if (curveTime < playerData.Walls.WallSlide.SlowDownTime)
                {
                    YVelocity = playerData.Walls.WallSlide.SlowDownCurve.Evaluate(curveTime / playerData.Walls.WallSlide.SlowDownTime);
                }
                else
                {
                    YVelocity = playerData.Walls.WallSlide.SlowDownCurve.Evaluate(1f);
                }
                YVelocity *= playerData.Walls.WallSlide.MaxSpeed;
            }

            return -YVelocity;
        }
    }
}

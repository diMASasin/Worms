using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Essentials;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Base_States;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Child_States.Interfaces;
using UnityEngine;

namespace _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Child_States
{
    public class PlayerWallGrabState : MainState, IMove1D
    {
        private float curveTime;
        public PlayerWallGrabState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum, PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            rigidbody2D.gravityScale = playerData.Walls.Physics2DGravityScale;
            curveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Walls.WallGrab.SlowDownCurve, Mathf.Abs(rigidbody2D.velocity.y) / playerData.Walls.WallClimb.MaxSpeed, 1f, false);
            curveTime *= playerData.Walls.WallGrab.SlowDownTime;
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
            if (playerData.Physics.Contacts.Count == 0)
            {
                rigidbody2D.velocity += new Vector2(playerData.Physics.WallDirection, 0f);
            }
            TimeBasedStaminaDrain();
            curveTime += Time.fixedDeltaTime;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void PhysicsCheck()
        {
            base.PhysicsCheck();
            if (InputHandler.Input_Jump && playerData.Physics.CanWallJump && (playerData.Walls.CurrentStamina > 0 || playerData.Walls.AllowJumpWhenExhausted))
            {
                stateMachine.ChangeState(player.WallJumpState);
            }
            else if (!InputHandler.Input_WallGrab || playerData.Walls.CurrentStamina == 0)
            {
                stateMachine.ChangeState(player.WallSlideState);
            }
            else if (InputHandler.Input_WallClimb != 0)
            {
                stateMachine.ChangeState(player.WallClimbState);
            }
        }

        public override void SwitchStateLogic()
        {
            base.SwitchStateLogic();
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
            if (curveTime < playerData.Walls.WallGrab.SlowDownTime)
            {
                YVelocity = playerData.Walls.WallGrab.SlowDownCurve.Evaluate(curveTime / playerData.Walls.WallGrab.SlowDownTime);
            }
            else
            {
                YVelocity = playerData.Walls.WallGrab.SlowDownCurve.Evaluate(1f);
            }
            YVelocity *= playerData.Walls.WallClimb.MaxSpeed * Mathf.Sign(rigidbody2D.velocity.y);
            return YVelocity;
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

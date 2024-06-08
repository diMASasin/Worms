using Plugins._2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Essentials;
using UnityEngine;
using static UltimateCC.PlayerData;
using static UltimateCC.PlayerData.JumpVariables.JumpType;

namespace UltimateCC
{
    public class PlayerJumpState : MainState, IMove2D
    {
        enum Phase
        {
            SpeedUp,
            SlowDown,
            TurnBack,
            Null
        }

        Phase phase;
        float cutJumpTime;
        float xCurveTime;
        private float localXVelovity;
        private int turnBackStartDirection;
        JumpVariables.JumpInfo jumpInfo;
        private int _facingDirection;
        private JumpVariables.JumpInf JumpType => jumpInfo.currentJumpType;


        public PlayerJumpState(PlayerMain player, PlayerStateMachine stateMachine, PlayerMain.AnimName animEnum,
            PlayerData playerData) : base(player, stateMachine, animEnum, playerData)
        {
        }

        public override void Enter()
        {
            base.Enter();
            var dataJump = playerData.Jump;
            _facingDirection = playerData.Physics.FacingDirection;
            
            switch (dataJump.NextJumpInt)
            {
                case 1:
                    jumpInfo = dataJump.Jumps[0];
                    break;
                case 2:
                    jumpInfo = dataJump.Jumps[1];
                    break;
                case 3:
                    jumpInfo = dataJump.Jumps[2];
                    break;
                default:
                    Debug.LogError("jump array error");
                    break;
            }

            jumpInfo.currentJumpType = dataJump.currentJumpType == HighJump ? jumpInfo.HighJump : jumpInfo.LongJump;
            dataJump.NextJumpInt++;
            player.Animator.SetBool(_animEnum.ToString(), false);
            player.Animator.SetBool(JumpType.Animation.ToString(), true);
            player.Rigidbody2D.velocity = new Vector2(player.Rigidbody2D.velocity.x, JumpType.MaxHeight);
            dataJump.JumpBufferTimer = 0;
            cutJumpTime = 0f;
            playerData.Physics.CutJump = false;
            dataJump.NewJump = false;
            rigidbody2D.gravityScale = dataJump.Physics2DGravityScale;
            if (rigidbody2D.velocity.x != 0)
            {
                phase = InputHandler.Input_Walk != 0 ? Phase.SpeedUp : Phase.SlowDown;
            }
            else
            {
                phase = Phase.Null;
            }

            localXVelovity = 0;
            rigidbody2D.AddForce(new Vector2(JumpType.jumpXImpulse, 0), ForceMode2D.Impulse);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PhysicsVariables physics = playerData.Physics;
            WalkVariables walk = playerData.Walk;
            
            if (cutJumpTime == 0 && physics.CutJump)
            {
                cutJumpTime = localTime;
                localTime = cutJumpTime * (cutJumpTime + (JumpType.JumpTime - cutJumpTime) * JumpType.TimeScaleOnCut) /
                            JumpType.JumpTime;
            }

            Move2D();
            rigidbody2D.velocity += physics.Platform.DampedVelocity;
            xCurveTime += Time.fixedDeltaTime;

            Vector2 inputSpeed = new Vector2(player.InputHandler.Input_Walk * playerData.Walk.MaxSpeed, 0);
            float jumpXImpulse = jumpInfo.currentJumpType.jumpXImpulse;
            inputSpeed += rigidbody2D.velocity;
            inputSpeed.x = Mathf.Clamp(inputSpeed.x, -jumpXImpulse, jumpXImpulse);
            rigidbody2D.velocity = inputSpeed;
        }

        public override void Exit()
        {
            base.Exit();
            player.Animator.SetBool(JumpType.Animation.ToString(), false);
            playerData.Physics.CutJump = false;
        }

        public override void PhysicsCheck()
        {
            base.PhysicsCheck();
        }

        public override void SwitchStateLogic()
        {
            base.SwitchStateLogic();

            if (localTime >= JumpType.JumpTime || (playerData.Physics.IsOnHeadBump && playerData.Physics.CanBumpHead))
            {
                stateMachine.ChangeState(player.LandState);
            }
            else if (InputHandler.Input_Dash && playerData.Dash.DashCooldownTimer <= 0f)
            {
                stateMachine.ChangeState(player.DashState);
            }
            else if (playerData.Physics.IsNextToWall)
            {
                if (InputHandler.Input_WallGrab)
                {
                    stateMachine.ChangeState(player.WallGrabState);
                }
                else if (localTime > playerData.Jump.IgnoreWallSlideTime && InputHandler.Input_Walk != 0 &&
                         Mathf.Sign(InputHandler.Input_Walk) == Mathf.Sign(playerData.Physics.WallDirection))
                {
                    stateMachine.ChangeState(player.WallSlideState);
                }
            }
        }

        public void Move2D()
        {
            Vector2 _newVelocity = Vector2.zero;
            if (localTime <= JumpType.JumpTime && !playerData.Physics.CutJump)
            {
                _newVelocity.y = JumpType.JumpVelocityCurve.Evaluate(localTime / JumpType.JumpTime);
                _newVelocity.y *= JumpType.MaxHeight * 1 / JumpType.JumpTime;
            }
            else if (localTime <= cutJumpTime + ((JumpType.JumpTime - cutJumpTime) * JumpType.TimeScaleOnCut) &&
                     playerData.Physics.CutJump)
            {
                _newVelocity.y = JumpType.JumpVelocityCurve.Evaluate(localTime /
                                                                     (cutJumpTime + ((JumpType.JumpTime - cutJumpTime) *
                                                                         JumpType.TimeScaleOnCut)));
                _newVelocity.y *= JumpType.MaxHeight * 1 / JumpType.JumpTime;
                _newVelocity.y *= 1 - JumpType.JumpCutPower;
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
            
            rigidbody2D.velocity = _newVelocity;
            localXVelovity = rigidbody2D.velocity.x;
        }

        private float VelocityOnx()
        {
            float XVelocity = rigidbody2D.velocity.x;
            if (InputHandler.Input_Walk != 0 && (localXVelovity == 0 ||
                                                 Mathf.Sign(InputHandler.Input_Walk) == Mathf.Sign(localXVelovity))
                                             && (phase != Phase.TurnBack || xCurveTime > playerData.Walk.TurnBackTime))
            {
                if (phase != Phase.SpeedUp && (phase != Phase.TurnBack || xCurveTime > playerData.Walk.TurnBackTime))
                {
                    xCurveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Jump.XSpeedUpCurve,
                        Mathf.Abs(rigidbody2D.velocity.x) / playerData.Jump.CurrentJump.jumpXImpulse, 1, true);
                    xCurveTime *= playerData.Jump.XSpeedUpTime;
                    phase = Phase.SpeedUp;
                }

                if (xCurveTime < playerData.Jump.XSpeedUpTime)
                {
                    XVelocity = playerData.Jump.XSpeedUpCurve.Evaluate(xCurveTime / playerData.Jump.XSpeedUpTime);
                }
                else
                {
                    XVelocity = playerData.Jump.XSpeedUpCurve.Evaluate(1f);
                }

                XVelocity *= InputHandler.Input_Walk * playerData.Jump.CurrentJump.jumpXImpulse;
            }
            else if (InputHandler.Input_Walk != 0 &&
                     ((localXVelovity != 0 && Mathf.Sign(InputHandler.Input_Walk) != Mathf.Sign(localXVelovity)) ||
                      phase == Phase.TurnBack))
            {
                if (phase != Phase.TurnBack)
                {
                    xCurveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Jump.XTurnBackCurve,
                        Mathf.Abs(localXVelovity) / playerData.Jump.CurrentJump.jumpXImpulse, 2f, false);
                    xCurveTime *= playerData.Jump.XTurnBackTime;
                    phase = Phase.TurnBack;
                    turnBackStartDirection = (int)Mathf.Sign(localXVelovity);
                }

                if (xCurveTime < playerData.Jump.XTurnBackTime)
                {
                    XVelocity = playerData.Jump.XTurnBackCurve.Evaluate(xCurveTime / playerData.Jump.XTurnBackTime);
                }
                else
                {
                    XVelocity = playerData.Jump.XTurnBackCurve.Evaluate(2f);
                }

                XVelocity *= playerData.Jump.CurrentJump.jumpXImpulse * turnBackStartDirection;
            }
            else if (localXVelovity != 0)
            {
                if (phase != Phase.SlowDown)
                {
                    xCurveTime = EssentialPhysics.SetCurveTimeByValue(playerData.Jump.XSlowDownCurve,
                        Mathf.Abs(rigidbody2D.velocity.x) / playerData.Jump.CurrentJump.jumpXImpulse, 1, false);
                    xCurveTime *= playerData.Jump.XSlowDownTime;
                    phase = Phase.SlowDown;
                }

                if (xCurveTime < playerData.Jump.XSlowDownTime)
                {
                    XVelocity = playerData.Jump.XSlowDownCurve.Evaluate(xCurveTime / playerData.Jump.XSlowDownTime);
                }
                else
                {
                    XVelocity = playerData.Jump.XSlowDownCurve.Evaluate(1f);
                }

                XVelocity *= playerData.Jump.CurrentJump.jumpXImpulse * _facingDirection * (int)JumpType.JumpDirection;
            }
            else
            {
                XVelocity = rigidbody2D.velocity.x;
                phase = Phase.Null;
            }

            return XVelocity;
        }
    }
}
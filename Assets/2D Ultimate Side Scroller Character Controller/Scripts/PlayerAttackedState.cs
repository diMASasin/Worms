using System;
using System.Threading;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Base_States;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ITimer = Timers.ITimer;

namespace _2D_Ultimate_Side_Scroller_Character_Controller.Scripts
{
    public class PlayerAttackedState : MainState
    {
        private readonly ITimer _timer;
        private CancellationTokenSource _tokenSource; 
        
        public PlayerAttackedState(PlayerMain playerMain, PlayerStateMachine playerStateMachine,
            PlayerMain.AnimName idle, PlayerData playerData, ITimer timer) : base(playerMain, playerStateMachine, idle, playerData)
        {
            _timer = timer;
        }

        public override void Enter()
        {
            base.Enter();

            _tokenSource = new CancellationTokenSource();
            WaitForStop().Forget();
        }

        public override void Exit()
        {
            base.Exit();
            
            _timer.Stop();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (playerData.Physics.died == true)
                stateMachine.ChangeState(player.IdleState);
                
            rigidbody2D.gravityScale = playerData.Land.Physics2DGravityScale;
        }

        private async UniTaskVoid WaitForStop()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: _tokenSource.Token);

            try
            {
                await UniTask.WaitWhile(
                    () => playerData.Physics.IsGrounded == false && rigidbody2D.velocity.magnitude > 0.2f,
                    cancellationToken: _tokenSource.Token);
            }
            catch (MissingReferenceException exception)
            {
                return;
            }
            
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
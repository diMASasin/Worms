using System;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Base_States;
using Cysharp.Threading.Tasks;
using Infrastructure.Interfaces;
using Timers;
using UnityEngine;

namespace _2D_Ultimate_Side_Scroller_Character_Controller.Scripts
{
    public class PlayerAttackedState : MainState
    {
        private readonly ITimer _timer;
        private readonly ICoroutinePerformer _coroutinePerformer;
        private Coroutine _coroutine;

        public PlayerAttackedState(PlayerMain playerMain, PlayerStateMachine playerStateMachine,
            PlayerMain.AnimName idle, PlayerData playerData, ITimer timer) : base(playerMain, playerStateMachine, idle, playerData)
        {
            _coroutinePerformer = playerMain;
            _timer = timer;
        }

        public override void Enter()
        {
            base.Enter();

            WaitForStop().Forget();
        }

        public override void Exit()
        {
            base.Exit();
            
            _timer.Stop();
            
            if(_coroutine != null)
                _coroutinePerformer.StopCoroutine(_coroutine);
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
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
            await UniTask.WaitWhile(() => playerData.Physics.IsGrounded == false && rigidbody2D.velocity.magnitude > 0.2f);
            
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
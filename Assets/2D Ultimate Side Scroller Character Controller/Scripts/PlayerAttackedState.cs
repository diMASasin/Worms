using System.Collections;
using Infrastructure;
using Timers;
using UnityEngine;
using Zenject;

namespace UltimateCC
{
    public class PlayerAttackedState : MainState
    {
        private readonly Timer _timer;
        private readonly ICoroutinePerformer _coroutinePerformer;
        private Coroutine _coroutine;

        public PlayerAttackedState(PlayerMain playerMain, PlayerStateMachine playerStateMachine,
            PlayerMain.AnimName idle, PlayerData playerData) : base(playerMain, playerStateMachine, idle, playerData)
        {
            _coroutinePerformer = playerMain;
            _timer = new Timer(_coroutinePerformer);
        }

        public override void Enter()
        {
            base.Enter();
            
            _timer.Start(0.05f, () => _coroutine = _coroutinePerformer.StartCoroutine(WaitForStop()));
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

        private IEnumerator WaitForStop()
        {
            while (playerData.Physics.IsGrounded == false && rigidbody2D.velocity.magnitude > 0.2f)
                yield return null;

            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
using UnityEngine;
using static UltimateCC.PlayerMain;

namespace UltimateCC
{
    internal class PlayerFreeFallingState : MainState
    {
        private readonly PlayerMain _playerMain;
        private readonly PlayerStateMachine _stateMachine;
        private float _speed = 9.8f;

        public PlayerFreeFallingState(PlayerMain playerMain, PlayerStateMachine stateMachine, 
            AnimName animName, PlayerData playerData) : base(playerMain, stateMachine, animName, playerData)
        {
            _playerMain = playerMain;
            _stateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            base.Update();
            _playerMain.Rigidbody2D.velocity += Vector2.down * (_speed * Time.deltaTime);
            
            if(playerData.Physics.IsGrounded == true)
                _stateMachine.ChangeState(_playerMain.IdleState);
        }
    }
}
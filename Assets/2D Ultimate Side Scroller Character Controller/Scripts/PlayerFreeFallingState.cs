using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Base_States;
using UnityEngine;
using static _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.PlayerMain;

namespace _2D_Ultimate_Side_Scroller_Character_Controller.Scripts
{
    internal class PlayerFreeFallingState : MainState
    {
        private readonly PlayerMain _playerMain;
        private readonly PlayerStateMachine _stateMachine;
        private readonly float _speed = 9.8f;
        private RigidbodyConstraints2D _previousConstraints;

        public PlayerFreeFallingState(PlayerMain playerMain, PlayerStateMachine stateMachine, 
            AnimName animName, PlayerData playerData) : base(playerMain, stateMachine, animName, playerData)
        {
            _playerMain = playerMain;
            _stateMachine = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            _previousConstraints = _playerMain.Rigidbody2D.constraints;
            _playerMain.Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public override void Update()
        {
            base.Update();
            _playerMain.Rigidbody2D.velocity += Vector2.down * (_speed * Time.deltaTime);

            if (playerData.Physics.IsGrounded == true && _playerMain.Rigidbody2D.velocity.magnitude < 0.2f)
            {
                _playerMain.Rigidbody2D.constraints = _previousConstraints;
                _stateMachine.ChangeState(_playerMain.IdleState);
            }
        }
    }
}
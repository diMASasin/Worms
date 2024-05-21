﻿using BattleStateMachineComponents;
using Infrastructure;
using UnityEngine;
using PlayerInput = InputService.PlayerInput;

namespace Battle_
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private CoroutinePerformer _coroutinePerformer;
        [SerializeField] private BattleStateMachineData _data;
    
        private Battle _battle;
        private MainInput _mainInput;
        private PlayerInput _input;
    
        private void Start()
        {
            _coroutinePerformer.Init();
            _mainInput = new MainInput();
        
            _data.Init(_mainInput);
            _battle = new Battle(_data);
        
            _battle.Start();
        }
    
        private void Update()
        {
            _battle.Tick();
        }

        private void FixedUpdate()
        {
            _battle.FixedTick();
        }

        private void OnDestroy()
        {
            if (_battle != null) _battle.Dispose();
        }
    }
}
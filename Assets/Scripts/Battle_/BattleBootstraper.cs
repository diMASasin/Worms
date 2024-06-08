using BattleStateMachineComponents;
using BattleStateMachineComponents.StatesData;
using Infrastructure;
using Services;
using UI;
using UltimateCC;
using UnityEngine;

namespace Battle_
{
    public class BattleBootstraper : MonoBehaviour
    {
        [SerializeField] private BattleStateMachineData _data;
        
        private Battle _battle;
    
        private void Start()
        {
            _battle = new Battle(_data, AllServices.Container);
        
            _battle.Start();
        }
    
        private void Update()
        {
            _battle?.Tick();
        }

        private void FixedUpdate()
        {
            _battle?.FixedTick();
        }
        
        private void LateUpdate()
        {
            _battle?.LateTick();
        }

        private void OnDestroy()
        {
            if (_battle != null) _battle.Dispose();
        }
    }
}

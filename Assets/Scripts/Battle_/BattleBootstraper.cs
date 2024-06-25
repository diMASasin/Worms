using BattleStateMachineComponents;
using BattleStateMachineComponents.States;
using UnityEngine;
using Zenject;

namespace Battle_
{
    public class BattleBootstraper : MonoBehaviour
    {
        private IBattleStateSwitcher _battleStateSwitcher;

        [Inject]
        private void Construct(IBattleStateSwitcher battleStateSwitcher)
        {
            _battleStateSwitcher = battleStateSwitcher;
        }

        private void Start()
        {
            _battleStateSwitcher.SwitchState<BootstrapBattleState>();
        }
    }
}

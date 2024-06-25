using GameStateMachineComponents.States;
using Zenject;

namespace GameStateMachineComponents
{
    public class StateFactory
    {
        private readonly DiContainer _container;

        public StateFactory(DiContainer container) =>
            _container = container;

        public T CreateState<T>() where T : GameState =>
            _container.Resolve<T>();
    }
}
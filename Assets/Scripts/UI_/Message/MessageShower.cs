using EventProviders;
using UnityEngine;
using WormComponents;
using Zenject;

namespace UI_.Message
{
    public class MessageShower : MonoBehaviour, IMessageShower
    {
        [SerializeField] private MessageFactory _factory;
        [SerializeField] private MessagesConfig _config;
        [SerializeField] private Animator _gradientAnimator;
        
        private IGameEventsProvider _gameEvents;
        private MessageView _view;
        
        private static readonly int AppearTrigger = Animator.StringToHash("Appear");
        private IWormEvents _wormEvents;

        [Inject]
        private void Construct(IWormEvents wormEvents)
        {
            _wormEvents = wormEvents;
            
            _wormEvents.WormDied += OnWormDied;
        }

        private void OnDestroy()
        {
            _wormEvents.WormDied -= OnWormDied;
        }
        
        private void Start()
        {
            _view = _factory.Create();
        }

        public void AppearTurnStartedText() => Appear(_config.TurnStartedText);

        public void AppearGetReadyText() => Appear(_config.GetReadyText);

        private void Appear(string text)
        {
            _gradientAnimator.SetTrigger(AppearTrigger);
            _view.Appear(text);
        }

        private void OnWormDied(Worm worm) => Appear(_config.WormDiedText);
    }
}
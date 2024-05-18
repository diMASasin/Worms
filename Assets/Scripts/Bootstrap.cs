using GameBattleStateMachine;
using UnityEngine;
using PlayerInput = PlayerInputSystem.PlayerInput;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private CoroutinePerformer _coroutinePerformer;
    [SerializeField] private BattleStateMachineData _data;
    
    private Game _game;
    private MainInput _mainInput;
    private PlayerInput _input;
    
    private void Awake()
    {
        _coroutinePerformer.Init();
        _mainInput = new MainInput();
        _input = new PlayerInput(_mainInput);
        
        _data.Init(_input);
        _game = new Game(_data);
        
        _game.Start();
    }
    
    private void Update()
    {
        _game.Tick();
    }

    private void FixedUpdate()
    {
        _game.FixedTick();
    }

    private void OnDestroy()
    {
        if (_game != null) _game.Dispose();
    }
}

using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System.Base_States;

namespace _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.State_System
{
    [System.Serializable]
    public class PlayerStateMachine
    {
        // Declaration of current runtime state
        public MainState CurrentState { get; private set; }

        // Function to initialize starting state
        public void Initialize(MainState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        // Function to change current state
        public void ChangeState(MainState newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}

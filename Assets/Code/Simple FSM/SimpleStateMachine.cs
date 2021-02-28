#if UNITY_EDITOR
//    #define TEST_FSM
#endif

using System;
using UnityEngine;

public class SimpleStateMachine<T>
{
    public event Action<T, T> OnStateTransition;

    public event Action<T> OnStateEnter;

    public event Action<T> OnStateExit;

    public T UninitializedState { get; private set; }

    public T CurrentState { get; private set; }

    public SimpleStateMachine( T initialState, Action<T> OnStateEnterFunc = null )
    {
        CurrentState = initialState;

        UninitializedState = initialState;
        CurrentState = initialState;

        if( OnStateEnterFunc != null )
            OnStateEnter += OnStateEnterFunc;
        if( OnStateEnter != null )
            OnStateEnter( CurrentState );
    }

    public T ChangeState( T stateNew )
    {
#if TEST_FSM
        Debug.LogFormat( "FSM {0} -> {1}\nOnStateTransition:{2}", CurrentState, stateNew, OnStateTransition.GetInvocationList().Print() );
#endif

        if( CurrentState.Equals( stateNew ) )
        {
            return CurrentState;
        }

        if( OnStateExit != null )
            OnStateExit( CurrentState );

        if( OnStateTransition != null )
            OnStateTransition( CurrentState, stateNew );

        CurrentState = stateNew;

        if( OnStateEnter != null )
            OnStateEnter( stateNew );

        return stateNew;
    }

#if DEBUG_PRINT

    public string GetListeners()
    {
        return OnStateEnter.GetInvocationList().Print();
    }

#endif
}

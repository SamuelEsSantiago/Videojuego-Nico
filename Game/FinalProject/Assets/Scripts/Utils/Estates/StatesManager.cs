using System;
using System.Collections.Generic;
using UnityEngine;

public class StatesManager : MonoBehaviour
{
    public Entity hostEntity;
    public Entity enemy;
    public List<State> currentStates = new List<State>();
    public List<State> bannedStates = new List<State>();
    public delegate void StatusCheck();
    public StatusCheck statusCheck;   
    private void Start() {
        hostEntity=gameObject.GetComponent<Entity>();
    } 
    void Update()
    {
        if(statusCheck!=null){
            statusCheck.Invoke();
        }
    }

    /// <summary>
    /// Adds and starts a new state to the entity manager
    /// </summary>
    /// <param name="newState">State to add</param>
    /// <returns>The instantiated state (could be different from the original)</returns>
    public State AddState(State newState){
        if(newState != null){
            if (!currentStates.Contains(newState) && !bannedStates.Contains(newState))
            {
                if (newState.onEffect)
                {
                    Debug.Log(gameObject + " manager cloned " + newState);
                    newState = Instantiate(newState);
                    newState.onEffect = false;
                }


                newState.StartAffect(this);
                currentStates.Add(newState);

                return newState;
            }
        }

        return null;
    }
    public State AddState(State newState, Entity newEnemy){
        if(newState != null){
            if (!currentStates.Contains(newState))
            {
                if (newState.onEffect)
                {
                    Debug.Log(gameObject + " manager cloned " + newState);
                    newState = Instantiate(newState);
                    newState.onEffect = false;
                }

                enemy=newEnemy;
                newState.StartAffect(this);
                currentStates.Add(newState);

                return newState;

            }
        }
        return null;
    }


    public State AddStateDontRepeat(State newState)
    {
        if(newState != null){
            if (!currentStates.Exists(s => s.name == newState.name))
            {
                if (newState.onEffect)
                {
                    Debug.Log(gameObject + " manager cloned " + newState);
                    newState = Instantiate(newState);
                    newState.onEffect = false;
                }


                newState.StartAffect(this);
                currentStates.Add(newState);

                return newState;
            }

            return currentStates.Find( s => s.name == newState.name);
        }

        return null;
    }


    public void RemoveState(State state){
        if(currentStates.Contains(state)){
            currentStates.Remove(state);
        }
    }

    /*public void RemoveFirst(Type stateType)
    {
        var states = currentStates.FindAll(s => s.GetType() == stateType);
    }*/

    public void RemoveAll( )
    {
        currentStates.ForEach(s => s.StopAffect());
    }

    public void RemoveAll( Type stateType )
    {
        if (stateType.Equals(typeof(State)) || stateType.IsSubclassOf (typeof(State)))
        {
            var states = currentStates.FindAll(s => s.GetType() == stateType);
            states.ForEach(s => s.StopAffect());
        }
    }

    public void RemoveAll(Type stateType, State exlude)
    {
        if (stateType.Equals(typeof(State)) || stateType.IsSubclassOf (typeof(State)))
        {
            var states = currentStates.FindAll(s => s != exlude && s.GetType() == stateType);
            states.ForEach(s => s.StopAffect());
        }
    }
}

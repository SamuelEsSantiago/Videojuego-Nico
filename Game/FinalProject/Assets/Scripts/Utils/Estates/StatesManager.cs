using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatesManager : MonoBehaviour
{
    public Entity hostEntity;
    private List<State> currentStates = new List<State>();
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

    public void AddState(State newState){
        if(newState != null){
            newState.StartAffect(this);
            currentStates.Add(newState);
        }
    }
    public void RemoveState(State state){
        if(currentStates.Contains(state)){
            currentStates.Remove(state);
        }
    }
}

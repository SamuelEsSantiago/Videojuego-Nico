using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : ScriptableObject {
    [System.NonSerialized] public GameObject gameObject;
    public enum InteractionType{Dialogue,Interface,GiveItem,Question,Animation,WaitCon,WorldState,OpenMenu,CloseMenu,Camera,Teleport,None}
    public InterCondition condition = null;
    public InteractionType type = InteractionType.None;
    public delegate void OnEndInteraction();
    public OnEndInteraction onEndInteraction;    
    public abstract void DoInteraction();
    public virtual void RestardCondition(){
        if(condition != null) condition.RestardValues(gameObject);
    }
}



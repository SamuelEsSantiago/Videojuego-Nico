using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public Sprite iconAbility;
    public bool isPasive;
    [TextArea (3,10)]public string description;
    public enum Abilities
    {
        Invisibilidad, 
        Correr, 
        DashHorizontal, 
        Trepar, 
        DobleSalto, 
        Overlord, 
        RáfagaDeViento, 
        DashVertical, 
        AveMaria, 
        VisionMejorada, 
        Escudo, 
        DodgePerfecto, 
        SuperFuerza, 
        VisionFutura, 
        Volar
    }
    [SerializeField] protected float cooldownTime;
    public virtual KeyCode hotkey {get;set;}
    [SerializeField] public float staminaCost;
    [SerializeField] protected float duration;
    [SerializeField] protected bool isInCooldown;
    public Abilities abilityName;
    protected bool isActive;
    protected PlayerManager player;
    protected float time;
    public bool isUnlocked;
    public bool beenUsed;

    public virtual void UseAbility()
    {
        if(player.currentStamina < staminaCost+.1)return;
        if (player.statesManager.currentStates.Contains(Escudo.scudoState))
        {
            beenUsed = true;
        }
        isInCooldown = true; 
        //Debug.Log($"Usando {abilityName.ToString()}");
        if (isInCooldown)
        {
            player.TakeTirement(staminaCost);
            time = 0;
            //Debug.Log("Usando en cooldown");
        }
    }

    protected virtual void Start()
    {
        player = PlayerManager.instance;
        time = 0;
    }

    protected virtual void Update()
    {
        if (!isUnlocked)
        {
            return;
        }
        if (Input.GetKeyDown(hotkey))
        {
            UseAbility();
        }
        if (isInCooldown)
        {
            time += Time.deltaTime;
            if (time >= cooldownTime)
            {
                isInCooldown = false;
                time = 0;
            }
        }
    }
    public float GetStaminaCost(){
        return staminaCost;
    }
    public void SetStaminaCost(float newStaminaCost){
        staminaCost = newStaminaCost;
    }
}

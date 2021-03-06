using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Item", menuName = "Inventory/ItemStamina")]
public class ItemStamina : Item
{
    public int staminaGain; 
    public override void Use()
    {
        if(isInCooldown){
            Debug.Log("Objeto en cooldown");
            return;
        }
        if(staminaGain < 0){
            PlayerManager.instance.TakeTirement(-staminaGain);
        }else{
            PlayerManager.instance.RegenStaminaDontLimit(staminaGain);
        }
        base.Use();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : ItemSlot
{
    EquipmentManager equipmentManager;
    InventoryUI inventoryUI;
    private void Start() {
        inventoryUI = InventoryUI.instance;
        equipmentManager = EquipmentManager.instance;
    }
    public override void OnButtonPress(){
        if(inventoryUI.GetMoveItem()!=null){
            SetItem(inventoryUI.GetMoveItem());
            inventoryUI.RemoveMoveItem();
            inventoryUI.UpdateUI();
        }
    }
    public override void UseItem(){
        item.Use();
        inventoryUI.RemoveFocusSlot();
    }
}

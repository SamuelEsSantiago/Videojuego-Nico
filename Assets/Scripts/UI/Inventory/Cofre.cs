using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cofre : MonoBehaviour
{
    public static Cofre instance;
    public List<Item> savedItems = new List<Item>();
    private void Awake() {
        if(instance!=null){
            Debug.Log("HOW!!!");
            return;
        }
        instance = this;
    }
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallBack;
    private void Start() {
        LoadSaveData();
    }
    public void LoadSaveData(){
        if(SaveFilesManager.instance?.currentSaveSlot != null){
            savedItems.Clear();
            if(SaveFilesManager.instance.currentSaveSlot.chestItems!=null){
                for(int i=0;i<SaveFilesManager.instance.currentSaveSlot.chestItems.Length;i++){
                    if(SaveFilesManager.instance.currentSaveSlot.chestItems[i] == null){
                        Debug.Log("Corrupted data");
                    }else{
                        SaveFilesManager.instance.currentSaveSlot.chestItems[i].ResetValues();
                        AddItem(SaveFilesManager.instance.currentSaveSlot.chestItems[i]);
                    }
                }
            }
        }
    }
    public void AddItem(Item newItem){
        savedItems.Add(newItem);
        if(onItemChangedCallBack != null){
            onItemChangedCallBack.Invoke();
        }
    }
    public void RemoveItem(Item oldItem){
        savedItems.Remove(oldItem);
        if(onItemChangedCallBack != null){
            onItemChangedCallBack.Invoke();
        }
    }
}

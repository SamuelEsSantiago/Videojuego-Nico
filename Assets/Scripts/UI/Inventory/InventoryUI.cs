using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    #region ObjectsUI
        public GameObject UI;
        [SerializeField] TextMeshProUGUI nametxt;
        [SerializeField] TextMeshProUGUI description;
        [SerializeField] TextMeshProUGUI typeText;
        [SerializeField] TextMeshProUGUI typeLabel;
        [SerializeField] GameObject menuDesplegable;
        public Transform slotsParent;
        public Button nextButton;
        public Button prevButton;
        [SerializeField] private GameObject InvSlotPrefab;
        #endregion

        #region variables
        public InventorySlot focusedSlot {get;set;}
        public static InventoryUI instance;
        List<InventorySlot> slots;
        [SerializeField] int slotsInPage;
        public int moveItemIndex;
        private int invPage;
    #endregion
    
    private void Awake() {
        if(instance!=null){
            Debug.Log("mmm esto podria ser malo?");
            return;
        }
        instance = this;
    }
    void Start()
    {
        Inventory.instance.onItemChangedCallBack += UpdateUI;
        invPage = 0;
        moveItemIndex = -1;
        menuDesplegable.SetActive(false);
        slots = new List<InventorySlot>();
        for(int i=0; i < slotsInPage; i++){
            GameObject obj = Instantiate(InvSlotPrefab);
            obj.transform.SetParent(slotsParent,false);
            InventorySlot slot = obj.GetComponent<InventorySlot>();
            slots.Add(slot);
        }
    }
    void Update()
    {
        if (PlayerManager.instance.inputs.enabled && PlayerManager.instance.inputs.OpenInventory)
        {
            focusedSlot = null;
            UpdateUI();
            UI.SetActive(!UI.activeSelf);
            MapUI.instance.mapUI.SetActive(false);
            AbilityUI.instance.UI.SetActive(false);
        }
       
    }
    public void NextPage(){
        invPage += slotsInPage;
        if(invPage > Inventory.instance.capacidad + slotsInPage){
            invPage -= slotsInPage;
            return;
        }
        UpdateUI();
    }
    public void PrevPage(){
        invPage -= slotsInPage;
        if(invPage < 0){
            invPage = 0;
            return;
        }
        UpdateUI();
    }
    public void UpdateUI(){
        for(int i=0; i < slotsInPage; i++){
            if((i+invPage) < Inventory.instance.items.Count){
                InventorySlot slot = slots[i];
                Item slotItem = Inventory.instance.items[i + invPage];
                slot.SetItem(slotItem);
                slot.SetIndex(i + invPage);
            }else{
                InventorySlot slot = slots[i];
                slot.SetItem(null);
                slot.SetIndex(i + invPage);
            }
            
        }

        //Next button
        if(invPage + slotsInPage < Inventory.instance.items.Count){
            nextButton.gameObject.SetActive(true);
        }
        else{
            nextButton.gameObject.SetActive(false);
        }

        //Prev button
        if(invPage!=0){
            prevButton.gameObject.SetActive(true);
        }
        else{
            prevButton.gameObject.SetActive(false);
        }
        UpdateText();
    }
    public void UpdateText(){
        nametxt.text = "";
        description.text = "";
        typeText.text = "";
        typeLabel.text = "";
        if(focusedSlot == null) return;
        nametxt.text = focusedSlot.GetItem().name;
        description.text = focusedSlot.GetItem().descripcion;
        typeLabel.text = "Tipo";
        typeText.text = focusedSlot.GetItem().type.ToString();
    }
    public void OpenDesMenu(InventorySlot slot){
        menuDesplegable.SetActive(true);
        MenuDes menuDes = menuDesplegable.GetComponent<MenuDes>();
        menuDes.onUse += slot.UseItem;
        menuDes.onMove += slot.MoveItem;
        menuDes.onLeave += slot.RemoveItem;
        menuDes.transform.position = Input.mousePosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PopUpHabUI : MonoBehaviour
{
    [HideInInspector] public Ability ability;
    [SerializeField] TextMeshProUGUI name_Ab;
    [SerializeField] TextMeshProUGUI desc_Ab;
    [SerializeField] GameObject groupHotkey;
    [SerializeField] TextMeshProUGUI isPasiveNotice;
    [SerializeField] TMP_Dropdown hotkey;
    public void UpdateUI(){
        if(ability == null) return;
        name_Ab.text = ability.abilityName.ToString();
        desc_Ab.text = ability.description;
        if(!ability.isPasive){
            groupHotkey.SetActive(true);
            isPasiveNotice.text = "Activa";
            hotkey.ClearOptions();
            TMP_Dropdown.OptionData currentHotKey = null;
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach(KeyCode key in PlayerManager.instance.inputs.skillHotkeys){
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = key.ToString();
                options.Add(option);
                if(key.ToString().Equals(ability.hotkey.ToString())){
                    currentHotKey = option;
                }
            }
            TMP_Dropdown.OptionData opt = new TMP_Dropdown.OptionData();
            opt.text = "Ninguna";
            options.Add(opt);
            hotkey.AddOptions(options);
            if(currentHotKey == null) {
                if(ability.hotkey.ToString() == "None"){
                    currentHotKey = opt;
                }else{
                    currentHotKey = hotkey.options.Find(x => x.text == ability.hotkey.ToString());
                }
            }
            hotkey.value = hotkey.options.IndexOf(currentHotKey);
            hotkey.onValueChanged.AddListener(updateAlgo);
        }
        else if(ability.isPasive){
            groupHotkey.SetActive(false);
            isPasiveNotice.text = "Pasiva";
        }
    }
    public void UpdateUI(Ability ability){
        this.ability = ability;
        name_Ab.text = ability.abilityName.ToString();
        desc_Ab.text = ability.description;
        if(!ability.isPasive){
            groupHotkey.SetActive(true);
            isPasiveNotice.text = "Activa";
            hotkey.ClearOptions();
            TMP_Dropdown.OptionData currentHotKey = null;
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach(KeyCode key in PlayerManager.instance.inputs.skillHotkeys){
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = key.ToString();
                options.Add(option);
                if(key.ToString().Equals(ability.hotkey.ToString())){
                    currentHotKey = option;
                }
            }
            TMP_Dropdown.OptionData opt = new TMP_Dropdown.OptionData();
            opt.text = "Ninguna";
            options.Add(opt);
            hotkey.AddOptions(options);
            if(currentHotKey == null) {
                if(ability.hotkey.ToString() == "None"){
                    currentHotKey = opt;
                }else{
                    currentHotKey = hotkey.options.Find(x => x.text == ability.hotkey.ToString());
                }
            }
            hotkey.value = hotkey.options.IndexOf(currentHotKey);
            hotkey.onValueChanged.AddListener(updateAlgo);
        }
        else if(ability.isPasive){
            groupHotkey.SetActive(false);
            isPasiveNotice.text = "Pasiva";
        }
    }
    public void GoBack(){
        gameObject.SetActive(false);
        string key = hotkey.options[hotkey.value].text;
        if(key == "Ninguna"){
            ability.hotkey = KeyCode.None;
        }else{
            ability.hotkey = (KeyCode) System.Enum.Parse(typeof(KeyCode),key);
        }
        
    }
    void updateAlgo(int i){
        string key = hotkey.options[hotkey.value].text;
        if(key == "Ninguna"){
            ability.hotkey = KeyCode.None;
        }else{
            KeyCode newHotkey = (KeyCode) System.Enum.Parse(typeof(KeyCode),key);
            foreach(Ability ab in AbilityManager.instance.abilities){
                if(ab.hotkey == newHotkey)
                ab.hotkey = KeyCode.None;
            }
            ability.hotkey = newHotkey;
        }
    }
}

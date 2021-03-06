using System;
using System.Collections.Generic;
using FinalProject.Assets.Scripts.Utils.Sound;
using UnityEngine;

public class BossFight : MonoBehaviour
{
    public static BossFight instance;
    [SerializeField] protected PopUpTrigger startMessageTrigger;
    [SerializeField] protected PopUpTrigger endMessageTrigger;
    [SerializeField] protected int levelToLoad;
    [SerializeField] protected WorldState worldState;
    [SerializeField] protected GameObject loadlevel;





    protected void Awake() {
        if (startMessageTrigger != null)
        {
            startMessageTrigger.popUpUI.exitButton.onClick.RemoveAllListeners();
            startMessageTrigger.popUpUI.exitButton.onClick.AddListener(ReturnToLastScene);
        }
        if(instance!=null){
            Debug.Log("this is bad : BossFight");
            return;
        }
        instance = this;



        //popUpTrigger = GetComponent<PopUpTrigger>();
    }
    [SerializeField] protected List<Stage> stages;
    
    [SerializeField] protected Ability.Abilities ability;

    public Stage currentStage;
    public bool isCleared;
    protected int indexStage;

    public Action BattleEnded = delegate{};

    

    protected void Start()
    {
        StartBattle();
        PlayerManager.instance.transform.position = loadlevel.GetComponent<loadlevel>().loadPosition.position;
        //AudioManager.instance?.Play("Theme");
        startMessageTrigger.popUpUI.closedPopUp -= InitializeLoad;
        startMessageTrigger.popUpUI.closedPopUp += InitializeLoad;
    }

    void InitializeLoad()
    {
        loadlevel?.SetActive(false);
    }

    protected void Update() {
        if(Input.GetKeyDown(KeyCode.L)){
            NextStage();
        }
        if (isCleared)
        {
            
        }
    }
    public virtual void NextStage(){
        if(indexStage<stages.Count-1){
            indexStage++;
            currentStage.Destroy();
            currentStage = stages[indexStage];
            currentStage.Generate();
        }
        else
        {
            /*Debug.Log("Lo hiciste ganaste!!!1");
            currentStage.Destroy();
            isCleared=true;*/
            //give ability
            //AbilityManager.instance.AddAbility(reward);
            EndBattle();
        }
    }

    public virtual void EndBattle()
    {
        if (!isCleared)
        {
            currentStage?.Destroy();
            isCleared=true;

            //Vector2 abilitySpawnPos = new Vector2(PlayerManager.instance.GetPosition().x, PlayerManager.instance.GetPosition().y + 5f);

            //Instantiate(abilityObject, abilitySpawnPos, abilityObject.transform.rotation);

            
            PlayerManager.instance.abilityManager.SetActiveSingle(ability, true);

            int i = 0;
            foreach(Ability ab in PlayerManager.instance.abilityManager.abilities){
                SaveFilesManager.instance.currentSaveSlot.unlockedAbilities[i] = ab.isUnlocked;
                i++;
            }
            
            endMessageTrigger.popUp.Message = ability.ToString();
            endMessageTrigger.TriggerPopUp(true);

            Debug.Log("Lo hiciste ganaste!!!1");
            SaveFile partida = SaveFilesManager.instance.currentSaveSlot;
            if(partida.WorldStates.Exists(x => x.id == worldState.id)){
                WorldState w = partida.WorldStates.Find(x => x.id == worldState.id);
                w.state = true;
            }else{
                worldState.state = true;
                partida.WorldStates.Add(worldState);
            }

            loadlevel?.SetActive(true);
            BattleEnded?.Invoke();
        }
    }
    public void StartBattle(){
        startMessageTrigger.TriggerPopUp(pauseGame: true);

        indexStage = 0;
        if (stages.Count > 0)
        {
            currentStage=stages[indexStage];
        }
        currentStage?.Generate();
    }

    public void ReturnToLastScene()
    {
        startMessageTrigger.popUpUI.Hide();
        SceneController.instance.LoadScene(loadlevel.GetComponent<loadlevel>().iLevelToLoad);
        Pause.ResumeGame();
    }
}

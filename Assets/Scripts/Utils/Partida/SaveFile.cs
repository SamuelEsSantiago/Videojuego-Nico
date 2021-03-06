using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveFile
{
    public int slotFile;
    public string namePlayer;
    public int timeSecondsPlayed;
    public int timeMinutesPlayed;
    public int timeHoursPlayed;
    public int sceneToLoad;
    public Vector2 positionSpawn;
    public List<WorldState> WorldStates;
    //public Dictionary<string, KeyCode> controlbinds;
    public List<string> controlBindsKeys;
    public List<KeyCode> controlBindsValues;
    public List<KeyCode> abilitiesBinds;
    public Item[] inventory;
    public int capacidad;
    public Item[] chestItems;
    public int money;
    public float staminaLimit;
    public bool[] unlockedAbilities;
    public float masterVol;
    public float hazardVol;
    public float musicVol;
    public int qualityIndex;
    public bool isFullScreen;
    public SaveFile(){
        this.slotFile = 0;
        this.namePlayer = "NoName";
        this.timeSecondsPlayed = 0;
        this.timeMinutesPlayed = 0;
        this.timeHoursPlayed = 0;
        this.sceneToLoad = 38;
        this.positionSpawn = new Vector2(-324f, 0f);
        this.capacidad = 20;
        this.inventory = null;
        this.chestItems = null;
        this.money = 0;
        this.WorldStates = new List<WorldState>();
        this.controlBindsKeys = new List<string>();
        this.controlBindsValues = new List<KeyCode>();
        this.staminaLimit = 100f;
        this.unlockedAbilities = new bool[15];
        abilitiesBinds = null;
        this.masterVol = 0;
        this.hazardVol = 0;
        this.musicVol = 0;
        this.qualityIndex = 0;
        this.isFullScreen = false;
    }
    public SaveFile(string namePlayer, int slotFile){
        this.namePlayer = namePlayer;
        this.slotFile = slotFile;
        this.timeSecondsPlayed = 0;
        this.timeMinutesPlayed = 0;
        this.timeHoursPlayed = 0;
        this.sceneToLoad = 38;
        this.positionSpawn = new Vector2(-324f, 0f);
        this.capacidad = 20;
        this.inventory = null;
        this.chestItems = null;
        this.money = 0;
        this.WorldStates = new List<WorldState>();
        this.controlBindsKeys = new List<string>();
        this.controlBindsValues = new List<KeyCode>();
        this.staminaLimit = 100f;
        this.unlockedAbilities = new bool[15];
        abilitiesBinds = null;
        this.masterVol = 0;
        this.hazardVol = 0;
        this.musicVol = 0;
        this.qualityIndex = 0;
        this.isFullScreen = false;
    }
}

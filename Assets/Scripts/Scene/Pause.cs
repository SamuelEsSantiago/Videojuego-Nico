using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public static bool active;
    public GameObject panel;
    public SettingsMenu settingsMenu;
    static PlayerInputs inputs;

    public static bool blocked;

    void Start()
    {
        inputs = PlayerManager.instance.gameObject.GetComponent<PlayerInputs>();
        active = false;
        panel.SetActive(true);
        settingsMenu.gameObject.SetActive(true);
        SaveFile partida = SaveFilesManager.instance.currentSaveSlot;
        settingsMenu.SetMasterVolume(partida.masterVol);
        settingsMenu.SetHazardVolume(partida.hazardVol);
        settingsMenu.SetMusicVolume(partida.musicVol);
        settingsMenu.SetQuality(partida.qualityIndex);
        settingsMenu.SetFullScreen(partida.isFullScreen);
        panel.SetActive(false);
        settingsMenu.gameObject.SetActive(false);
    }
    void Update()
    {
        if(Input.GetKeyDown(inputs.Pause) && !MinigameUI.instance.InMinigame && !blocked)
        {
            FindObjectOfType<MapUI>().mapUI.SetActive(false);
            //active = !active;
            HandleActive(!active);
            panel.SetActive(active);
            //Time.timeScale = (active) ? 0 : 1f;
            //inputs.enabled = !active;
        }
    }
    public static void PauseGame()
    {
        FindObjectOfType<InteractionUI>()?.gameObject.SetActive(false);
        HandleActive(true);
    }
    public static void ResumeGame()
    {
        HandleActive(false);
        FindObjectOfType<InteractionUI>()?.gameObject.SetActive(true);
        AbilityUI.instance.UpdateUI();
    }
    static void HandleActive(bool value)
    {
        active = value;
        Time.timeScale = (active) ? 0 : 1f;
        
        if (inputs != null)
        {
            inputs.enabled = !active;
        }

        PlayerManager.instance.SetEnabledPlayer(!active);

    }
    public void ReturnMenu(){
        ResumeGame();
        panel.SetActive(false);
        SceneController.instance.RealLoasScene(0);
        //Debug.Log("uSSop");
    }
}

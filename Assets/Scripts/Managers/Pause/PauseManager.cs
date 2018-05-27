﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : Colleague
{
    IBarManagerState storedBarState;

    [SerializeField]
    Button optionsButton;

    [SerializeField]
    Image optionsPanel;

    [SerializeField]
    Image optionsMenu;

    [SerializeField]
    Button quitButton;

    [SerializeField]
    Image pausePanel;

    private PauseScrollDownComponent pauseScrollDown;

    private void Start()
    {
        pauseScrollDown = this.gameObject.GetComponent<PauseScrollDownComponent>();
    }

    public void TogglePauseGame()
    {
        if (!pauseScrollDown.IsMenuDown)
            pauseScrollDown.MoveMenuDown();

        else
            pauseScrollDown.MoveMenuUp();
    }

    public void OpenExitGamePopUp()
    {
        pausePanel.gameObject.SetActive(true);
    }

    public void quitToMainMenu()
    {
        Mediator.GetRidOfObservers();
        SceneManager.LoadScene("TitleScreen");
    }

    public void ResumeGame()
    {
        pausePanel.gameObject.SetActive(false);
    }

    public void OpenOptionsMenu()
    {
        optionsPanel.gameObject.SetActive(true);
        optionsMenu.gameObject.SetActive(true);
        
    }

    public void CloseOptionsMenu()
    {
        optionsPanel.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(false);
    }

    public void StoreBarState(IBarManagerState stateToStore)
    {
        storedBarState = stateToStore;
    }

    public IBarManagerState getStoredBarState()
    {
        return storedBarState;
    }

}

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{
    [SerializeField]
    private Button QuitButton, StartButton, CreditsButton, BackButton;

    private List<Button> MainMenuButtons;

    [SerializeField]
    private GameObject loadingPanel, titlePanel, NamesInCredits;

    [SerializeField]
    private Image titleImage;

    [SerializeField]
    private float speedOfFade;

    private const float fadeOffeset = 0.1f;

    private enum AnimationStates { OPEN, FADING, CLOSED}
    private AnimationStates currentAnimationState;

    public void Start()
    {
        SetupButtonsList();
        setAnimationState(AnimationStates.OPEN);
    }

    private void setAnimationState(AnimationStates anaimationStateToSwapTo)
    {
        currentAnimationState = anaimationStateToSwapTo;
    }

    private void Update()
    {
        if (currentAnimationState == AnimationStates.FADING)
        {
            titlePanel.gameObject.GetComponent<Image>().color = Vector4.Lerp(titlePanel.gameObject.GetComponent<Image>().color, new Vector4(0, 0, 0, 255), speedOfFade * Time.deltaTime);
            titleImage.color = Vector4.Lerp(titleImage.color, new Vector4(0, 0, 0, 255), speedOfFade * Time.deltaTime);
            checkIfFadeHasEnded(titlePanel.gameObject.GetComponent<Image>().color.r + titlePanel.gameObject.GetComponent<Image>().color.b + titlePanel.gameObject.GetComponent<Image>().color.g);
        }
    }

    private void checkIfFadeHasEnded(float fadeColor)
    {
        if(fadeColor <= fadeOffeset)
        {
            setAnimationState(AnimationStates.CLOSED);
            loadingPanel.SetActive(true);
            SceneManager.LoadScene("Scencely");

        }
    }

    private void SetupButtonsList()
    {
        MainMenuButtons = new List<Button>();

        MainMenuButtons.Add(QuitButton);
        MainMenuButtons.Add(StartButton);
        MainMenuButtons.Add(CreditsButton);

        foreach (Button button in MainMenuButtons)
        {
            button.GetComponent<Animator>().SetTrigger("FadeIn");
        }
    }

    public void startGameButtonPressed()
    {
        foreach (Button button in MainMenuButtons)
        {
            button.GetComponent<Animator>().SetTrigger("FadeOut");
        }
        setAnimationState(AnimationStates.FADING);
    }
    
    public void FadeOutMainMenuButtonsAndOpenCredits()
    {
        foreach (Button button in MainMenuButtons)
        {
            button.GetComponent<Animator>().SetTrigger("FadeOut");
        }

        BackButton.GetComponent<Animator>().SetTrigger("FadeIn");

        NamesInCredits.GetComponent<Animator>().SetTrigger("FadeIn");
    }

    public void FadeInMainMenuButtonsAndCloseCredits()
    {
        StartCoroutine(PlayAndWaitForAnimation(NamesInCredits, "FadeOut"));
        StartCoroutine(PlayAndWaitForAnimation(BackButton.gameObject, "FadeOut"));

        foreach (Button button in MainMenuButtons)
        {
            button.GetComponent<Animator>().SetTrigger("FadeIn");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator PlayAndWaitForAnimation(GameObject target, string stateName)
    {
        int animLayer = 0;

        Animator anim = target.GetComponent<Animator>();
        anim.Play(stateName);

        while (anim.GetCurrentAnimatorStateInfo(animLayer).IsName(stateName) && anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f)
        {
            yield return null;
        }
    }

    //public void ResetCreditAnimation(string animationName)
    //{
    //    //Animation.Play lets me reset the animation to the first frame ("State", layer, normalizedTime)
    //    CreditsButton.GetComponent<Animator>().Play(animationName, -1, 0f);
    //}

}
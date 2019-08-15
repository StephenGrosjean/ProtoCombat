﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private List<Selectable> menuBtns;
    private int selectedIndex = 0;
    public bool inMenu = false;
    [SerializeField] private bool inGameMenu;

    // Update is called once per frame
    void Update()
    {
        if (GameInput.GetInputDown(GameInput.InputType.PAUSE))
            inMenu = !inMenu;

        if (inMenu || !inGameMenu)
            ManageNavigation();
    }

    public void SetupMenuBtns(Transform parent)
    {
        menuBtns.Clear();
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).GetComponent<Selectable>())
            {
                menuBtns.Add(parent.GetChild(i).GetComponent<Selectable>());
            }
        }
        if(menuBtns.Count > 0)
            menuBtns[0].Select();
    }


    private void ManageNavigation()
    {
        if (GameInput.GetInputDown(GameInput.InputType.DOWN) || GameInput.GetInputDown(GameInput.InputType.LEFT))
            NavigateMenu(false);
        if (GameInput.GetInputDown(GameInput.InputType.UP) || GameInput.GetInputDown(GameInput.InputType.RIGHT))
            NavigateMenu(true);

        if (GameInput.GetInputDown(GameInput.InputType.ACTION_CONFIRM) && !Input.GetMouseButtonDown(0))
            SubmitButtonAction();
    }

    public void NavigateMenu(bool onLeft)
    {
        if (inMenu)
        {
            if (onLeft)
            {
                selectedIndex--;
                if (selectedIndex == -1)
                    selectedIndex = 0;
                //else
                //    SoundManager._instance.PlaySound(SoundManager.SoundList.MENU_SELECTION);
            }
            else
            {
                selectedIndex++;
                if (selectedIndex == menuBtns.Count)
                    selectedIndex = menuBtns.Count - 1;
                //else
                //    SoundManager._instance.PlaySound(SoundManager.SoundList.MENU_SELECTION);
            }
            if(menuBtns.Count > 0)
                menuBtns[selectedIndex].Select();
        }
    }

    public void SelectButton(string name)
    {
        int newIndex = 0;
        foreach (Selectable btn in menuBtns)
        {
            if (btn.name == name)
            {
                selectedIndex = newIndex;
                break;
            }
            newIndex++;
        }
        menuBtns[selectedIndex].Select();
    }

    public void SubmitButtonAction()
    {
        menuBtns[selectedIndex].GetComponent<Button>().onClick.Invoke();
    }

    
}
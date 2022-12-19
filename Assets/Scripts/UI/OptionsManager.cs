using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private List<Button> Player1Buttons;
    [SerializeField] private List<Button> Player2Buttons;

    private void Awake()
    {
        SelctAI1(1);
        SelctAI2(1);
        ChangeButtonColor(Player1Buttons[0]);
        ChangeButtonColor(Player2Buttons[0]);
    }

    public void SelctAI1(int AI_ID)
    {
        OptionsSettings.SelectedAI1 = AI_ID;
    }
    
    public void SelctAI2(int AI_ID)
    {
        OptionsSettings.SelectedAI2 = AI_ID;
    }

    public void ChangeButtonColor(Button pressed)
    {
        var isPlayer1 = false;
        foreach (Button button in Player1Buttons)
        {
            if (button == pressed) isPlayer1 = true;
        }

        if (isPlayer1)
        {
            foreach (Button button in Player1Buttons)
            {
                var buttonBaseColors = button.colors;
                Color notSelected = Color.white;
                buttonBaseColors.normalColor = notSelected;
                buttonBaseColors.selectedColor = notSelected;
                button.colors = buttonBaseColors;
            }
        }
        else
        {
            foreach (Button button in Player2Buttons)
            {
                var buttonBaseColors = button.colors;
                Color notSelected = Color.white;
                buttonBaseColors.normalColor = notSelected;
                buttonBaseColors.selectedColor = notSelected;
                button.colors = buttonBaseColors;
            }
        }
        var pressedColors = pressed.colors;
        Color selected = Color.green;
        pressedColors.normalColor = selected;
        pressedColors.selectedColor = selected;
        pressed.colors = pressedColors;
    }
}

public static class OptionsSettings
{
    public const int HUMAN = 0;
    public const int DECISIONTREE = 1;
    public const int GNB = 2;
    public const int DINAMICSTATES = 3;
    public const int OSLA = 4;
    public const int FSM = 5;
    public static int SelectedAI1 { get; set; }
    public static int SelectedAI2 { get; set; }
}

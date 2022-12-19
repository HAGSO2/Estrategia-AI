using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionsSetter : MonoBehaviour
{
    [Tooltip("Add the AIs in the same order as the settings page:" +
        "\nHuman \nDecisionTree \nGNB ")]
    public GameObject[] rightPlayer;
    [Tooltip("Add the AIs in the same order as the settings page:" +
        "\nDecisionTree \nGNB ")]
    public GameObject[] leftPlayer;

    [SerializeField] TextMeshProUGUI player1;
    [SerializeField] TextMeshProUGUI player2;

    // Activate the AIs according to the settings of the player
    void Awake()
    {
        rightPlayer[OptionsSettings.SelectedAI1].SetActive(true);
        leftPlayer[OptionsSettings.SelectedAI2].SetActive(true);

        player1.text = " " + rightPlayer[OptionsSettings.SelectedAI1].name;
        player2.text = leftPlayer[OptionsSettings.SelectedAI2].name;
    }
}

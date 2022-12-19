using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGameScript : MonoBehaviour
{
    [SerializeField] GameObject endGameUI;
    [SerializeField] TextMeshProUGUI endGameText;


    public void EndGame(bool team)
    {
        endGameUI.SetActive(true);

        if (!team)
            endGameText.text = "Game Over\nLeft Player Wins";
        else
            endGameText.text = "Game Over\nRight Player Wins";

        Invoke("BackToMenu", 5f);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}

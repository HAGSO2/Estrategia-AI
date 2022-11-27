using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _pauseMenu;
    [SerializeField]
    private GameObject _pauseButton;
    
    public static bool IsPaused = false;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _pauseMenu.SetActive(!_pauseMenu.activeSelf); 
            _pauseButton.SetActive(!_pauseButton.activeSelf);
            IsPaused = !IsPaused;
        }
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

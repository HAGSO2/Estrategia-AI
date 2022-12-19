using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class MenuManager : MonoBehaviour
{
    /*
    [SerializeField] private Image Option2;
    [SerializeField] private Image Option3;
    [SerializeField] private Image Option4;
    [SerializeField] private Image Option5;
    [SerializeField] private Image Option6;
    */
    [SerializeField] private List<Image> _options;
    [SerializeField] private Image _actualOption;

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }

    private void Start()
    {
        StartCoroutine(nameof(ChangeImages));
    }
    
    private IEnumerator ChangeImages()
    {
        var antIndex = 0;
        while (true)
        {
            yield return new WaitForSeconds(2f);
            var random = new Random();
            var index = 0;
            do 
            {
                index = random.Next(_options.Count);
            } while (antIndex == index);
            antIndex = index;
            StartCoroutine(DoTransiction(index));
            
        }
    }

    private IEnumerator DoTransiction(int n)
    {
        const float stepSize = 0.01f;
        for (var i = 0; i < 1/stepSize; i++)
        {
            var actualOptionColor = _actualOption.color;
            actualOptionColor.a -= stepSize;
            _actualOption.color = actualOptionColor;
        
            var optionColor = _options[n].color;
            optionColor.a += stepSize;
            _options[n].color = optionColor;

            yield return new WaitForSeconds(stepSize);
        }

        _actualOption = _options[n];
    }
}
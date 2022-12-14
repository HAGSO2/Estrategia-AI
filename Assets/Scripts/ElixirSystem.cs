using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElixirSystem : MonoBehaviour
{
    [SerializeField] GameObject elixirGameObject;
    [SerializeField] float elixirWaitTime = 1f;
    TextMeshProUGUI elixirTMP;

    public int elixir = 6;
    int maxElixir = 10;

    bool isAI = false;

    void Start()
    {
        if (elixirGameObject == null)
            isAI = true;
        else
            elixirTMP = elixirGameObject.GetComponent<TextMeshProUGUI>();

        StartCoroutine(IncreaseElixir());
    }

    IEnumerator IncreaseElixir()
    {
        while (true)
        {
            if (!isAI)
                UpdateText();
            yield return new WaitForSeconds(elixirWaitTime);
            if (elixir < maxElixir)
                elixir++;
        }
    }

    public void UpdateText()
    {
        elixirTMP.text = "Elixir: " + " " + elixir;
    }
}

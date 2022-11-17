using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElixirSystem : MonoBehaviour
{
    [SerializeField] GameObject elixirGameObject;
    TextMeshProUGUI elixirTMP;

    public float elixir = 6;
    float maxElixir = 10;

    void Start()
    {
        elixirTMP = elixirGameObject.GetComponent<TextMeshProUGUI>();

        StartCoroutine(IncreaseElixir());
    }

    IEnumerator IncreaseElixir()
    {
        while (true)
        {
            UpdateText();
            yield return new WaitForSeconds(1);
            if (elixir < maxElixir)
                elixir++;
        }
    }

    public void UpdateText()
    {
        elixirTMP.text = "Elixir: " + " " + elixir;
    }
}

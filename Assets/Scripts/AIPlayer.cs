using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    CardSystem cardSystem;

    float maxTime = 2f;
    float timer;

    void Start()
    {
        timer = maxTime;
        cardSystem = GetComponent<CardSystem>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= maxTime)
        {
            cardSystem.TryToPlayCardAI(Random.Range(0, 3));
            timer = 0f;
        }
    }

    
}

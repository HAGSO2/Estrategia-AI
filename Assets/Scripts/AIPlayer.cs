using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    CardSystem cardSystem;

    [SerializeField] Transform minCardPosToBePlayed;
    [SerializeField] Transform maxCardPosToBePlayed;

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
            cardSystem.TryToPlayCardAI(Random.Range(0, 3), RandomPositionBetween(minCardPosToBePlayed.position, maxCardPosToBePlayed.position));
            timer = 0f;
        }
    }

    private Vector3 RandomPositionBetween(Vector3 minPos, Vector3 maxPos)
    {
        Vector3 result = Vector3.zero;

        result.x = Random.Range(minPos.x, maxPos.x);
        result.y = Random.Range(minPos.y, maxPos.y);
        //result.z = 0;

        return result;
    }
}

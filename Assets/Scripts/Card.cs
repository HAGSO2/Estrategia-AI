using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    const int MAX_X = 11;
    const int MIN_X = -11;
    const int MAX_Y = 5;
    const int MIN_Y = -5;

    public GameObject enemy;
    public int cost;
    public int handIndex;

    public bool canBePlayed;
    public bool rightClick = false;

    Vector3 scaleUp;
    Vector3 scaleDown;

    private void Start()
    {
        scaleUp = transform.localScale + new Vector3(0.2f, 0.2f, 0);
        scaleDown = transform.localScale;
    }

    private void OnMouseEnter()
    {
        transform.localScale = scaleUp;
    }

    private void OnMouseExit()
    {
        transform.localScale = scaleDown;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            canBePlayed = true;
        }
        else
        {
            canBePlayed = false;
        }
    }
}

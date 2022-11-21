using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{

    public GameObject enemy;
    public int cost;
    public int handIndex;

    public bool canBePlayed;

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

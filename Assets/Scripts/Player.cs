using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CardSystem cardSystem;

    bool IsCardMoving = false;
    bool rightClickSelection = false;
    Transform selectedCard;
    void Start()
    {
        cardSystem = GetComponent<CardSystem>();
    }

    // Podria cambiar el dibujo de carta a soldado cuando la carta esta en el campo de juego
    void Update()
    {
        if (IsCardMoving && Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = -2;

            selectedCard.localPosition = pos;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Card"))
            {
                selectedCard = hit.collider.transform;
                rightClickSelection = true;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (rightClickSelection)
            {
                rightClickSelection = false;
                Card card = selectedCard.GetComponent<Card>();
                card.rightClick = true;
                cardSystem.TryToPlayCard(card.handIndex);
            }
            else if(!IsCardMoving)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.CompareTag("Card"))
                {
                    selectedCard = hit.collider.transform;
                    IsCardMoving = true;
                }
            }
             
        }
        else if (Input.GetMouseButtonUp(0) && IsCardMoving)
        {
            IsCardMoving = false;
            cardSystem.TryToPlayCard(selectedCard.GetComponent<Card>().handIndex);
        }
    }
}

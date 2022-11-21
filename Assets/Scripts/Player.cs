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
    // Podria indicarse mejor cuando se tiene seleccionada una carta con el click derecho
    // Es necesario un doble click izquierdo para poder mover una carta si antes se ha usado click derecho
    void Update()
    {
        if (IsCardMoving && Input.GetMouseButton(0)) // Left click being pressed
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = -2;

            selectedCard.localPosition = pos;
        }
        else if (Input.GetMouseButtonDown(1)) // Rigth click down
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.CompareTag("Card"))
            {
                selectedCard = hit.collider.transform;
                rightClickSelection = true;
            }
        }
        else if (Input.GetMouseButtonDown(0)) // Left click down
        {
            if (rightClickSelection)
            {
                rightClickSelection = false;
                Card card = selectedCard.GetComponent<Card>();
                cardSystem.TryToPlayCard(card.handIndex, Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
        else if (Input.GetMouseButtonUp(0) && IsCardMoving) // Left click up
        {
            IsCardMoving = false;
            cardSystem.TryToPlayCard(selectedCard.GetComponent<Card>().handIndex, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}

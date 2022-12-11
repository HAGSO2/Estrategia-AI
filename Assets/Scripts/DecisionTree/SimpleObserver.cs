using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObserver : MonoBehaviour
{
    [SerializeField] ElixirSystem elixirSystem;
    [SerializeField] CardSystem cardSystem;

    [HideInInspector] public Queue<bool> sides;
    [HideInInspector] public List<Card> cardsByCost = new List<Card>(4);

    [HideInInspector] public int lowestCardCost;

    private void Start()
    {
        for (int i = 0; i < cardSystem.hand.Length; i++)
            cardsByCost[i] = cardSystem.hand[i];

        sortCards();
        lowestCardCost = cardsByCost[3].cost;

        //DEBUGGING
        for (int i = 0; i < cardSystem.hand.Length; i++)
            Debug.Log(cardsByCost[i].cost);
    }

    // Hacer lo de la cola !!!
    void EnemyUpSide()
    {
        sides.Enqueue(true);
    }

    void EnemyDownSide()
    {
        sides.Enqueue(false);
    }

    public void sortCards()
    {
        cardsByCost.Sort(CompareCardByCost);
    }

    public int ActualElixir()
    {
        return elixirSystem.elixir;
    }

    public void UpSide()
    {
        sides.Enqueue(true);
    }

    public void DownSide()
    {
        sides.Enqueue(false);
    }

    static int CompareCardByCost(Card c1, Card c2)
    {
        if (c1.cost > c2.cost)
            return 1;

        return -1;
    }
}

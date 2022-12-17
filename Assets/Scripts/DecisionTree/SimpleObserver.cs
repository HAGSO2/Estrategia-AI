using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObserver : MonoBehaviour
{
    [SerializeField] ElixirSystem elixirSystem;
    [SerializeField] CardSystem cardSystem;
    [SerializeField] int enemyLayer;

    [HideInInspector] public Queue<bool> sides = new Queue<bool>();
    [HideInInspector] public List<Card> cardsByCost = new List<Card>(4);

    [HideInInspector] public int lowestCardCost;

    [HideInInspector] public bool terminal = false;

    private void Start()
    {
        for (int i = 0; i < cardSystem.hand.Length; i++)
            cardsByCost.Add(cardSystem.hand[i]);

        SortCards();
        lowestCardCost = cardsByCost[0].cost;

        /*
        //DEBUGGING
        for (int i = 0; i < cardSystem.hand.Length; i++)
            Debug.Log(cardsByCost[i].cost);
        */
    }

    public bool IsTerminal()
    {
        return terminal;
    }

    // Falta gestionar la entrada de booleanos a la cola!!
    // ¿Cómo detectar que el enemigo ha jugado?
    // ¿Dónde ha jugado? --> Posicion y respecto 0
    void EnemyUpSide()
    {
        //Debug.Log("Enemy up side");
        sides.Enqueue(true);
    }

    void EnemyDownSide()
    {
        //Debug.Log("Enemy down side");
        sides.Enqueue(false);
    }

    public void SortCards()
    {
        cardsByCost = new List<Card>(4);
        for (int i = 0; i < cardSystem.hand.Length; i++)
            cardsByCost.Add(cardSystem.hand[i]);

        cardsByCost.Sort(CompareCardByCost);

        lowestCardCost = cardsByCost[0].cost;
    }

    public int ActualElixir()
    {
        return elixirSystem.elixir;
    }

    public void Defended()
    {
        sides.Dequeue();
    }

    static int CompareCardByCost(Card c1, Card c2)
    {
        if (c1.cost > c2.cost)
            return 1;

        return -1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == enemyLayer)
        {
            if (collision.transform.position.y > 0)
                EnemyUpSide();
            else
                EnemyDownSide();
        }
    }
}

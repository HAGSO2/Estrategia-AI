using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObserver : MonoBehaviour
{
    [SerializeField] ElixirSystem elixirSystem;
    [SerializeField] CardSystem cardSystem;
    [SerializeField] int enemyLayer;

    [HideInInspector] public Queue<bool> sides = new Queue<bool>();
    [HideInInspector] public Queue<string> names = new Queue<string>();
    [HideInInspector] public Queue<int> costs = new Queue<int>();
    [HideInInspector] public List<Card> cardsByCost = new List<Card>(4);

    [HideInInspector] public int lowestCardCost;

    [HideInInspector] public bool terminal = false;

    [HideInInspector] public string playedCardName;

    private void Start()
    {
        for (int i = 0; i < cardSystem.hand.Length; i++)
            cardsByCost.Add(cardSystem.hand[i]);

        SortCards();

        /*
        //DEBUGGING
        for (int i = 0; i < cardSystem.hand.Length; i++)
            Debug.Log(cardsByCost[i].cost);
        */
    }

    public int CheckForCard(string card_name)
    {
        for (int i = 0; i < cardSystem.hand.Length; i++)
        {
            if (cardSystem.hand[i].name == card_name)
            {
                playedCardName = card_name;
                return i;
            }
        }

        int cardIndex = Random.Range(0, cardSystem.hand.Length);
        playedCardName = cardSystem.hand[cardIndex].name;

        return cardIndex;
    }

    public bool IsTerminal()
    {
        return terminal;
    }

    void EnemyUpSide()
    {
        sides.Enqueue(true);
    }

    void EnemyDownSide()
    {
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

    public void PositionDefended()
    {
        sides.Dequeue();
    }

    public void NamesDefended()
    {
        names.Dequeue();
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
            NPC enemy = collision.GetComponent<NPC>();
            names.Enqueue(enemy.atributes.name);
            costs.Enqueue(enemy.atributes.elixirCost);
            if (collision.transform.position.y > 0)
                EnemyUpSide();
            else
                EnemyDownSide();
        }
    }
}

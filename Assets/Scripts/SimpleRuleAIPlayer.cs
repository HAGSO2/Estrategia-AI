using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRuleAIPlayer : MonoBehaviour, IAI
{
    public string id { get; set; }
    
    [SerializeField] private Observer _observer;
    [SerializeField] private CardSystem _cardSystem;
    [SerializeField] private ElixirSystem _elixirSystem;

    [SerializeField] private Transform upAttackTransform;
    [SerializeField] private Transform upDefenseTransform;
    [SerializeField] private Transform downAttackTransform;
    [SerializeField] private Transform downDefenseTransform;
    

    public int player;
    private int rival;
    private Vector3 myTowerPos;
    
    // Start is called before the first frame update
    void Start()
    {
        rival = player == 0 ? 1 : 0;
        myTowerPos = player == 0 ? _observer.Player1KingTower.transform.position : _observer.Player2KingTower.transform.position;
        StartCoroutine(nameof(UpdateAI));
    }

    private IEnumerator UpdateAI()
    {
        while (_observer.timeLeft >=0 && _observer.Player1KingTower.health > 0 && _observer.Player2KingTower.health >0)
        {
            Debug.Log("Pensando");
            int index = think(_observer, 1f);
            yield return new WaitForSeconds(1f);
        }
    }
    
    public int think(Observer observer, float budget)
    {
        /*
        Debug.Log("Hand: " + _cardSystem.hand[0].name +
                  " : " + _cardSystem.hand[1].name +
                  " : " + _cardSystem.hand[2].name +
                  " : " + _cardSystem.hand[3].name +
                  " Elixir: " + _observer.player2Elixir);
                  */

        // Elixir 10, tirar (carta de mayor elixir) defensa
        if (ElixirHigherOrEcuals(10))
        {
            DondeCaemosGente(MaxCardWithLimit(10), false, false);
        }
        else if (ThereIsAnEnemy())
        {
            Debug.Log("Hay Enemigo");
            GameObject enemy = GetRivalEnemy();
            bool enemyUp = IsRivalEnemyUp(enemy);
            
            // Enemigo muy cerca, (carta de mayor elixir) defensa 
            if (EnemyCloserThan(enemy , 3))
            {
                DondeCaemosGente(MaxCardWithLimit(10), false, enemyUp);
            }
            // Enemigo es rango largo -> tirar (la de menos rango) ataque
            else if (EnemyRangeIsHigherThan(enemy, 7))
            {
                DondeCaemosGente(LessRangeCard(), true, enemyUp);
            }
            // Enemigo es rango corto -> tirar (la de más rango) defensa
            else
            {
                DondeCaemosGente(MostRangeCard(), false, enemyUp);
            }
        }
        
        return 1;
    }

    private GameObject GetRivalEnemy()
    {
        _observer.TroopsInField(rival);
        return _observer.playersTroops[rival][0];
    }

    private Card MostRangeCard()
    {
        Card finalCard = _cardSystem.hand[0];
        float maxRange = 1;
        foreach (Card card in _cardSystem.hand)
        {
            float newRange = card.enemy.GetComponent<NPC>().atributes.attackRange;
            if (newRange <= maxRange) continue;
            maxRange = newRange;
            finalCard = card;
        }

        return finalCard;
    }

    private Card LessRangeCard()
    {
        Card finalCard = _cardSystem.hand[0];
        float minRange = 10000;
        foreach (Card card in _cardSystem.hand)
        {
            float newRange = card.enemy.GetComponent<NPC>().atributes.attackRange;
            if (newRange > minRange) continue;
            minRange = newRange;
            finalCard = card;
        }

        return finalCard;
    }

    private bool EnemyRangeIsHigherThan(GameObject enemy, int range)
    {
        return enemy.GetComponent<NPC>().atributes.attackRange > range;
    }

    private bool EnemyCloserThan(GameObject enemy, int distance)
    {
        return Math.Abs(Vector3.Distance(enemy.transform.position, myTowerPos)) < distance;
    }

    private bool ThereIsAnEnemy()
    {
        _observer.TroopsInField(rival);
        Debug.Log("Enemigos: " + _observer.playersTroops[rival].Length);
        return _observer.playersTroops[rival].Length > 0;
    }

    private bool IsRivalEnemyUp(GameObject enemy)
    {
        return enemy.transform.position.y > 0;
    }


    private void DondeCaemosGente(Card card, bool Attack, bool Up)
    {
        if (Attack)
        {
            _cardSystem.AlejandroTryToPlayCard(card, Up ? upAttackTransform.position : downAttackTransform.position, 
                player == 0);
        }
        else
        {
            _cardSystem.AlejandroTryToPlayCard(card, Up ? upDefenseTransform.position : downDefenseTransform.position,
                player == 0);
        }
    }

    private bool ElixirHigherOrEcuals(int elixir)
    {
        return _elixirSystem.elixir >= elixir;
    }

    private Card MaxCardWithLimit(float limit)
    {
        Card finalCard = _cardSystem.hand[0];
        int maxCost = 1;
        foreach (Card card in _cardSystem.hand)
        {
            int newCost = card.cost;
            if (newCost <= maxCost || newCost < limit) continue;
            maxCost = newCost;
            finalCard = card;
        }

        return finalCard;
    }
}

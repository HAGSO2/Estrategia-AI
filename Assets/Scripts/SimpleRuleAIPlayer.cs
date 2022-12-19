using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRuleAIPlayer : MonoBehaviour, IAI
{
    public string id { get; set; }
    
    [SerializeField] private Observer _observer;
    [SerializeField] private CardSystem _cardSystem;

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
            int index = think(_observer, 1f);
            yield return new WaitForSeconds(2f);
        }
    }
    
    public int think(Observer observer, float budget)
    {
        Card cardToDeploy;
        if ((_observer.player1Elixir >= 10 && player == 0) || (_observer.player2Elixir >= 10 && player == 1))
        {
            Debug.Log("Límite Elixir");
            cardToDeploy = MaxCardWithLimit(10);
            _cardSystem.AlejandroTryToPlayCard(cardToDeploy, downDefenseTransform.position, player == 0);
        }
        
        _observer.TroopsInField(rival);
        if (_observer.playersTroops[rival].Length <= 0) return 1;
        
        foreach (GameObject enemyTroop in _observer.playersTroops[rival])
        {
            if (enemyTroop == null) continue;
            
            // Si hay una carta muy cerca tiramos una carta rápida de defensa
            if (Math.Abs(Vector3.Distance(enemyTroop.transform.position, myTowerPos)) < 3)
            {
                Debug.Log("Muy CERCA ataca");
                cardToDeploy = player == 0 ? MaxCardWithLimit(_observer.player1Elixir) : MaxCardWithLimit(_observer.player2Elixir);
                Vector3 pos = enemyTroop.transform.position.y < 0 ? downDefenseTransform.position : upDefenseTransform.position;
                _cardSystem.AlejandroTryToPlayCard(cardToDeploy, pos, player == 0);
                return 1;
            }
            
            //Rango corto, tirar carta de rango largo
            if (enemyTroop.GetComponent<NPC>().atributes.attackRange < 7)
            {
                Debug.Log("Rango CORTO ataca");
                Debug.Log("Nombre: " + enemyTroop.name + ", Rango: " + enemyTroop.GetComponent<NPC>().atributes.attackRange);
                foreach (Card card in _cardSystem.hand)
                {
                    if (!(card.enemy.GetComponent<NPC>().atributes.visionRange > 7)) continue;
                    cardToDeploy = card;

                    Vector3 pos = enemyTroop.transform.position.y < 0 ? downDefenseTransform.position : upDefenseTransform.position;
                    _cardSystem.AlejandroTryToPlayCard(cardToDeploy, pos, player == 0);
                    Debug.Log("Carta: " + cardToDeploy.name + "Pos: " + pos);
                    return 1;
                }
            }
            //Rango largo, tirar carta de rango corto
            else
            {
                Debug.Log("Rango LARGO ataca");
                Debug.Log("Nombre: " + enemyTroop.name + ", Rango: " + enemyTroop.GetComponent<NPC>().atributes.attackRange);
                foreach (Card card in _cardSystem.hand)
                {
                    if (!(card.enemy.GetComponent<NPC>().atributes.visionRange < 7)) continue;
                    cardToDeploy = card;
                    
                    Debug.Log("La Y del enemigo: " + card.transform.position.y);
                    Vector3 pos = card.transform.position.y < 0 ? downAttackTransform.position : upAttackTransform.position;
                    _cardSystem.AlejandroTryToPlayCard(cardToDeploy, pos, player == 0);
                    Debug.Log("Carta: " + cardToDeploy.name + "Pos: " + pos);
                    return 1;
                }
            }
        }
        return 1;
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

    private Vector3 DondeCaemosGente(string troopName)
    {
        _observer.TroopsInField(rival);
        if (_observer.playersTroops[rival].Length < 1) return Vector3.zero;
        
        foreach (GameObject troop in _observer.playersTroops[rival])
        {
            if (troop == null) continue;
            if (troop.name == troopName)
            {
                return troop.transform.position;
            }
        }
        return Vector3.zero;
    }
}

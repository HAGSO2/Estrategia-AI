using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Observer : MonoBehaviour
{
    
    public Card[] player1Hand = new Card[4];
    public Card[] player2Hand = new Card[4];
    private List<Card[]> _playersHand = new List<Card[]>(2);

    public Queue<Card> player1Deck = new Queue<Card>(4);
    public Queue<Card> player2Deck = new Queue<Card>(4);
    private List<Queue<Card>> _playersDeck = new List<Queue<Card>>(2);

    // Make sure that the spawned troops will have the NPC component
    public GameObject[] player1Troops;
    public GameObject[] player2Troops;
    private List<GameObject[]> _playersTroops = new List<GameObject[]>(2);
    public GameObject player1TroopsParent;
    public GameObject player2TroopsParent;
    private List<GameObject> _playersTroopsParents = new List<GameObject>(2);

    // Probably the elixir here will need to be changed to adapt it to the ElixirSystem class
    public float player1Elixir;
    public float player2Elixir;
    private List<float> _playersElixir = new List<float>(2);
    
    public float player1BurnedElixirInLastSimulation;
    public float player2BurnedElixirInLastSimulation;

    public Tower Player1KingTower;
    public Tower Player2KingTower;

    public float timeLeft;

    private void Awake()
    {
        // -------------------------------------------------------------------------------------- //
        // Make sure that what we are adding here are references and we don't need to update them //
        // -------------------------------------------------------------------------------------- //
        _playersHand.Add(player1Hand);
        _playersHand.Add(player2Hand);
        
        _playersDeck.Add(player1Deck);
        _playersDeck.Add(player2Deck);
        
        _playersTroops.Add(player1Troops);
        _playersTroops.Add(player2Troops);
        
        _playersTroopsParents.Add(player1TroopsParent);
        _playersTroopsParents.Add(player2TroopsParent);
        
        _playersElixir.Add(player1Elixir);
        _playersElixir.Add(player2Elixir);
    }

    // Returns a List with the cards that can be played when called, the int player must be 0 or 1
    public List<Card> AviableTroops(int player)
    {
        if (player < 0) player = 0;
        else if (player > 1) player = 1;
        var aviableTroops = new List<Card>();

        aviableTroops.AddRange(_playersHand[player].Where(troop => troop.cost <= player1Elixir));
        return aviableTroops;
    }

    // Updates the variable "playerXTroops" to the troops in the field when called, the int player must be 0 or 1
    public void TroopsInField(int player)
    {
        if (player < 0) player = 0;
        else if (player > 1) player = 1;

        _playersTroops[player] = _playersTroopsParents[player].GetComponentsInChildren<GameObject>();

    }
/*
    public Observer Clone()
    {
        return gameObject.AddComponent<Observer>();
    }
    */

    
}

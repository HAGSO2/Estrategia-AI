using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class Observer : MonoBehaviour
{
    [SerializeField] private OptionsSetter _optionsSetter;
    
    [SerializeField] private CardSystem CSP1;
    [SerializeField] private CardSystem CSP2;

    #region VariablesDeclarations
    
    public Card[] player1Hand
    {
        get { return CSP1.hand; }
        set { CSP1.hand = value; }
    }
    public Card[] player2Hand
    {
        get { return CSP1.hand; }
        set { CSP2.hand = value; }
    }
    private List<Card[]> _playersHand = new List<Card[]>(2);

    public Queue<Card> player1Deck
    {
        get { return CSP1.deck; }
        set { CSP1.deck = value; }
    }
    public Queue<Card> player2Deck
    {
        get { return CSP2.deck; }
        set { CSP2.deck = value; }
    }
    private List<Queue<Card>> _playersDeck = new List<Queue<Card>>(2);

    // Make sure that the spawned troops will have the NPC component
    private GameObject[] player1Troops;
    private GameObject[] player2Troops;
    public List<GameObject[]> playersTroops = new List<GameObject[]>(2);
    
    public GameObject player1TroopsParent;
    public GameObject player2TroopsParent;
    private List<GameObject> _playersTroopsParents = new List<GameObject>(2);

    [SerializeField] private ElixirSystem ESP1;
    [SerializeField] private ElixirSystem ESP2;
    
    public float player1Elixir
    {
        get { return ESP1.elixir; }
        set { ESP1.elixir = (int)value; }
    }
    public float player2Elixir
    {
        get { return ESP2.elixir; }
        set { ESP2.elixir = (int)value; }
    }
    [HideInInspector] public List<float> playersElixir = new List<float>(2);
    
    [HideInInspector] public float player1BurnedElixirInLastSimulation;
    [HideInInspector] public float player2BurnedElixirInLastSimulation;

    public Tower Player1KingTower;
    public Tower Player2KingTower;

    public float timeLeft = 180;
    [SerializeField] private TextMeshProUGUI timeLeftTMP;

    public bool isSimulator;
    
    #endregion

    private void Awake()
    {
        // -------------------------------------------------------------------------------------- //
        // Make sure that what we are adding here are references and we don't need to update them //
        // -------------------------------------------------------------------------------------- //

        CSP1 = _optionsSetter.leftPlayer[OptionsSettings.SelectedAI1].GetComponent<CardSystem>();
        CSP2 = _optionsSetter.rightPlayer[OptionsSettings.SelectedAI2].GetComponent<CardSystem>();
        
        ESP1 = _optionsSetter.leftPlayer[OptionsSettings.SelectedAI1].GetComponent<ElixirSystem>();
        ESP2 = _optionsSetter.rightPlayer[OptionsSettings.SelectedAI2].GetComponent<ElixirSystem>();
        
        _playersHand.Add(player1Hand);
        _playersHand.Add(player2Hand);
        
        _playersDeck.Add(player1Deck);
        _playersDeck.Add(player2Deck);
        
        playersTroops.Add(player1Troops);
        playersTroops.Add(player2Troops);
        
        _playersTroopsParents.Add(player1TroopsParent);
        _playersTroopsParents.Add(player2TroopsParent);

        if(!isSimulator) StartCoroutine("ClashTimer");

        /*
        _playersElixir.Add(player1Elixir);
        _playersElixir.Add(player2Elixir);
        */
    }

    // Returns a List with the cards that can be played when called, the int player must be 0 or 1
    public List<Card> AvailableTroops(int player)
    {
        player = player <= 0 ? 0 : 1;
        var aviableTroops = new List<Card>();

        Debug.Log("Player 2 Hand: " + player2Hand[2].name);
        _playersHand[player] = player == 0 ? player1Hand : player2Hand;
        aviableTroops.AddRange(_playersHand[player].Where(troop => troop.cost <= player1Elixir));
        return aviableTroops;
    }

    // Updates the variable "playerXTroops" to the troops in the field when called, the int player must be 0 or 1
    public void TroopsInField(int player)
    {
        if (player < 0) player = 0;
        else if (player > 1) player = 1;

        GameObject[] childs = new GameObject[_playersTroopsParents[player].transform.childCount];
        int i = 0;
        foreach (NPC child in _playersTroopsParents[player].GetComponentsInChildren<NPC>())
        {
            childs[i] = child.gameObject;
            i++;
        }
        playersTroops[player] = childs;
    }
    
    // Control time
    IEnumerator ClashTimer()
    {
        while (timeLeft-- >= 0)
        {
            timeLeftTMP.text = "Time left: " + SecondsToMinutesFormat(timeLeft);
            yield return new WaitForSeconds(1f);
        }

        GetComponent<EndGameScript>().BackToMenu();
    }

    private string SecondsToMinutesFormat(float timeInSeconds)
    {
        int minutes = (int)timeInSeconds / 60;
        int seconds = (int)timeInSeconds % 60;

        if(seconds < 10) return minutes + ":0" + seconds;
        return minutes + ":" + seconds;
    }
    
/*
    public Observer Clone()
    {
        return gameObject.AddComponent<Observer>();
    }
    */

    
}

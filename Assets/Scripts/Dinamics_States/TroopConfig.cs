using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopConfig
{
    public static Vector3 UpForward;
    public static Vector3 UpBackward;
    public static Vector3 DownForwad;
    public static Vector3 DownBackWard;
    private List<Plays> _disponible;
    private Queue<Plays> _waitingToPlay;
    private int[,] _realTable;
    private ForwardModel _fm;
    private float _temporalH;
    
    public TroopConfig(){}
    public TroopConfig(Card[] cards, ForwardModel fm)
    {
        _fm = fm;
        _disponible = new List<Plays>();
        _realTable = new int[4, 4]; // row col
        foreach (Card card in cards)
        {
            AddCard(ref _disponible,card);
        }
    }

    private void AddCard(ref List<Plays> table, Card card)
    {
        switch (card.name)
            {
                case "Archer":
                    //Archer
                    table.Add(new Plays(card,spawnPosition.Any ));
                    break;
                case "Barbarian":
                    //Barbarian
                    table.Add(new Plays(card ,spawnPosition.Any ));
                    break;
                case "Giant":
                    //Giant
                    table.Add(new Plays(card ,spawnPosition.For ));
                    break;
                case "Goblin":
                    //Goblin
                    table.Add(new Plays(card ,spawnPosition.Back ));
                    break;
                case "Knight":
                    table.Add(new Plays(card ,spawnPosition.For ));
                    break;
                case "Mini-Pekka":
                    table.Add(new Plays(card ,spawnPosition.For ));
                    break;
            }
    }

    private TroopsToDeploy CreateTroopsToDeploy(Plays[] plays)
    {
        Vector3[] positions = new Vector3[plays.Length];
        Card[] cards = new Card[plays.Length];
        float[] times = new float[plays.Length];
        for (int i = 0; i < plays.Length; i++)
        {
            switch (plays[i].Sided)
            {
                case spawnPosition.For:
                    positions[i] = UpForward;
                    break;
                case spawnPosition.Back:
                    positions[i] = UpBackward;
                    break;
            }
            cards[i] = plays[i].Implicated;
            times[i] = 1;
        }
        TroopsToDeploy joderQueTropa = new TroopsToDeploy(cards, times, positions);
        return joderQueTropa;
    }
    public IEnumerator DoFirstPlays(Observer obs)
    {
        List<Plays> allPlays = new List<Plays>();
        for (int i = 0; i < 4; i++)
        {
            allPlays.Add(_disponible[i]);
            Observer obs2 = obs;
            _temporalH = -1;
            _fm.SimulateInP2(obs2, 1, CreateTroopsToDeploy(new[] { allPlays[i] }),()=>SetH(obs,obs2));
            yield return new WaitWhile(() => _temporalH == -1);
            _realTable[i, 0] = (int)_temporalH;
            Debug.Log(_temporalH);
        }
    }

    private int SetH(Observer obs,Observer obs2)
    {
        _temporalH = Heuristic.GetScore(obs, obs2);
        return 1;
    }

    private Plays PlayRow(Plays theOneBefore,List<Plays> row, Observer obs)
    {
        int i = 0;
        int j = 0;
        foreach (Plays plays in row)
        {
            if (plays.Heuristic > row[j].Heuristic)
                j = i;
            i++;
        }
        return row[j];
    }

    public void SetTableValues()
    {
        
    }

    public void Whatever(List<Plays> pls,int row, int col)
    {
        
    }
}

public enum spawnPosition
{
    For = 0,
    Back = 1,
    Any = 2
}
public struct Plays
{
    public Card Implicated { get; set; }
    public spawnPosition Sided { get; set; }
    public float Heuristic;

    public Plays(Card t, spawnPosition s)
    {
        Implicated = t;
        Sided = s;
        Heuristic = 0;
    }

    public void SetHeuristic(float h)
    {
        Heuristic = h;
    }

}
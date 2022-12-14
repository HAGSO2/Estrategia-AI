using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ForwardModel : MonoBehaviour
{
    [SerializeField] private EnemiesManager enemiesManager;
    //[SerializeField] private CardSystem _gameCards;
    public Observer _simulationObserver;
    [SerializeField] private  Card[] cardsP1 = new Card[8];
    [SerializeField] private  Card[] cardsP2 = new Card[8];
    [SerializeField] private  float[] timesP1 = new float[8];
    [SerializeField] private  float[] timesP2 = new float[8];
    [SerializeField] private  Vector3[] posP1 = new Vector3[8];
    [SerializeField] private  Vector3[] posP2 = new Vector3[8];
    
    private static int _speedMultiplier = 10;
    public bool finished = false;

    private Observer obs;
    private List<NPC> _troops = new List<NPC>(); // no funciona, no guarda referencias?

    private void Start()
    {
        _simulationObserver.timeLeft = 10000;
        _simulationObserver.player1Elixir = 5;
        _simulationObserver.player2Elixir = 5;
        _simulationObserver.TroopsInField(0);
        _simulationObserver.TroopsInField(1);
        obs = _simulationObserver;
        TroopsToDeploy player1Parameters = new TroopsToDeploy(cardsP1, timesP1, posP1);
        TroopsToDeploy player2Parameters = new TroopsToDeploy(cardsP2, timesP2, posP2);

        Simulate(obs, 10000, player1Parameters, player2Parameters,JokeEnd);
    }

    int JokeEnd()
    {
        return 1;
    }

    private void Update()
    {
        if (finished)
        {
            Debug.Log("Heuristic: " + Heuristic.GetScore(obs, _simulationObserver));
            Debug.Log("Tower 1 health: " + _simulationObserver.Player1KingTower.health + ".\n Tower 2 health: " + _simulationObserver.Player2KingTower.health);
            finished = false;
        }
    }

    public void Simulate(Observer observation, float maxSimulationTime,
                                    TroopsToDeploy player1Parameters,TroopsToDeploy player2Parameters, Func<int> end)
    {
        finished = false;
        _simulationObserver.timeLeft = observation.timeLeft / _speedMultiplier;
        BurnedAndFinalElixir(observation, true, player1Parameters);
        BurnedAndFinalElixir(observation, false, player2Parameters);
        //Deploy all troops in the game field (the ones at the observation)
        observation.TroopsInField(0);
        //Debug.Log("Obs: " + observation.playersTroops[0][0].name);
        foreach (GameObject troop in observation.playersTroops[0])
        {
            Debug.Log("Troop: " + troop.name);
            DeployTroop(troop, troop.transform.position, true);
        }
        
        observation.TroopsInField(1);
        foreach (GameObject troop in observation.playersTroops[1])
        {
            DeployTroop(troop, troop.transform.position, false);
        }
        
        // Coroutine to deploy future troops

        StartCoroutine(PlayerDeploys(0, true, player1Parameters));
        StartCoroutine(PlayerDeploys(0, false, player2Parameters));
        
        // Coroutine that manages when simulation arrived to an ending point

        StartCoroutine(SimulationEnded(maxSimulationTime,end));
    }
    
    private IEnumerator PlayerDeploys(int n, bool isPlayer1, TroopsToDeploy playerTroops)
    {
        if (n < playerTroops.Troops.Length)
        {
            yield return new WaitForSeconds((1f / _speedMultiplier) * playerTroops.TimeToWaitSinceLastTroop[n]);
            DeployTroop(playerTroops.Troops[n], playerTroops.TroopPosition[n], isPlayer1);
            yield return PlayerDeploys(n + 1, isPlayer1, playerTroops);
        }
    }

    private void DeployTroop(Card troop, Vector3 position, bool isPlayer1)
    {
        NPC enemy;
        if (isPlayer1){ enemy = Instantiate(troop.enemy, position, Quaternion.identity, _simulationObserver.player1TroopsParent.transform).GetComponent<NPC>(); }
        else { enemy = Instantiate(troop.enemy, position, Quaternion.identity, _simulationObserver.player2TroopsParent.transform).GetComponent<NPC>(); }
        enemy.Team = isPlayer1;
        enemy.Set(enemiesManager);
        _troops.Add(enemy);
    }
    private void DeployTroop(GameObject troop, Vector3 position, bool isPlayer1)
    {
        NPC enemy = Instantiate(troop, position, Quaternion.identity).GetComponent<NPC>();
        enemy.Team = isPlayer1;
        enemy.Set(enemiesManager);
        _troops.Add(enemy);
    }

    private IEnumerator SimulationEnded(float maxSimulationTime, Func<int> end)
    {
        // If simulation not ended due to conditions
        // ------ Make sure conditions are 0K ------
        if  (_simulationObserver.timeLeft > 0 && maxSimulationTime > 0 &&
            (_simulationObserver.player1TroopsParent.transform.childCount > 0 ||
            _simulationObserver.player2TroopsParent.transform.childCount > 0) &&
            _simulationObserver.Player1KingTower.health > 0 &&
            _simulationObserver.Player2KingTower.health > 0)
        {
            var comprobationInterval = 10f / _speedMultiplier;
            yield return new WaitForSeconds(comprobationInterval);
            _simulationObserver.timeLeft -= comprobationInterval;
            maxSimulationTime -= comprobationInterval;
            yield return SimulationEnded(maxSimulationTime,end);
        }
        else
        {
            _simulationObserver.TroopsInField(0);
            _simulationObserver.TroopsInField(1);
            finished = true;
            foreach (NPC troop in _troops)
            {
                Debug.Log("Destroying: " + troop);
                Destroy(troop);
            }

            end.Invoke();
        }
    }

    private void BurnedAndFinalElixir(Observer observation, bool isPlayer1, TroopsToDeploy playerParameters)
    {
        float burnedElixir = 0;
        var currElixir = isPlayer1 ? observation.player1Elixir : observation.player2Elixir;
        for (int i = 0; i < playerParameters.Troops.Length; i++)
        {
            currElixir += playerParameters.TimeToWaitSinceLastTroop[i] - playerParameters.Troops[i].cost;
            if (!(currElixir > 10)) continue;
            burnedElixir = currElixir - 10;
            currElixir = 10;
        }

        if (isPlayer1)
        {
            _simulationObserver.player1BurnedElixirInLastSimulation = burnedElixir;
            _simulationObserver.player1Elixir = currElixir;
        }
        else
        {
            _simulationObserver.player2BurnedElixirInLastSimulation = burnedElixir;
            _simulationObserver.player2Elixir = currElixir;
        }
        
    }
}

/*
public struct TroopsToDeployRight
{
    public NPC[] Troops;
    public float[] TimeToWait;
    public Vector3 Position;

    public TroopsToDeployRight(NPC[] troops, float[] timeToWait, Vector3 position)
    {
        this.Troops = troops;
        this.TimeToWait = timeToWait;
        this.Position = position;
    }
}
*/
public struct TroopsToDeploy
{
    public Card[] Troops;
    public float[] TimeToWaitSinceLastTroop;
    public Vector3[] TroopPosition;

    public TroopsToDeploy(Card[] troops, float[] timeToWaitSinceLastTroop, Vector3[] troopPosition)
    {
        this.Troops = troops;
        this.TimeToWaitSinceLastTroop = timeToWaitSinceLastTroop;
        this.TroopPosition = troopPosition;
    }
}
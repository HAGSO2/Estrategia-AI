using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardModel : MonoBehaviour
{

    [SerializeField] private static NPC[] _troops;
    [SerializeField] private EnemiesManager enemiesManager;
    //[SerializeField] private CardSystem _gameCards;
    [SerializeField] private Observer _simulationObserver;
    
    private static int _speedMultiplier = 100;
    

    public Observer Simulate(Observer observation, float maxSimulationTime, Vector3 bridgeL, Vector3 bridgeR,
                                    GameObject player1KingTower, GameObject player2KingTower,
                                    TroopsToDeploy player1Parameters,TroopsToDeploy player2Parameters)
    {
        _simulationObserver.timeLeft = observation.timeLeft / _speedMultiplier;
        BurnedAndFinalElixir(observation, true, player1Parameters);
        BurnedAndFinalElixir(observation, false, player2Parameters);
        //Deploy all troops in the game field (the ones at the observation)
        observation.TroopsInField(0);
        foreach (GameObject troop in observation.player1Troops)
        {
            DeployTroop(troop, troop.transform.position, true);
        }
        
        observation.TroopsInField(1);
        foreach (GameObject troop in observation.player2Troops)
        {
            DeployTroop(troop, troop.transform.position, false);
        }
        
        // Coroutine to deploy future troops

        StartCoroutine(PlayerDeploys(0, true, player1Parameters));
        StartCoroutine(PlayerDeploys(0, false, player2Parameters));
        
        // Coroutine that manages when simulation arrived to an ending point

        StartCoroutine(SimulationEnded(maxSimulationTime));

        return _simulationObserver;
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
        NPC enemy = Instantiate(troop.enemy, position, Quaternion.identity).GetComponent<NPC>();
        enemy.Team = isPlayer1;
        enemy.Set(enemiesManager);
    }
    private void DeployTroop(GameObject troop, Vector3 position, bool isPlayer1)
    {
        NPC enemy = Instantiate(troop, position, Quaternion.identity).GetComponent<NPC>();
        enemy.Team = isPlayer1;
        enemy.Set(enemiesManager);
    }

    private IEnumerator SimulationEnded(float maxSimulationTime)
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
        }
        else
        {
            _simulationObserver.TroopsInField(0);
            _simulationObserver.TroopsInField(1);
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
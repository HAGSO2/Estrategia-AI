using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ForwardModel
{

    [SerializeField] private static NPC[] _troops;
    
    public static Observer Simulate(Observer observation, Vector3 bridgeL, Vector3 bridgeR,
                                    GameObject player1KingTower, GameObject player2KingTower,
                                    TroopsToDeployLeft player1LParameters,TroopsToDeployRight player1RParameters,
                                    TroopsToDeployLeft player2LParameters, TroopsToDeployRight player2RParameters)
    {
        // averiguar el orden de colision y el tiempo en el que ocurre (idea, simplificarlo sacando distancia euclediana)
        // ver quien gana mediante el DPS y la vida restar daño al finalizar
        // actualizar trapas restante y repetir hasta llegar al final del tiempo, titar torre o ya no hayan más enemigos
        
        // tropas en rango se pegan
        // si no 
        float timeLeft = observation.timeLeft;
        int player1nTroops = player1LParameters.Troops.Length + player1RParameters.Troops.Length + observation.player1Troops.Length;
        int player2nTroops = player2LParameters.Troops.Length + player2RParameters.Troops.Length + observation.player2Troops.Length;
        while (timeLeft > 0 && player1nTroops > 0 && player2nTroops > 0 /*&& player1KingTower.health > 0 && player2KingTower.health > 0*/)
        {
            timeLeft--;
            
            
        }

        return null;
    }

    private static float[] distance()
    {
        return null;
    }
}


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

public struct TroopsToDeployLeft
{
    public NPC[] Troops;
    public float[] TimeToWait;
    public Vector3 Position;

    public TroopsToDeployLeft(NPC[] troops, float[] timeToWait, Vector3 position)
    {
        this.Troops = troops;
        this.TimeToWait = timeToWait;
        this.Position = position;
    }
}
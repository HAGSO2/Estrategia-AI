using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Heuristic
{
    private static int _elixirValueOfkingTower = 100;

    public static float GetScore(Observer preObservation, Observer postObservation)
    {
        
        if (postObservation.Player1KingTower.health <= 0)
        {
            return -10000;
        }
        else if (postObservation.Player2KingTower.health <= 0)
        {
            return 10000;
        }
        
        return IndividualObservationDifference(preObservation) - IndividualObservationDifference(postObservation);
    }

    private static float IndividualObservationDifference(Observer observation)
    {
        float elixirDifference = 0;
        elixirDifference += observation.player1Elixir;
        elixirDifference -= observation.player2Elixir;

        foreach (GameObject troop in observation.player1Troops)
        {
            NPC troopNPC = troop.GetComponent<NPC>();
            
            elixirDifference += troopNPC.atributes.elixirCost * (troopNPC.atributes.health / troopNPC.atributes.maxHealth);
        }
        foreach (GameObject troop in observation.player2Troops)
        {
            NPC troopNPC = troop.GetComponent<NPC>();
            
            elixirDifference -= troopNPC.atributes.elixirCost * (troopNPC.atributes.health / troopNPC.atributes.maxHealth);
        }
        
        elixirDifference += _elixirValueOfkingTower * (observation.Player1KingTower.health / observation.Player1KingTower.maxHealth);
        elixirDifference -= _elixirValueOfkingTower * (observation.Player2KingTower.health / observation.Player2KingTower.maxHealth);

        // ----------------------------------------------------------------------------------- //
        // Could include in the equation of the heuristic que burned elixir but don't know how //
        // ----------------------------------------------------------------------------------- //
        return elixirDifference;
    }
}

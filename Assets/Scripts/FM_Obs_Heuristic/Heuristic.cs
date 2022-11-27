using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Heuristic
{
    private static int _elixirValueOfkingTower = 100;

    public static float GetScore(Observer preObservation, Observer postObservation)
    {
        /*
        if (postObservation.Player1KingTower.health <= 0)
        {
            return -10000;
        }
        else if (postObservation.Player2KingTower.health <= 0)
        {
            return 10000;
        }
        */
        return IndividualObservationDifference(preObservation) - IndividualObservationDifference(postObservation);
    }

    private static float IndividualObservationDifference(Observer observation)
    {
        float elixirDifference = 0;
        elixirDifference += observation.player1Elixir;
        elixirDifference -= observation.player2Elixir;

        foreach (NPC troop in observation.player1Troops)
        {
            // ---------------------------------------------------------------------------------------------- //
            // NPC class needs a public variable of the Elixir cost, Max Health and make public actual health //
            // ---------------------------------------------------------------------------------------------- //
            
            //elixirDifference += troop.elixirCost * (troop.health / troop.maxHealth);
        }
        foreach (NPC troop in observation.player2Troops)
        {
            //elixirDifference -= troop.elixirCost * (troop.health / troop.maxHealth);
        }
        
        // ----------------------------------------------------------------------------------- //
        // Need a Game object with a component including a public Max Health and actual health //
        // ----------------------------------------------------------------------------------- //
        
/*
        elixirDifference += _elixirValueOfkingTower * (observation.Player1KingTower.health / observation.Player1KingTower.maxHealth);
        elixirDifference -= _elixirValueOfkingTower * (observation.Player2KingTower.health / observation.Player2KingTower.maxHealth);
*/
        return elixirDifference;
    }
}

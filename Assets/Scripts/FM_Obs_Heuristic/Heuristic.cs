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

        observation.TroopsInField(0);
        observation.TroopsInField(1);
        
        if (observation.playersTroops[0].Length >= 1)
        {
            foreach (GameObject troop in observation.playersTroops[0])
            {
                if (troop == null) continue;
                NPC troopNPC = troop.GetComponent<NPC>();
                elixirDifference += troopNPC.atributes.elixirCost * (troopNPC.atributes.health / troopNPC.atributes.maxHealth);


            }
        }
        if (observation.playersTroops[1].Length >= 1)
        {
            foreach (GameObject troop in observation.playersTroops[1])
            {
                if (troop == null) continue;
                NPC troopNPC = troop.GetComponent<NPC>();

                elixirDifference -= troopNPC.atributes.elixirCost *
                                    (troopNPC.atributes.health / troopNPC.atributes.maxHealth);
            }
        }

        elixirDifference += _elixirValueOfkingTower * (observation.Player1KingTower.health / observation.Player1KingTower.maxHealth);
        elixirDifference -= _elixirValueOfkingTower * (observation.Player2KingTower.health / observation.Player2KingTower.maxHealth);

        // ----------------------------------------------------------------------------------- //
        // Could include in the equation of the heuristic que burned elixir but don't know how //
        // ----------------------------------------------------------------------------------- //
        return elixirDifference;
    }
}

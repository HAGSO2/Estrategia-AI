using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployCardSystem : MonoBehaviour
{
    [SerializeField] ElixirSystem elixirSystem;
    [SerializeField] EnemiesManager enemiesManager;
    [SerializeField] Transform _player1TroopsParent;
    [SerializeField] Transform _player2TroopsParent;

    public void DeployCard(Card card, Vector3 pos, bool t)
    {
        pos.z = -1;

        elixirSystem.elixir -= card.cost;
        elixirSystem.UpdateText();
        NPC enemy = Instantiate(card.enemy, pos, Quaternion.identity, _player1TroopsParent).GetComponent<NPC>();
        enemy.Team = t;
        enemy.Set(enemiesManager);
    }

    public void DeployCardAI(Card card, Vector3 pos, bool t)
    {
        pos.z = -1;

        elixirSystem.elixir -= card.cost;
        NPC enemy = Instantiate(card.enemy, pos, Quaternion.identity, _player2TroopsParent).GetComponent<NPC>();
        enemy.Team = t;
        enemy.Set(enemiesManager);
    }
    
    public void DeployCardGeneral(Card card, Vector3 pos, bool team)
    {
        pos.z = -1;

        elixirSystem.elixir -= card.cost;
        
        NPC enemy = team ?  Instantiate(card.enemy, pos, Quaternion.identity, _player1TroopsParent).GetComponent<NPC>() :
                            Instantiate(card.enemy, pos, Quaternion.identity, _player2TroopsParent).GetComponent<NPC>() ;
        enemy.Team = team;
        enemy.Set(enemiesManager);
    }

    public bool CanDeploy(Card card)
    {
        return card.cost <= elixirSystem.elixir;
    }
}

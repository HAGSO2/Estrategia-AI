using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployCardSystem : MonoBehaviour
{
    [SerializeField] ElixirSystem elixirSystem;
    [SerializeField] EnemiesManager enemiesManager;
    [SerializeField] Transform _playerTroopsParent;

    public void DeployCard(Card card, Vector3 pos, bool t)
    {
        pos.z = -1;

        elixirSystem.elixir -= card.cost;
        elixirSystem.UpdateText();
        NPC enemy = Instantiate(card.enemy, pos, Quaternion.identity, _playerTroopsParent).GetComponent<NPC>();
        enemy.Team = t;
        enemy.Set(enemiesManager);
    }

    public void DeployCardAI(Card card, Vector3 pos, bool t)
    {
        pos.z = -1;

        elixirSystem.elixir -= card.cost;
        NPC enemy = Instantiate(card.enemy, pos, Quaternion.identity, _playerTroopsParent).GetComponent<NPC>();
        enemy.Team = t;
        enemy.Set(enemiesManager);
    }

    public bool CanDeploy(Card card)
    {
        return card.cost <= elixirSystem.elixir;
    }
}

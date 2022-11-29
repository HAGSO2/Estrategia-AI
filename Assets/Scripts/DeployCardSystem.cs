using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployCardSystem : MonoBehaviour
{
    [SerializeField] ElixirSystem elixirSystem;
    [SerializeField] EnemiesManager enemiesManager;

    public void DeployCard(Card card, Vector3 pos)
    {
        //Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = -1;

        elixirSystem.elixir -= card.cost;
        elixirSystem.UpdateText();
        NPC enemy = Instantiate(card.enemy, pos, Quaternion.identity).GetComponent<NPC>();
        enemy.Team = true;
        enemy.Set(enemiesManager);

        //Destroy(enemy.gameObject, 3f);
    }

    public void DeployCardAI(Card card, Vector3 pos)
    {
        //Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = -1;

        elixirSystem.elixir -= card.cost;
        NPC enemy = Instantiate(card.enemy, pos, Quaternion.identity).GetComponent<NPC>();
        enemy.Team = false;
        enemy.Set(enemiesManager);

        Destroy(enemy.gameObject, 3f);
    }

    public bool CanDeploy(Card card)
    {
        return card.cost <= elixirSystem.elixir;
    }
}

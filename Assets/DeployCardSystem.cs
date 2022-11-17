using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployCardSystem : MonoBehaviour
{
    [SerializeField] ElixirSystem elixirSystem;

    public void DeployCard(Card playedCard)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = -1;

        elixirSystem.UpdateText();
        var enemy = Instantiate(playedCard.enemy, pos, Quaternion.identity);

        Destroy(enemy.gameObject, 3f);
    }

    public bool CanDeploy(Card card)
    {
        if (card.cost < elixirSystem.elixir)
        {
            elixirSystem.elixir -= card.cost;
            return true;
        }
        return false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DecisionTreeAI : MonoBehaviour, IAI
{
    [SerializeField] CardSystem cardSystem;
    [SerializeField] SimpleObserver simpleObserver;

    [SerializeField] Transform upAttackTransform;
    [SerializeField] Transform upDefenseTransform;
    [SerializeField] Transform downAttackTransform;
    [SerializeField] Transform downDefenseTransform;

    public string id { get; set; }

    Vector3 pos;

    void Start()
    {
        id = "DecisionTreeAI"; 
        CreateDecisionTree();

        StartCoroutine(UpdateAI());
    }

    BinaryNode notEnoughElixir;
    BinaryNode attackAnywhere;
    BinaryNode defenseDown;
    BinaryNode attackedDown;
    BinaryNode defenseUp;
    BinaryNode attackedUp;
    BinaryNode root;
    private void CreateDecisionTree()
    {
        // LEAFS
        attackAnywhere = new BinaryNode("Attacking Anywhere", AttackingAnywhere);

        defenseDown = new BinaryNode("Defending Down", DefendingDown);

        defenseUp = new BinaryNode("Defending up", DefendingUp);

        notEnoughElixir = new BinaryNode("Not Enough Elixir", NotEnoughElixir);

        // NOT LEAFS

        attackedDown = new BinaryNode("Attacked Down?", defenseDown, attackAnywhere, AttackedDownF);

        attackedUp = new BinaryNode("Attacked Up?", defenseUp, attackedDown, AttackedUpF);

        root = new BinaryNode("Enough Elixir?", attackedUp, notEnoughElixir, EnoughElixirF);
    }

    bool NotEnoughElixir()
    {
        pos = Vector3.zero;

        return true;
    }

    bool DefendingUp()
    {
        pos = upDefenseTransform.position;

        return true;
    }

    bool DefendingDown()
    {
        pos = downDefenseTransform.position;

        return true;
    }

    bool AttackingAnywhere()
    {
        if (Random.Range(0, 1f) > 0.5f)
            pos = upAttackTransform.position;
        else
            pos = downAttackTransform.position;

        return true;
    }

    bool AttackedUpF()
    {
        bool isUpSide;

        if (!simpleObserver.sides.TryPeek(out isUpSide))
            return false;

        return isUpSide;
    }

    bool AttackedDownF()
    {
        bool isDownSide;

        if (!simpleObserver.sides.TryPeek(out isDownSide))
            return false;

        return !isDownSide;
    }

    bool EnoughElixirF()
    {
        return simpleObserver.ActualElixir() >= simpleObserver.lowestCardCost;
    }

    IEnumerator UpdateAI()
    {
        // Revisar el notTerminal, cuando cambia??
        bool notTerminal = true;
        while (notTerminal)
        {
            int index = think(null, 10f);
            // Cuidado cuando pos es Vector3.zero
            if (pos != Vector3.zero)
            {
                cardSystem.TryToPlayCardAI(index, pos);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public int think(Observer observer, float budget)
    {
        root.Evaluate();

        // Pensar que carta jugar no se 

        string cardName = "Pepe";
        FindObjectsOfType<DataGameCollector>()[0].RegisterNewEntryData(id, id, cardName);

        // return pos and index
        return 0;
    }
}

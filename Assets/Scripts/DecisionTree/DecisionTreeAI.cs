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
        attackAnywhere = new BinaryNode("Attacking Anywhehre", null, null, AttackingAnywhere);

        defenseDown = new BinaryNode("Defending Down", null, null, DefendingDown);

        attackedDown = new BinaryNode("Attacked Down?", defenseDown, attackAnywhere, AttackedDownF);

        defenseUp = new BinaryNode("Defending up", null, null, DefendingUp);

        attackedUp = new BinaryNode("Attacked Up?", defenseUp, attackedDown, AttackedUpF);

        notEnoughElixir = new BinaryNode("Not Enough Elixir", null, null, NotEnoughElixir);

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

    void Update()
    {
        int index = think(null, 10f);

        // Cuidado cuando pos es Vector3.zero
        cardSystem.TryToPlayCardAI(index, pos);
    }

    public int think(Observer observer, float budget)
    {
        root.Evaluate();

        string cardName = "A";
        FindObjectsOfType<DataGameCollector>()[0].RegisterNewEntryData(id, id, cardName);
        return 0;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DecisionTreeAI : MonoBehaviour, IAI
{
    const string ARCHER_NAME = "Archer";
    const string BARBARIAN_NAME = "Barbarian";
    const string GIANT_NAME = "Giant";
    const string GOBLIN_NAME = "Goblin";
    const string KNIGHT_NAME = "Knight";
    const string MINIPEKKA_NAME = "Mini-Pekka";

    const int ARCHER = 0;
    const int BARBARIAN = 1;
    const int GIANT = 2;
    const int GOBLIN = 3;
    const int KNIGHT = 4;
    const int MINIPEKKA = 5;

    [SerializeField] CardSystem cardSystem;
    [SerializeField] SimpleObserver simpleObserver;

    [SerializeField] Transform upAttackTransform;
    [SerializeField] Transform upDefenseTransform;
    [SerializeField] Transform downAttackTransform;
    [SerializeField] Transform downDefenseTransform;

    public string id { get; set; }
    public bool isLeftSide = false;

    int indexToPlay = -1;
    Vector3 pos = Vector3.zero;

    void Start()
    {
        id = "DecisionTreeAI"; 
        CreatePositionDecisionTree();
        CreateCardDecisionTree();
    }

    BinaryNode notEnoughElixir;
    BinaryNode attackAnywhere;
    BinaryNode defenseDown;
    BinaryNode attackedDown;
    BinaryNode defenseUp;
    BinaryNode attackedUp;
    BinaryNode positionRoot;
    private void CreatePositionDecisionTree()
    {
        // LEAFS
        attackAnywhere = new BinaryNode("Attacking Anywhere", AttackingAnywhere);

        defenseDown = new BinaryNode("Defending Down", DefendingDown);

        defenseUp = new BinaryNode("Defending up", DefendingUp);

        notEnoughElixir = new BinaryNode("Not Enough Elixir", NotEnoughElixir);


        // NOT LEAFS
        attackedDown = new BinaryNode("Attacked Down?", defenseDown, attackAnywhere, AttackedDownF);

        attackedUp = new BinaryNode("Attacked Up?", defenseUp, attackedDown, AttackedUpF);

        positionRoot = new BinaryNode("Enough Elixir?", attackedUp, notEnoughElixir, EnoughElixirF);
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

    ///////////////////////////////////////////////////////////////////////////////////////////////////////

    bool AttackedUpF()
    {
        bool isUpSide;

        // If it's empty --> false
        if (!simpleObserver.sides.TryPeek(out isUpSide))
        {
            return false;
        }

        if (isUpSide)
        {
            simpleObserver.PositionDefended();
            return true;
        }

        return false;
    }

    bool AttackedDownF()
    {
        bool isDownSide;

        // If it's empty --> false
        if (!simpleObserver.sides.TryPeek(out isDownSide))
        {
            return false;
        }

        if (!isDownSide)
        {
            simpleObserver.PositionDefended();
            return true;
        }

        return false;
    }

    bool EnoughElixirF()
    {
        return simpleObserver.ActualElixir() >= simpleObserver.lowestCardCost + 1;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////

    BinaryNode pseudoRandom;
    BinaryNode barbarian;
    BinaryNode minipekka;
    BinaryNode giant;
    BinaryNode knight;
    BinaryNode goblin;
    BinaryNode archer;
    BinaryNode isMinipekka;
    BinaryNode isGiant;
    BinaryNode isGoblin;
    BinaryNode isArcher;
    BinaryNode checkCost;
    BinaryNode cardRoot;
    private void CreateCardDecisionTree()
    {

        // LEAFS
        barbarian = new BinaryNode("Barbarian", PlayAgainstBarbarian);

        minipekka = new BinaryNode("Minipekka", PlayAgainstMinipekka);

        giant = new BinaryNode("Giant", PlayAgainstGiant);

        knight = new BinaryNode("Knight", PlayAgainstKnight);

        goblin = new BinaryNode("Goblin", PlayAgainstGoblin);

        archer = new BinaryNode("Archer", PlayAgainstArcher);

        pseudoRandom = new BinaryNode("Pseudo random", PseudoRandom);

        // NO LEAFS
        isMinipekka = new BinaryNode("Is minipekka?", minipekka, barbarian, IsMinipekka);

        isGiant = new BinaryNode("Is giant?", giant, isMinipekka, IsGiant);

        isGoblin = new BinaryNode("Is goblin?", goblin, knight, IsGoblin);

        isArcher = new BinaryNode("Is archer?", archer, isGoblin, IsArcher);

        checkCost = new BinaryNode("Cost <= 3?", isArcher, isGiant, CheckCost);

        cardRoot = new BinaryNode("Card played?", checkCost, pseudoRandom, IsCardPlayed);
    }

    bool PlayAgainstBarbarian()
    {
        indexToPlay = simpleObserver.CheckForCard(GIANT_NAME);

        return true;
    }

    bool PlayAgainstMinipekka()
    {
        indexToPlay = simpleObserver.CheckForCard(ARCHER_NAME);

        return true;
    }

    bool PlayAgainstGiant()
    {
        indexToPlay = simpleObserver.CheckForCard(MINIPEKKA_NAME);

        return true;
    }

    bool PlayAgainstKnight()
    {
        indexToPlay = simpleObserver.CheckForCard(MINIPEKKA_NAME);

        return true;
    }

    bool PlayAgainstGoblin()
    {
        indexToPlay = simpleObserver.CheckForCard(KNIGHT_NAME);

        return true;
    }

    bool PlayAgainstArcher()
    {
        indexToPlay = simpleObserver.CheckForCard(GOBLIN_NAME);

        return true;
    }

    bool PseudoRandom()
    {
        indexToPlay = simpleObserver.CheckForCard(GIANT_NAME);

        return true;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////

    bool IsMinipekka()
    {
        if (simpleObserver.names.Peek() == MINIPEKKA_NAME)
        {
            simpleObserver.NamesDefended();
            return true;
        }

        simpleObserver.NamesDefended();
        return false;
    }

    bool IsGiant()
    {
        if (simpleObserver.names.Peek() == GIANT_NAME)
        {
            simpleObserver.NamesDefended();
            return true;
        }

        return false;
    }

    bool IsGoblin()
    {
        if (simpleObserver.names.Peek() == GOBLIN_NAME)
        {
            simpleObserver.NamesDefended();
            return true;
        }

        simpleObserver.NamesDefended();
        return false;
    }

    bool IsArcher()
    {
        if (simpleObserver.names.Peek() == ARCHER_NAME)
        {
            simpleObserver.NamesDefended();
            return true;
        }

        return false;
    }

    bool CheckCost()
    {
        int enemyCardCost = simpleObserver.costs.Dequeue();

        if (enemyCardCost <= 3)
        {
            return true;
        }

        return false;
    }

    bool IsCardPlayed()
    {
        if (simpleObserver.names.TryPeek(out _))
            return true;

        return false;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////

    float timer = 0f;
    public float maxTime = 5f;
    void Update()
    {
        // Revisar el notTerminal, cuando cambia??
        timer += Time.deltaTime;
        if (timer >= maxTime && !simpleObserver.IsTerminal())
        {
            timer = 0f;
            int index = think(null, 10f);

            if (pos != Vector3.zero)
            {
                cardSystem.TryToPlayCardAI(index, pos, isLeftSide);
                simpleObserver.SortCards();
            }
            else
            {
                Debug.Log("Poco elixir: " + simpleObserver.ActualElixir() + " < " + (simpleObserver.lowestCardCost + 1));
            }
        }
    }
    public int think(Observer observer, float budget)
    {
        positionRoot.Evaluate();

        if (pos == Vector3.zero)
            return -1;
            
        cardRoot.Evaluate();

        string cardName = simpleObserver.playedCardName;
        //FindObjectsOfType<DataGameCollector>()[0].RegisterNewEntryData(id, id, cardName);

        // return pos and index
        return indexToPlay;
    }
}

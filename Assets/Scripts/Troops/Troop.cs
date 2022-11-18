using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Troop", menuName = "ScriptableObjects/Troop", order = 1)]

public class Troop : ScriptableObject
{
    [Header("Parameters")]
    [SerializeField] float speed = 10f;
    [SerializeField] float attackRange = 10f;
    [SerializeField] float health = 10f;
    [SerializeField] float attacksPerSecond = 2f;
    [SerializeField] float damage = 10f;
    [SerializeField] bool onlyAttacksBuildings = false;
    [SerializeField] int elixirCost = 1;

    [Header("Sprites")]
    [SerializeField] Sprite[] walkingSprites;
    [SerializeField] Sprite[] attackSprites;
    [SerializeField] Sprite[] deathSprites;


}

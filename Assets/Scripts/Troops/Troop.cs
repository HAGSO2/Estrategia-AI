using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "Troop", menuName = "ScriptableObjects/Troop", order = 1)]

public class Troop : ScriptableObject
{
    [Header("Parameters")]
    public float speed = 10f;
    public float attackRange = 10f;
    public float health = 10f;
    public float maxHealth = 10f;
    public float attacksPerSecond = 2f;
    public float damage = 10f;
    public float visionRange = 10f;
    public bool onlyAttacksBuildings = false;
    public int elixirCost = 1;
}

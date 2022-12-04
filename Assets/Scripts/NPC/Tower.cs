using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public EnemiesManager _Manager;
    public bool Team;

    public void Hurt(float dmg)
    {
        // TO DO
        health -= dmg;
        if (health <= 0)
        {
            _Manager.TowerDown();
            Debug.Log("Za warudo");
            Time.timeScale = 0;
            Destroy(gameObject);
        }
    }
}

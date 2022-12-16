using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public EnemiesManager _Manager;
    public bool Team;
    [SerializeField] private TextMeshProUGUI healthTMP;
    public bool isSimulator;
    public void Hurt(float dmg)
    {
        // TO DO
        health -= dmg;
        if(!isSimulator) healthTMP.text = "Health: " + health;
        if (health <= 0)
        {
            _Manager.TowerDown();
            Debug.Log("Za warudo");
            Time.timeScale = 0;
            Destroy(gameObject);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]private float _health;
    public EnemiesManager _Manager;
    public bool Team;

    public void Hurt(float dmg)
    {
        // TO DO
        _health -= dmg;
        if (_health <= 0)
        {
            _Manager.TowerDown();
            Debug.Log("Za warudo");
            Time.timeScale = 0;
            Destroy(gameObject);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]private float _health;
    public EnemiesManager _Manager;
    public bool Team;

    private void Start()
    {
        if (Team)
            _Manager.Tower1 = transform;
        else
            _Manager.Tower2 = transform;
    }

    public void Hurt(float dmg)
    {
        _health -= dmg;
        if (_health <= 0)
        {
            _Manager.TowerDown();
            Destroy(gameObject);
        }
    }
}

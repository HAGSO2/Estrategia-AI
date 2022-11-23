using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NPC : MonoBehaviour
{
    [SerializeField] private EnemiesManager _manager;
    private Transform _target = null;
    [SerializeField] private Troop atributes;
    private BinTree _behaviour;
    private Rigidbody2D _body;
    private Animator _controller;
    private float _health;
    private bool _attackMutex = true;
    private bool _eInRange = false;
    public bool Team;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _controller = GetComponent<Animator>();
        _health = atributes.health;
        transform.GetChild(0).localScale = new Vector3(atributes.attackRange / 2,atributes.attackRange / 2);
        _attackMutex = true;
        _eInRange = false;
        _manager.AddTroop(transform,Team);
        MakeBehaviour();
    }

    void MakeBehaviour()
    {
    }

    private void FixedUpdate()
    {
        _behaviour.DoStep();
    }

    

    public IEnumerator Attack(NPC other)
    {
        yield return new WaitForSeconds(1 / atributes.attacksPerSecond);
        if (!_eInRange)
        {
            yield return new WaitForSeconds(1);
            _attackMutex = true;
            yield break;
        }
        try
        {
            other.Hurt(atributes.damage);
        }
        catch
        {
            _attackMutex = true;
            _controller.SetBool("Attacking",false);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
            yield break;
        }
        StartCoroutine(Attack(other));
    }

    public void Hurt(float dmg)
    {
        _health -= dmg;
        if (_health <= 0)
        {
            _controller.SetTrigger("Dead");
        }
    }

    public void Die()
    {
        _manager.DeleteTroop(transform,Team);
        Destroy(gameObject);
    }
}
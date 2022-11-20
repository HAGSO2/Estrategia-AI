using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Transform t;
    [SerializeField] private Troop Atributes;
    private Rigidbody2D _body;
    private Animator _controller;
    private float _health;
    private bool _attackMutex;
    private bool _eInRange;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _controller = GetComponent<Animator>();
        _health = Atributes.health;
        transform.GetChild(0).localScale = new Vector3(Atributes.attackRange / 2,Atributes.attackRange / 2);
        _attackMutex = true;
        _eInRange = false;
    }
    
    private void FixedUpdate()
    {
        if(_attackMutex)
            MoveTo(t.position);
    }

    private Vector2 ToV2(Vector3 xyz)
    {
        return new Vector2(xyz.x, xyz.y);
    }

    private Vector3 ToV3(Vector2 xy)
    {
        return new Vector3(xy.x, xy.y, 0);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Destructible") && _attackMutex)
        {
            _attackMutex = false;
            _eInRange = true;
            Debug.Log("Attack");
            _controller.SetBool("Attacking",true);
            StartCoroutine(Attack(col.GetComponent<NPC>()));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.CompareTag("Destructible"))
        {
            _eInRange = false;
            Debug.Log("Stop");
            _controller.SetBool("Attacking",false);
        }
    }

    public void MoveTo(Vector3 target,bool seek = true)
    {
        Vector3 desired = target - transform.position;
        if (desired.magnitude > 0.5f)
        {
            desired = desired.normalized * Atributes.speed * Time.deltaTime;
            _body.MovePosition(transform.position + desired);
        }
    }

    public IEnumerator Attack(NPC other)
    {
        yield return new WaitForSeconds(1 / Atributes.attacksPerSecond);
        if (!_eInRange)
        {
            yield return new WaitForSeconds(1);
            _attackMutex = true;
            yield break;
        }
        try
        {
            other.Hurt(Atributes.damage);
        }
        catch
        {
            _attackMutex = true;
            _controller.SetBool("Attacking",false);
            yield break;
        }
        StartCoroutine(Attack(other));
    }

    public void Hurt(float dmg)
    {
        Debug.Log(gameObject.name + ": " + _health);
        _health -= dmg;
        if (_health <= 0)
        {
            _controller.SetTrigger("Dead");
            Destroy(this);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MEF : MonoBehaviour
{
    public Transform t;
    public bool Team;
    [SerializeField] private Troop atributes;
    private Rigidbody2D _body;
    private Animator _controller;
    private float _health;
    private bool _attackMutex;
    private bool _eInRange;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _controller = GetComponent<Animator>();
        _health = atributes.health;
        transform.GetChild(0).localScale = new Vector3(atributes.attackRange / 2,atributes.attackRange / 2);
        _attackMutex = true;
        _eInRange = false;
    }

    private void FixedUpdate()
    {
        if (!_eInRange)
            MoveTo(t.position);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Destructible") && _attackMutex && col.GetComponent<MEF>().Team != Team)
        {
            Debug.Log("Clang");
            _attackMutex = false;
            _controller.SetBool("Attacking",true);
            StartCoroutine(Attack(col.GetComponent<MEF>()));
            _eInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Destructible"))
        {
            _eInRange = false;
            _controller.SetBool("Attacking",false);
        }
    }
    
    public int MoveTo(Vector3 target)
    {
        Vector3 desired = target - transform.position;
        if (desired.magnitude > 0.5f)
        {
            desired = desired.normalized * atributes.speed * Time.deltaTime;
            //desired = seek ? desired : -desired;
            _body.MovePosition(transform.position + desired);
        }

        return 1;
    }
    public IEnumerator Attack(MEF other)
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
            if(_health > 0)
                other.Hurt(atributes.damage);
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
        _health -= dmg;
        if (_health <= 0)
        {
            StartCoroutine(Disapear());
        }
    }

    private IEnumerator Disapear()
    {
        _controller.SetTrigger("Dead");
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }


}

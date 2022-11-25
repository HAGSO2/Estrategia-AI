using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private bool _InRange = false;
    public bool Team;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _controller = GetComponent<Animator>();
        _health = atributes.health;
        transform.GetChild(0).localScale = new Vector3(atributes.attackRange / 2,atributes.attackRange / 2);
        _attackMutex = true;
        _InRange = false;
        _manager.AddTroop(transform,Team);
        GetComponent<SpriteRenderer>().flipX = Team;
        MakeBehaviour();
    }

    void MakeBehaviour()
    {
        _behaviour = new BinTree("Alive", () => _health > 0, () => 1, true, true);
        _behaviour.InsertState("Dead",()=>true, () =>
        {
            transform.GetChild(0).gameObject.SetActive(false);
            GetComponent<BoxCollider2D>().enabled = false;
            return 1;
        },false,false);
        _behaviour.InsertState("To attack what?",()=>_target!=null,()=>1,true,true);
        _behaviour.InsertState("Check a look",()=>true,TakeTarget,false,false);
        _behaviour.InsertState("In range?",()=>_InRange,()=>1,true,true);
        _behaviour.InsertState("Go near",()=>true,MoveTo,false,false);
        _behaviour.InsertState("Atack!",()=>true, ()=>
        {
            if (_attackMutex)
            {
                _attackMutex = false;
                _controller.SetBool("Attacking",true);
                StartCoroutine(Attack(_target.GetComponent<NPC>()));
            }

            return 1;
        },false,false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Attack") && col.GetComponentInParent<NPC>().Team != Team)
        {
            _InRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Attack") && other.GetComponentInParent<NPC>().Team != Team)
        {
            _InRange = false;
            Debug.Log("Buala");
        }
    }

    private void FixedUpdate()
    {
        _behaviour.DoStep();
    }

    private int TakeTarget()
    {
        _target = _manager.SearchNearest(transform.position, Team);
        Debug.Log("Target for:" + _target.name);
        return 1;
    }
    
    public int MoveTo()
    {
        Vector3 desired = _target.position - transform.position;
        if (desired.magnitude > 0.1f)
        {
            desired = desired.normalized * atributes.speed * Time.deltaTime;
            _body.MovePosition(transform.position + desired);
        }

        return 1;
    }
    
    public IEnumerator Attack(NPC other)
    {
        yield return new WaitForSeconds(1 / atributes.attacksPerSecond);
        try
        {
            other.Hurt(atributes.damage);
        }
        catch
        {
            _attackMutex = true;
            _InRange = false;
            _controller.SetBool("Attacking",false);
            Debug.Log("Abula");
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
        Debug.Log("I've died, i'm an " + gameObject.name);
        _manager.DeleteTroop(transform,Team);
        Destroy(gameObject);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class NPC : MonoBehaviour
{
    private EnemiesManager _manager;
    private Transform _target = null;
    [SerializeField] private Troop atributes;
    private Pathfinding _path;
    private BinTree _behaviour;
    private Rigidbody2D _body;
    private Animator _controller;
    private float _health;
    private bool _attackMutex = true;
    private bool _hardOponent;
    private bool _InRange = false;
    public bool Team;
    public bool doStart;
    public EnemiesManager manualManager;

    private void Start()
    {
        if(doStart)
            Set(manualManager);
    }

    public void Set(EnemiesManager m)
    {
        _path = GetComponent<Pathfinding>();
        _manager = m;
        _body = GetComponent<Rigidbody2D>();
        _controller = GetComponent<Animator>();
        _health = atributes.health;
        transform.GetChild(0).localScale = new Vector3(atributes.attackRange / 2,atributes.attackRange / 2);
        _attackMutex = true;
        _InRange = false;
        _manager.AddTroop(transform,Team);
        GetComponent<SpriteRenderer>().flipX = !Team;
        MakeBehaviour();
    }

    void MakeBehaviour()
    {
        _behaviour = new BinTree("Alive", () => _health > 0, () => 1, true, true);
        _behaviour.InsertState("Dead", () => true, () =>
        {
            transform.GetChild(0).gameObject.SetActive(false);
            GetComponent<BoxCollider2D>().enabled = false;
            return 1;
        }, false, false);
        _behaviour.InsertState("To attack what?", () => _target != null, () => 1, true, true);
        _behaviour.InsertState("Check a look", () => _target.GetComponent<NPC>() != null, TakeTarget, true, true);
        _behaviour.InsertState("Attack Structure", () => true, () =>
            {
                _hardOponent = false;
                return 1;
            }
            , false, false);
        _behaviour.InsertState("Attack NPC", () => true, () =>
        {
            _hardOponent = true;
            return 1;
        }, false, false);
        _behaviour.InsertState("In range?",()=>_InRange,()=>1,true,true);
        _behaviour.InsertState("Go near",()=>true,MoveTo,false,false);
        _behaviour.InsertState("Atack!",()=>true, DistinguishAttack,false,false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.CompareTag("Destructible") && col.GetComponentInParent<NPC>().Team != Team) || (col.CompareTag("Structure") && col.GetComponentInParent<Tower>().Team != Team))
        {
            _InRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Attack") || other.CompareTag("Structure")) && other.GetComponentInParent<NPC>().Team != Team)
        {
            _InRange = false;
            Debug.Log("Buala");
        }
    }

    private int DistinguishAttack()
    {

        if (_attackMutex)
        {
//            Debug.Log(gameObject.name + " go for the attack");
            _path.enabled = false;
            if(gameObject.name == "Giant (1)")
                Debug.Log("On attack");
            if (_hardOponent)
            {
                _controller.SetBool("Attacking", true);
                StartCoroutine(Attack(_target.GetComponent<NPC>()));
            }
            else
            {
                _controller.SetBool("Attacking", true);
                StartCoroutine(AttackTower(_target.GetComponent<Tower>()));
            }

            _attackMutex = false;
        }


        return 1;
    }

    private void FixedUpdate()
    {
        _behaviour.DoStep();
    }

    private int TakeTarget()
    {
        _target = _manager.SearchNearest(transform.position,atributes.visionRange, Team);
        _path.targetPos = _target;
        if (Vector3.Distance(transform.position, _target.position) < atributes.attackRange / 2)
            _InRange = true;
        _path.enabled = true;
        if(gameObject.name == "Giant (1)")
            Debug.Log(_target.name);
//        Debug.Log("Target for:" + _target.name);
        return 1;
    }
    
    public int MoveTo()
    {
        if (_path.finalPath.Count > 1)
        {
            Vector3 desired = _path.finalPath[1].position - transform.position;
            if (desired.magnitude > 0.1f)
            {
                desired = desired.normalized * atributes.speed * Time.deltaTime * Time.timeScale;
                _body.MovePosition(transform.position + desired);
            }
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
            yield break;
        }
        StartCoroutine(Attack(other));
    }
    
    public IEnumerator AttackTower(Tower other)
    {
        yield return new WaitForSeconds(1 / atributes.attacksPerSecond);
        try
        {
            other.Hurt(atributes.damage);
        }
        catch
        {
            _attackMutex = true;
            _target = null;
            _InRange = false;
            _controller.SetBool("Attacking",false);
            yield break;
        }
        StartCoroutine(AttackTower(other));
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

    public void Stop()
    {
        _path.enabled = false;
        Destroy(GetComponent<Pathfinding>());
        Destroy(this);
    }
}
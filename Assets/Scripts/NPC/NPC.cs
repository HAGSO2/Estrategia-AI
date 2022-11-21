using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NPC : MonoBehaviour
{
    public Transform t;
    [SerializeField] private Troop atributes;
    private BinTree _Behaviour;
    private Rigidbody2D _body;
    private Animator _controller;
    private float _health;
    private bool _attackMutex;
    private bool _eInRange;
    private NPC _attacking;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _controller = GetComponent<Animator>();
        _health = atributes.health;
        transform.GetChild(0).localScale = new Vector3(atributes.attackRange / 2,atributes.attackRange / 2);
        _attackMutex = true;
        _eInRange = false;
        MakeBehaviour();
    }

    void MakeBehaviour()
    {
        _Behaviour = new BinTree("Top", () => _eInRange, () => 1, true, true);
        _Behaviour.InsertState("Move to",()=> true,()=>MoveTo(t.position),false,false);
        _Behaviour.InsertState("In range", () => false, () => 1, true, true);
        _Behaviour.InsertState("Fleeee!",()=> true,()=>MoveTo(_attacking.transform.position,false),false,false);
        _Behaviour.InsertState("Position",()=> Vector3.Distance(transform.position,_attacking.transform.position) <= atributes.attackRange,
            ()=>MoveTo(_attacking.transform.position),false,true);
        _Behaviour.InsertState("Fireee!",()=> true,()=>{
            if (!_attackMutex)
            {
                _attackMutex = true;
                StartCoroutine(Attack(_attacking));
            }

            return 1;
        },false,false);
    }
    
    private void FixedUpdate()
    {
        _Behaviour.DoStep();
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
            _attacking = col.GetComponent<NPC>();
            /*Debug.Log("Attack");
            _controller.SetBool("Attacking",true);
            //StartCoroutine(Attack(col.GetComponent<NPC>()));*/
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.CompareTag("Destructible"))
        {
            _eInRange = false;
            _controller.SetBool("Attacking",false);
        }
    }

    public int MoveTo(Vector3 target,bool seek = true)
    {
        Vector3 desired = target - transform.position;
        if (desired.magnitude > 0.5f)
        {
            desired = desired.normalized * atributes.speed * Time.deltaTime;
            desired = seek ? desired : -desired;
            _body.MovePosition(transform.position + desired);
        }

        return 1;
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
            Destroy(this);
        }
    }
}
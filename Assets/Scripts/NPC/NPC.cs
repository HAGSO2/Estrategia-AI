using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Transform t;
    [SerializeField] private Troop Atributes;
    private Rigidbody2D _body;
    private float _health;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _health = Atributes.health;
        transform.GetChild(0).localScale = new Vector3(Atributes.attackRange / 2,Atributes.attackRange / 2);
    }
    
    private void FixedUpdate()
    {
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
        if (col.CompareTag("Destructible"))
        {
            StartCoroutine(Attack(col.GetComponent<NPC>()));
        }
    }

    public void MoveTo(Vector3 target)
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
        try
        {
            other.Hurt(Atributes.damage);
        }
        catch{yield break;}
        yield return Attack(other);
    }

    public void Hurt(float dmg)
    {
        _health -= dmg;
        if (_health <= 0)
            Destroy(gameObject);
    }
}
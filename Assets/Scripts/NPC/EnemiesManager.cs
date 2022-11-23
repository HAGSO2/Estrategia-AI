using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public Transform Tower1;
    public Transform Tower2;
    private List<Transform> TroopsA = new List<Transform>();
    private List<Transform> TroopsB = new List<Transform>();

    private void Start()
    {
        TroopsA.Add(Tower1);
        TroopsB.Add(Tower2);
    }

    public void AddTroop(Transform en, bool team)
    {
        if(team)
            TroopsA.Add(en);
        else
            TroopsB.Add(en);
    }

    public void DeleteTroop(Transform en, bool team)
    {
        if(team)
            TroopsA.Remove(en);
        else
            TroopsB.Remove(en);
    }

    public Transform SearchNearest(Vector3 pos, bool team)
    {
        List<Transform> Troops = team ? TroopsA : TroopsB;
        float dist = Vector3.Distance(Troops[0].position, pos);
        int j = 0;
        for (int i = 1; i < Troops.Count; i++)
        {
            float temp = Vector3.Distance(Troops[i].position, pos);
            if (temp < dist)
            {
                dist = temp;
                j = i;
            }
        }

        return Troops[j];
    }

    /*struct TargetByTeam
    {
        public Transform pos;
        public bool Team;

        public TargetByTeam(Transform p, bool t)
        {
            pos = p;
            Team = t;
        }

    }*/
}

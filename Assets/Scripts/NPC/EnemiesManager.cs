using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField]private Transform Tower1;
    [SerializeField]private Transform Tower2;
    private List<Transform> TroopsA;
    private List<Transform> TroopsB;

    private void Awake()
    {
        TroopsA = new List<Transform>();
        TroopsB = new List<Transform>();
        TroopsA.Add(Tower1);
        TroopsB.Add(Tower2);
    }

    public void AddTroop(Transform en, bool team)
    {
        if(team)
            TroopsA.Add(en);
        else
            TroopsB.Add(en);
        //DebugEnemies();
    }

    public void DeleteTroop(Transform en, bool team)
    {
        if (team)
            TroopsA.Remove(en);
        else
            TroopsB.Remove(en);
        //DebugEnemies();
    }

    public void TowerDown()
    {
        for(int i = 0; i < TroopsA.Count;i++){TroopsA[i].GetComponent<NPC>().Stop();}
        for(int i = 0; i < TroopsB.Count;i++){TroopsB[i].GetComponent<NPC>().Stop();}
        Debug.Log("FIN");
    }

    public bool OnlyTower(bool team)
    {
        if (team)
            return TroopsB.Count == 1;
        else
            return TroopsA.Count == 1;

    }

    public Transform SearchNearest(Vector3 pos,float mindist, bool team)
    {
        List<Transform> troops = !team ? TroopsA : TroopsB;
        //float dist = mindist;
        int j = 0;
        for (int i = 1; i < troops.Count; i++)
        {
            float temp = Vector3.Distance(troops[i].position, pos);
            if (temp < mindist)
            {
                mindist = temp;
                j = i;
            }
        }

        return troops[j];
    }

    private void DebugEnemies()
    {
        string allTransforms = "";
        for (int i = 0; i < TroopsA.Count; i++){allTransforms += TroopsA[i].name + "\t";}

        allTransforms += "\n";
        for (int i = 0; i < TroopsB.Count; i++){allTransforms += TroopsB[i].name + "\t";}
        Debug.Log(allTransforms);
    }
}

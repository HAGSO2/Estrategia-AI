using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DinamicAI : MonoBehaviour//, IAI
{
    [SerializeField] private Transform upForward;
    [SerializeField] private Transform upBackward;
    [SerializeField] private Transform downForwad;
    [SerializeField] private Transform downBackWard;
    private TroopConfig _config;
    [SerializeField] private Observer observer;
    [SerializeField] private ForwardModel fm;

    private void Awake()
    {
        TroopConfig.UpForward = upForward.position;
        TroopConfig.UpBackward = upBackward.position;
        TroopConfig.DownForwad = downForwad.position;
        TroopConfig.DownBackWard = downBackWard.position;
    }

    void Start()
    {
        //Debug.Log(observer.player2Hand);
        _config = new TroopConfig(GetComponent<CardSystem>().hand,fm);
        //_config = new TroopConfig();
        StartCoroutine(_config.DoFirstPlays(observer));
    }

    /*public string id { get; set; }
    public int think(Observer observer, float budget)
    {
        FindObjectOfType<DataGameCollector>().RegisterNewEntryData(id,id,"Ahh");
        return 1;
    }*/
}


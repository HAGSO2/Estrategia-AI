using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinamicAI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

struct PlayResult
{
    private int best;
    private int[] results;

    public PlayResult(int num)
    {
        best = -1;
        results = new int[num];
    }
}
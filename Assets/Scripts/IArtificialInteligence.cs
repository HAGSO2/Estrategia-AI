using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAI
{
    public string id { get; set; }

    public int think(Observer observer, float budget);
}
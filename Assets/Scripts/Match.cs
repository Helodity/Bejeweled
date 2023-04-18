using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct Match
{
    int Length;
    TileContents.ContentType Type;
   
    public Match( TileContents.ContentType type, int length) {
        Length = length;
        Type = type;
    }
}

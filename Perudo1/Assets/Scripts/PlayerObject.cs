using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject
{
   
   
    public int Id 
    {
        get;
        set;
    }
    public string Cup 
    
    {
        get;
        set;
    }
    

    public string Name 
    {
        get;
        set;
    }
    
    public bool ReadyState
    {
        get;
        set;
    }
    public bool NewDice 
    {
        get;
        set;
    }
    public int DiceLeft
    {
        get;
        set;
    } = 5;
    
    public List<int> Dice 
    {
        get;
        set;
    }

    public List<int> PreviousDice 
    {
        get;
        set;
    }
    public int CupColour 
    {
        get;
        set;
    }
    public int DiceColour
    {
        get;
        set;
    }
}

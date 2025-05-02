using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    private void Start()
    {
        
    }

    

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Pool", menuName = "Gamer Wolf Utilities/Object Pooling/Pool")]
public class PoolSO : ScriptableObject
{
    public string tag;
    public GameObject prefabs;
    [Min(1)] public int size = 5;

    [Header("Auto Refill")]
    public bool canExpand = true;
    [Min(1)] public int expandAmount = 3;
    [Min(0)] public int maxSize = 0;
}

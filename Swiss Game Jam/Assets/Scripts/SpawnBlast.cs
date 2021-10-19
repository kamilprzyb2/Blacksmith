using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlast : MonoBehaviour
{
    [SerializeField] private SmithyManager _manager;

    void Spawn()
    {
        _manager.SpawnBlast();
    }
}

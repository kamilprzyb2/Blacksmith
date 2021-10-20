using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogMove : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 1f;
    
    void Update()
    {
        transform.position =
            transform.position + Vector3.left * _movementSpeed * Time.deltaTime;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform target;
    void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.transform.position = target.transform.position;
    }
}

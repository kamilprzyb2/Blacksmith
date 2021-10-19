using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cut : MonoBehaviour
{
    [SerializeField] private List<Sprite> _sprites;
    void Start()
    {
        GetComponent<SpriteRenderer>()
            .sprite = _sprites[Random.Range(0, _sprites.Count)];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private GameObject[] _sqaures = new GameObject[5];
    
    public void ShowState(float HP)
    {
        if (HP <= 0.8 && _sqaures.Length > 4)
        {
            Destroy(_sqaures[4].gameObject);
        }
        if (HP <= 0.6 && _sqaures.Length > 3)
        {
            Destroy(_sqaures[3].gameObject);
        }
        if (HP <= 0.4 && _sqaures.Length > 2)
        {
            Destroy(_sqaures[2].gameObject);
        }
        if (HP <= 0.2 && _sqaures.Length > 1)
        {
            Destroy(_sqaures[1].gameObject);
        }
    }
}

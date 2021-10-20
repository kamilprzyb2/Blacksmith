using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMarkup : MonoBehaviour
{
    [SerializeField] private GameObject _parent;
    public void KillMe()
    {
        Destroy(_parent);
    }
}

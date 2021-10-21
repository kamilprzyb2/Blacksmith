using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillGoblin : MonoBehaviour
{
    void Kill()
    {
        Destroy(transform.parent.gameObject);
    }
}

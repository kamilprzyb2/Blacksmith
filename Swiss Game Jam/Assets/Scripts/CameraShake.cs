using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Animator _cameraAnim;

    void Shake()
    {
        _cameraAnim.Play("Camera_Shake");
    }
}

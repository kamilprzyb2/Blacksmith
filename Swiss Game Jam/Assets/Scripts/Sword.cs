using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "ScriptableObjects/Sword", order = 1)]
public class Sword : ScriptableObject
{
    public string swordName = "New Sword";
    public int requiresCuts = 3;
    public float shaftSpeed = 3f;
    public int baseUsage = 1;
    public int baseDamage = 3;

    public int usagesLeft;
}

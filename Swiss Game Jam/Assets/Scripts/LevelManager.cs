using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _levels;
    [SerializeField] private GameObject _magicPoint;

    public void ChangeLevel()
    {
        foreach (Transform child in _magicPoint.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject newLevel = _levels[Random.Range(0, _levels.Count)];

        Instantiate(newLevel, _magicPoint.transform);
    }

    public int EnemiesRemaining()
    {
        var enemies = _magicPoint.transform.GetComponentsInChildren<Enemy>();

        // -1 compensates dangerous destroy delay
        return enemies.Length-1;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

        GameObject newLevel;

        int safety = 0;

        do
        {
            newLevel = _levels[Random.Range(0, _levels.Count)];
            safety++;
            Debug.Log(newLevel.name);
            Debug.Log(safety);
        } while (!IsLevelGood(newLevel) && safety < 100);

        Instantiate(newLevel, _magicPoint.transform);
    }

    public int EnemiesRemaining()
    {
        var enemies = _magicPoint.transform.GetComponentsInChildren<Enemy>();

        // -1 compensates dangerous destroy delay
        return enemies.Length-1;
    }

    private bool IsLevelGood(GameObject level)
    {
        // enemies sorted from right to left
        List<Enemy> enemies = level.GetComponentsInChildren<Enemy>()
            .OrderBy(x =>x.transform.position.x)
            .Reverse()
            .ToList();

        int i = 0;
        Sword sword = CurrentGameState.swords[i];

        int potentialUsage = i == 0 ? sword.baseUsage + 2 : sword.baseUsage + 3;

        foreach (Enemy enemy in enemies)
        {
            int health = enemy.HP;

            if (potentialUsage == 0)
            {
                i++;
                if (i == 3)
                {
                    return false;
                }
                
                sword = CurrentGameState.swords[i];
                if (sword == null)
                {
                    return false;
                }

                potentialUsage = i == 0 ? sword.baseUsage + 2 : sword.baseUsage + 3;
            }

            while (true)
            {              
                health -= sword.baseDamage;
                potentialUsage--;

                if (health <= 0)
                {
                    break;
                }

                if (potentialUsage == 0)
                {
                    // copied oops
                    i++;
                    if (i == 3)
                    {
                        return false;
                    }

                    sword = CurrentGameState.swords[i];
                    if (sword == null)
                    {
                        return false;
                    }

                    potentialUsage = i == 0 ? sword.baseUsage + 2 : sword.baseUsage + 3;
                }
            }
        }

        //check if it isn't too easy
        if (i < 3)
        {
            for (int j = i+1; j<3; j++)
            {
                if (CurrentGameState.swords[j] != null) 
                {
                    potentialUsage += CurrentGameState.swords[j].baseUsage + 3;
                }
            }
            
        }

        Debug.Log("POTENTIAL "+ potentialUsage);
        return potentialUsage < 4;
    }
}

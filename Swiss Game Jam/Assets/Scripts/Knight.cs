using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    private Enemy _currentEnemy;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((_currentEnemy = other.GetComponent<Enemy>()) != null)
        {
            if (CurrentGameState.state == GameState.SEARCHING)
            {
                CurrentGameState.state = GameState.FIGHTING;
            }
        }
        else if (other.GetComponent<KnightStop>() != null && 
            CurrentGameState.state == GameState.RETRIEVING)
        {
            CurrentGameState.state = GameState.STARTDIALOGUE;
        }
    }
}

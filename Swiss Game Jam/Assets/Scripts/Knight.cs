using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    private Enemy _currentEnemy;
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((_currentEnemy = other.GetComponent<Enemy>()) != null)
        {
            if (CurrentGameState.state == GameState.SEARCHING)
            {
                CurrentGameState.state = GameState.FIGHTING;
                _anim.Play("Knight_Attack");
            }
        }
        else if (other.GetComponent<KnightStop>() != null && 
            CurrentGameState.state == GameState.RETRIEVING)
        {
            CurrentGameState.state = GameState.STARTDIALOGUE;
        }
    }

    // Called from Animation Event
    public void HitEnemy()
    {
        Sword currentSword = CurrentGameState.CurrentSword();
        _currentEnemy.HP -= currentSword.baseDamage;

        if (_currentEnemy.HP <= 0)
        {
            Destroy(_currentEnemy.gameObject);
            _currentEnemy = null;
            CurrentGameState.state = GameState.SEARCHING;
        }

        currentSword.usagesLeft--;

        if (currentSword.usagesLeft <= 0)
        {
            Debug.Log("Miecz rozjebany");

            if (CurrentGameState.currentSwordIndex < CurrentGameState.swords.Length - 1)
            {
                Debug.Log(string.Format("Biore miecz nr {0}", ++CurrentGameState.currentSwordIndex));
            }
            else
            {
                Debug.Log("Spierdalam");
                CurrentGameState.state = GameState.RETRIEVING;
                return;
            }
        }

        if (_currentEnemy != null)
        {
            _anim.Play("Knight_Attack");
        }
    }


}

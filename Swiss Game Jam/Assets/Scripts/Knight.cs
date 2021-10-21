using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Knight : MonoBehaviour
{
    private Enemy _currentEnemy;
    private Animator _anim;
    private AudioSource _audio;

    [SerializeField] private AudioClip _swordBreak;
    [SerializeField] private GameObject _MarkupTemplate;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private GameplayManager _gameplayManager;
    [SerializeField] private GameObject _healthBarTemplate;

    private int _baseEnemyHP;
    private GameObject _enemyHealthBar;
    private void Start()
    {
        _anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((_currentEnemy = other.GetComponent<Enemy>()) != null)
        {
            if (CurrentGameState.state == GameState.SEARCHING)
            {
                CurrentGameState.state = GameState.FIGHTING;

                _baseEnemyHP = _currentEnemy.HP;
                _enemyHealthBar = Instantiate(_healthBarTemplate, _currentEnemy.transform);

                _anim.SetInteger("Attacks", _anim.GetInteger("Attacks") + 1);
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
        // decriment attack counter
        if (_anim.GetInteger("Attacks")> 0)
            _anim.SetInteger("Attacks", _anim.GetInteger("Attacks") - 1);


        if (_currentEnemy == null)
            return;

        Sword currentSword = CurrentGameState.CurrentSword();
        _currentEnemy.HP -= currentSword.baseDamage;

        GameObject markup = Instantiate(_MarkupTemplate, _currentEnemy.transform.position, Quaternion.identity);
        markup.GetComponentInChildren<TextMeshPro>().text = currentSword.baseDamage.ToString();

        if (_currentEnemy.HP <= 0)
        {
            Destroy(_enemyHealthBar);
            _enemyHealthBar = null;
            KillEnemy();
        }
        else
        {
            _enemyHealthBar.GetComponent<Healthbar>().ShowState((float)_currentEnemy.HP / (float)_baseEnemyHP);
        }


        currentSword.usagesLeft--;

        if (currentSword.usagesLeft <= 0)
        {
            _audio.clip = _swordBreak;
            _audio.Play();

            if (CurrentGameState.currentSwordIndex < CurrentGameState.swords.Length - 1 &&
                CurrentGameState.swords[CurrentGameState.currentSwordIndex+1] != null)
            {
                // uselss?
            }
            else
            {
                CurrentGameState.state = GameState.RETRIEVING;
                return;
            }
        }

        if (_currentEnemy != null)
        {
            _anim.SetInteger("Attacks", _anim.GetInteger("Attacks") + 1);
        }
    }

    private void KillEnemy()
    {
        // weapon pickup mechanic
        if (_currentEnemy.weapon != null)
        {
            int worstSlot = 0;
            SwordTier worstTier = SwordTier.VICTORINOX;
            for (int i = 0; i < CurrentGameState.swords.Length; i++)
            {
                if (CurrentGameState.swords[i] == null)
                {
                    worstSlot = i;
                    worstTier = SwordTier.NULL;
                    break;
                }

                if (CurrentGameState.swords[i].tier < worstTier)
                {
                    worstSlot = i;
                    worstTier = CurrentGameState.swords[i].tier;
                }
            }

            if (worstTier < _currentEnemy.weapon.tier)
            {
                CurrentGameState.swords[worstSlot] = Instantiate(_currentEnemy.weapon);
                _gameplayManager.UpdateUI();
            }

        }

        Destroy(_currentEnemy.gameObject);
        _currentEnemy = null;

        if (_levelManager.EnemiesRemaining() <= 0)
            CurrentGameState.state = GameState.RETRIEVING;
        else
            CurrentGameState.state = GameState.SEARCHING;
    }
}

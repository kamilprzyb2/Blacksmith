using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Knight : MonoBehaviour
{
    private const float BROKEN_SWORD_DIALOGUE_PROBABILITY = 0.3f;

    private Enemy _currentEnemy;
    private Animator _anim;
    [SerializeField]  private AudioSource _audio;
    [SerializeField]  private AudioSource _audio2;

    [SerializeField] private AudioClip _swordBreak;
    [SerializeField] private List<AudioClip> _hitSounds;


    [SerializeField] private GameObject _MarkupTemplate;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private GameplayManager _gameplayManager;
    [SerializeField] private GameObject _healthBarTemplate;

    private int _baseEnemyHP;
    private GameObject _enemyHealthBar;
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

        _currentEnemy.GetComponentInChildren<Animator>().Play("Goblin Hit");
        _currentEnemy.GetComponent<AudioSource>().Play();

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
        _gameplayManager.SwordUITextGrow(CurrentGameState.currentSwordIndex);

        if (currentSword.usagesLeft <= 0)
        {
            _audio.Stop();
            _audio.clip = _swordBreak;
            _audio.Play();

            if (Random.Range(0f, 1f) < BROKEN_SWORD_DIALOGUE_PROBABILITY)
            {
                _dialogueManager.ShowDialogue(DIALOGUE.FAILURE);
            }

            bool changedSword = false;
            for (int i = CurrentGameState.currentSwordIndex + 1; i < CurrentGameState.swords.Length; i++) 
            {

                if (CurrentGameState.swords[i] != null && CurrentGameState.swords[i].usagesLeft > 0)
                {
                    CurrentGameState.currentSwordIndex = i;
                    changedSword = true;
                    break;
                }
            }

            if (!changedSword)
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
    // Called from Animation Event
    public void PlayAttackSound()
    {
        _audio.Stop();
        _audio.clip = _hitSounds[Random.Range(0, _hitSounds.Count)];
        _audio.Play();
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
                //CurrentGameState.swords[worstSlot].usagesLeft = CurrentGameState.swords[worstSlot].baseUsage;
            }

        }

        _currentEnemy.GetComponentInChildren<Animator>().Play("Goblin Death");
        //Destroy(_currentEnemy.gameObject);
        _currentEnemy = null;

        if (_levelManager.EnemiesRemaining() <= 0)
        {
            _dialogueManager.ShowDialogue(DIALOGUE.SUCCESS);
            CurrentGameState.state = GameState.RETRIEVING;
        }
        else
            CurrentGameState.state = GameState.SEARCHING;
    }
}

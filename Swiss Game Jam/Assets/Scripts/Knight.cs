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
        Debug.Log("Uderzam " + currentSword.swordName);
        _currentEnemy.HP -= currentSword.baseDamage;

        Debug.Log("ENEMY POS " + _currentEnemy.transform.position.x);

        GameObject markup = Instantiate(_MarkupTemplate, _currentEnemy.transform.position, Quaternion.identity);
        markup.GetComponentInChildren<TextMeshPro>().text = currentSword.baseDamage.ToString();

        if (_currentEnemy.HP <= 0)
        {
            KillEnemy();
        }

        currentSword.usagesLeft--;

        if (currentSword.usagesLeft <= 0)
        {
            Debug.Log("Miecz rozjebany");
            _audio.clip = _swordBreak;
            _audio.Play();

            if (CurrentGameState.currentSwordIndex < CurrentGameState.swords.Length - 1 &&
                CurrentGameState.swords[CurrentGameState.currentSwordIndex+1] != null)
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

    private void KillEnemy()
    {
        // weapon pickup mechanic
        if (_currentEnemy.weapon != null)
        {
            int worstSlot = 0;
            SwordTier worstTier = SwordTier.DIAMOND;
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
                Debug.Log(string.Format("Podnosze {0} w slot {1}",
                    _currentEnemy.weapon.swordName,
                    worstSlot));
            }

        }

        Destroy(_currentEnemy.gameObject);
        _currentEnemy = null;
        CurrentGameState.state = GameState.SEARCHING;
    }
}

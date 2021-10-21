using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    private const float DELAY_BETWEEN_SWORD_VALUE_BUMPS = 0.5f;
    private const float TRANSITION_DURATION = 1f;

    [SerializeField] private Sword[] _starterSwords = new Sword[3];

    [SerializeField] private SmithyManager _smithyManager;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private GameObject _knight;
    [SerializeField] private float _knightSpeed = 1f;
    [SerializeField] private float _smoothTime = 0.3f;

    [SerializeField] private Color _selectionColor;

    [SerializeField] private Image[] _swordIcons = new Image[3];
    [SerializeField] private Text[] _swordUsageTexts = new Text[3];

    private Vector3 _cameraStartPos;
    private Vector3 _velocity = Vector3.zero;

    private bool _transitionFlag = false;

    void Start()
    {
        _cameraStartPos = _cameraTransform.position;

        int i = 0; //whatever
        foreach(Sword sword in _starterSwords)
        {
            if (sword != null && i < CurrentGameState.swords.Length)
            {
                CurrentGameState.swords[i] = Instantiate(sword);
                CurrentGameState.swords[i].usagesLeft = CurrentGameState.swords[i].baseUsage;
            }
            i++;
        }

        CurrentGameState.state = GameState.SEARCHING;
    }
    void Update()
    {
        UpdateUI();

        switch (CurrentGameState.state)
        {
            case GameState.STARTDIALOGUE:
                {
                    if (!_transitionFlag)
                    {
                        _knight.GetComponent<Animator>().SetBool("MoveRight", false);
                        _transitionFlag = true;
                        TransitionTo(GameState.SMITHING);
                    }

                    MoveCameraToSmith();

                    break; 
                }
            case GameState.AFTERDIALOGUE:
                {
                    if (!_transitionFlag)
                    {
                        // moved
                        // CurrentGameState.ResetSwordUsages();
                        CurrentGameState.currentSwordIndex = 0;

                        TransitionTo(GameState.SEARCHING);
                        _levelManager.ChangeLevel();

                        _transitionFlag = true;
                    }

                    break;
                }
            case GameState.SMITHING:
            {
                    if (_transitionFlag)
                    {
                        _smithyManager.Kickoff();                      
                    }

                    MoveCameraToSmith();
                    // All smithing logic in SmithyManager
                    _transitionFlag = false;

                    break;
            }
            case GameState.SEARCHING:
                {
                    _knight.transform.position =
                        _knight.transform.position + Vector3.left * _knightSpeed * Time.deltaTime;

                    MoveCameraWithKnight();


                    _knight.GetComponent<Animator>().SetBool("MoveLeft", true);

                    _transitionFlag = false;

                    break;
                }
            case GameState.FIGHTING:
                {
                    if (!_transitionFlag)
                    {
                        _knight.GetComponent<Animator>().SetBool("MoveLeft", false);
                        _transitionFlag = true;
                    }

                    break;
                }
            case GameState.RETRIEVING:
                {
                    _knight.transform.position =
                        _knight.transform.position + Vector3.right * _knightSpeed * Time.deltaTime;

                    MoveCameraWithKnight();

                    _transitionFlag = false;

                    _knight.GetComponent<Animator>().SetBool("MoveRight", true);

                    break;
                }

        }        
    }
    void MoveCameraWithKnight()
    {
        Vector3 cameraTarget = new Vector3(
                        _knight.transform.position.x,
                        _cameraTransform.position.y);

        _cameraTransform.position = Vector3.SmoothDamp(
            _cameraTransform.position,
            cameraTarget,
            ref _velocity,
            _smoothTime);
    }
    void MoveCameraToSmith()
    {
        _cameraTransform.position = Vector3.SmoothDamp(
            _cameraTransform.position,
            _cameraStartPos,
            ref _velocity,
            _smoothTime);
    }
    public void UpdateUI()
    {
        for(int i = 0; i < CurrentGameState.swords.Length; i++)
        {
            if (CurrentGameState.swords[i] == null)           
            {
                // invisible
                _swordIcons[i].color = new Color(_swordIcons[i].color.r, _swordIcons[i].color.g, _swordIcons[i].color.b, 0f);
                _swordUsageTexts[i].text = "";
            }
            else 
            {
                // visible
                _swordIcons[i].color = new Color(_swordIcons[i].color.r, _swordIcons[i].color.g, _swordIcons[i].color.b, 255f);
                _swordIcons[i].sprite = CurrentGameState.swords[i].usagesLeft > 0 ?
                    CurrentGameState.swords[i].sprite : CurrentGameState.swords[i].damagedSprite;
                _swordUsageTexts[i].text = CurrentGameState.swords[i].usagesLeft == 0 ? "X" : CurrentGameState.swords[i].usagesLeft.ToString() ;

                if (i == CurrentGameState.currentSwordIndex)
                {
                    _swordIcons[i].rectTransform.localScale = new Vector3(2.3f, 2.3f);
                    _swordUsageTexts[i].color = _selectionColor;
                }
                else
                {
                    _swordIcons[i].rectTransform.localScale = new Vector3(1.7f, 1.7f);
                    _swordUsageTexts[i].color = Color.white;
                }
            }
        }
    }
    public IEnumerator BumpSwordTo(int slotIndex, int value)
    {
        // safety
        if (CurrentGameState.swords[slotIndex] == null || value < CurrentGameState.swords[slotIndex].usagesLeft)
        {
            Debug.LogError("INVALID SWORD BUMP REQUEST!!!");
            yield return null;
        }
        else
        {
            while (CurrentGameState.swords[slotIndex].usagesLeft != value)
            {

                CurrentGameState.swords[slotIndex].usagesLeft++;
                SwordUITextGrow(slotIndex);

                yield return new WaitForSeconds(DELAY_BETWEEN_SWORD_VALUE_BUMPS);
            }
        }
    }

    public void SwordUITextGrow(int slotIndex)
    {
        _swordUsageTexts[slotIndex].GetComponent<Animator>().Play("UI_Usage_Text_Grow");
    }

    private void TransitionTo(GameState state)
    {
        switch (state)
        {
            case GameState.SMITHING:
                {
                    _dialogueManager.ShowDialogue(DIALOGUE.BLACKSMITH);
                    break;
                }
            case GameState.SEARCHING:
                {
                    _dialogueManager.ShowDialogue(DIALOGUE.THANKS);
                    break;
                }
        }
        StartCoroutine(DelayedTransition(state));
    }

    IEnumerator DelayedTransition(GameState state)
    {
        yield return new WaitForSeconds(TRANSITION_DURATION);
        CurrentGameState.state = state;
    }

}

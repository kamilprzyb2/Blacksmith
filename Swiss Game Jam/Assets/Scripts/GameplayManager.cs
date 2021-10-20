using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private Sword[] _starterSwords = new Sword[3];

    [SerializeField] private SmithyManager _smithyManager;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private GameObject _knight;
    [SerializeField] private float _knightSpeed = 1f;
    [SerializeField] private float _smoothTime = 0.3f;

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
            }
            i++;
        }

        CurrentGameState.state = GameState.SEARCHING;
    }

    void Update()
    {
        switch (CurrentGameState.state)
        {
            case GameState.STARTDIALOGUE:
                {
                    if (!_transitionFlag)
                    {
                        _knight.GetComponent<Animator>().SetBool("MoveRight", false);
                        _transitionFlag = true;
                    }

                    if (Input.anyKeyDown)
                    {
                        CurrentGameState.state = GameState.SMITHING;
                    }

                    MoveCameraToSmith();

                    break; 
                }
            case GameState.AFTERDIALOGUE:
                {
                    if (!_transitionFlag)
                    {
                        CurrentGameState.ResetSwordUsages();
                        CurrentGameState.currentSwordIndex = 0;

                        _levelManager.ChangeLevel();

                        _transitionFlag = true;
                    }

                    if (Input.anyKeyDown)
                    {
                        CurrentGameState.state = GameState.SEARCHING;
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
}

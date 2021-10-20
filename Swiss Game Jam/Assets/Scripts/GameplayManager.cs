using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private Sword[] _starterSwords = new Sword[3];

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private GameObject _knight;
    [SerializeField] private float _knightSpeed = 1f;
    [SerializeField] private float _smoothTime = 0.3f;

    private Vector3 _cameraStartPos;
    private Vector3 _velocity = Vector3.zero;

    private bool _messageSent = false;

    void Start()
    {
        _cameraStartPos = _cameraTransform.position;
        CurrentGameState.swords = _starterSwords;
        CurrentGameState.state = GameState.SEARCHING;
    }

    void Update()
    {
        switch (CurrentGameState.state)
        {
            case GameState.STARTDIALOGUE:
                {
                    if (!_messageSent)
                    {
                        Debug.Log("CHUJU NAPRAW MI MIECZ");
                        _messageSent = true;
                    }

                    if (Input.anyKeyDown)
                    {
                        CurrentGameState.state = GameState.SMITHING;
                    }
                    break; 
                }
            case GameState.AFTERDIALOGUE:
                {
                    if (!_messageSent)
                    {
                        Debug.Log("DZIÊKI CHUJU");
                        _messageSent = true;
                    }

                    if (Input.anyKeyDown)
                    {
                        CurrentGameState.state = GameState.SEARCHING;
                    }
                    break;
                }
            case GameState.SMITHING:
            {
                    MoveCameraToSmith();
                    // All smithing logic in SmithyManager
                    _messageSent = false;
                    break;
            }
            case GameState.SEARCHING:
                {
                    _knight.transform.position =
                        _knight.transform.position + Vector3.left * _knightSpeed * Time.deltaTime;

                    MoveCameraWithKnight();

                    _messageSent = false;

                    //temp shit

                    CurrentGameState.currentSwordIndex = 0;

                    foreach (Sword sword in CurrentGameState.swords)
                    {
                        if (sword != null)
                        {
                            sword.usagesLeft = sword.baseUsage;
                        }
                    }

                    break;
                }
            case GameState.FIGHTING:
                {
                    if (!_messageSent)
                    {
                        Debug.Log("JAKAŒ WALKA KURWA TEN");
                        _messageSent = true;
                    }

                    break;
                }
            case GameState.RETRIEVING:
                {
                    _knight.transform.position =
                        _knight.transform.position + Vector3.right * _knightSpeed * Time.deltaTime;

                    MoveCameraWithKnight();

                    _messageSent = false;

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

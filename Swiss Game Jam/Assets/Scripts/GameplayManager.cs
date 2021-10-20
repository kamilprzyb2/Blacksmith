using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Sword _starterSword;
    [SerializeField] private GameObject _knight;
    [SerializeField] private float _knightSpeed = 1f;
    [SerializeField] private float _smoothTime = 0.3f;

    private Vector3 _cameraStartPos;
    private Vector3 _velocity = Vector3.zero;
    void Start()
    {
        _cameraStartPos = _cameraTransform.position;

        Debug.Log("CHUJU NAPRAW MI MIECZ");

        CurrentGameState.swords[0] = _starterSword;

    }

    void Update()
    {
        switch (CurrentGameState.state)
        {
            case GameState.STARTDIALOGUE:
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        CurrentGameState.state = GameState.SMITHING;
                    }
                    break; 
                }
            case GameState.SEARCHING:
                {
                    _knight.transform.position =
                        _knight.transform.position + Vector3.left * _knightSpeed * Time.deltaTime;

                    Vector3 cameraTarget = new Vector3(
                        _knight.transform.position.x,
                        _cameraTransform.position.y);

                    _cameraTransform.position = Vector3.SmoothDamp(
                        _cameraTransform.position,
                        cameraTarget,
                        ref _velocity,
                        _smoothTime);

                    Debug.Log(cameraTarget);

                    break;
                }
            case GameState.FIGHTING:
                {
                    Debug.Log("FIGHT");
                    break;
                }

        }        
    }
}

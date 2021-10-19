using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmithyManager : MonoBehaviour
{
    private const float MARGIN_BETWEEN_CUTS = 1f;
    private const float HIT_COOLDOWN = 0.05f;
    private const KeyCode ACTION_KEY = KeyCode.Space;
    private const KeyCode ACTION_KEY2 = KeyCode.Mouse0;

    [SerializeField] private int _numberOfCuts = 1;
    [SerializeField] private float _timeToPrepare = 3f;
    [SerializeField] private float _shaftSpeed = 2f;

    [SerializeField] private GameObject _cutTemplate;
    [SerializeField] private GameObject _shaftTemplate;

    [SerializeField] private GameObject _barStart;
    [SerializeField] private GameObject _barEnd;

    [SerializeField] private Animator _blacksmithAnim;
    
    [SerializeField] private GameObject _cutsParent; 

    private List<float> _cutsList;
    private GameObject _shaft;
    private bool _running = false;
    private bool _cooldown = false;
    private int _clicksLeft = 0;
    private int _score = 0;

    void Start()
    {
        _cutsList = new List<float>();
        GenerateCuts();
        DrawCuts();

        _shaft = Instantiate(_shaftTemplate, _barStart.transform.position, Quaternion.identity);
        StartCoroutine(Run());
    }

    void Update()
    {
        if (_running)
        {

            if ((Input.GetKeyDown(ACTION_KEY) || Input.GetKeyDown(ACTION_KEY2))
                    && _clicksLeft > 0 && !_cooldown)
            {
                if (_cutsList.Count == 0)
                {
                    Debug.LogError("OUT OF CUTS?!");
                    return;
                }

                float nearestCut = _cutsList[0];

                float distance = Mathf.Abs(nearestCut - _shaft.transform.position.x);
                Debug.Log(distance);
                _score += (int) (distance * 10);
                _cutsList.Remove(nearestCut);
                _clicksLeft--;

                _blacksmithAnim.Play("Blacksmith_Hit");
            }


            if (_shaft.transform.position.x >= _barEnd.transform.position.x)
            {
                _running = false;
                Debug.Log(string.Format("Your score is {0}", _score));
            }
            else
            {
                _shaft.transform.position = Vector3.MoveTowards(
                    _shaft.transform.position,
                    _barEnd.transform.position,
                    _shaftSpeed * Time.deltaTime);
            }
        }
    }

    void GenerateCuts()
    {
        while (_cutsList.Count < _numberOfCuts)
        {
            float randomX = Random.Range(
                _barStart.transform.position.x + MARGIN_BETWEEN_CUTS,
                _barEnd.transform.position.x - MARGIN_BETWEEN_CUTS);

            bool isGood = true;

            foreach (var cut in _cutsList)
            {
                if (Mathf.Abs(randomX - cut) < MARGIN_BETWEEN_CUTS)
                    isGood = false;
            }

            if (isGood)
                _cutsList.Add(randomX);
        }

        _cutsList.Sort();
    }
    void DrawCuts()
    {
        foreach (var cut in _cutsList)
        {
            Instantiate(
                _cutTemplate, 
                new Vector2(cut, _barStart.transform.position.y), 
                Quaternion.identity)
                    .transform.SetParent(_cutsParent.transform);
        }
    }
    IEnumerator Run()
    {
        yield return new WaitForSeconds(_timeToPrepare);
        _running = true;
        _clicksLeft = _numberOfCuts;
    }

    IEnumerator Cooldown()
    {
        _cooldown = true;
        yield return new WaitForSeconds(HIT_COOLDOWN);
        _cooldown = false;
    }
}
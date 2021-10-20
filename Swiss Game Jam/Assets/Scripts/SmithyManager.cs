using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SmithyManager : MonoBehaviour
{
    private const float MARGIN_BETWEEN_CUTS = 0.3f;
    private const float HIT_COOLDOWN = 0.05f;
    private const float SHAFT_GLOW_DISTANCE = 1f;
    private const KeyCode ACTION_KEY = KeyCode.Space;
    private const KeyCode ACTION_KEY2 = KeyCode.Mouse0;

    public List<Sword> swords;
    public int currentSwordIndex = -1;

    [SerializeField] private int _numberOfCuts = 1;
    [SerializeField] private float _timeToPrepare = 3f;
    [SerializeField] private float _shaftSpeed = 2f;

    [SerializeField] private GameObject _cutTemplate;
    [SerializeField] private GameObject _shaftTemplate;
    [SerializeField] private GameObject _blastTemplate;

    [SerializeField] private GameObject _barStart;
    [SerializeField] private GameObject _barEnd;
    [SerializeField] private GameObject _spawnCutsY;
    [SerializeField] private GameObject _spawnBlastY;

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
        //GameplayManager.SetUp();
        //swords.Add(GameplayManager.swords[0]);

        //Sword sword = swords[0];
        //Debug.Log(string.Format("Repairing {0}", sword.swordName));
        //Play(sword.requiresCuts, sword.shaftSpeed);
    }

    void Play(int numberOfCuts, float shaftSpeed)
    {
        _numberOfCuts = numberOfCuts;
        _shaftSpeed = shaftSpeed;

        _cutsList = new List<float>();
        GenerateCuts();
        DrawCuts();

        _shaft = Instantiate(_shaftTemplate, _barStart.transform.position, Quaternion.identity);
        StartCoroutine(Run());
    }


    void Update()
    {
        if (CurrentGameState.state != GameState.SMITHING)
        {
            return;
        }

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    SceneManager.LoadScene("Test");
        //}

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

                float nextCut = _cutsList[_cutsList.Count - _clicksLeft];

                float distance = Mathf.Abs(nextCut - _shaft.transform.position.x);
                Debug.Log(distance);
                _score += (int) (distance * 100);
                _clicksLeft--;

                _blacksmithAnim.Play("Blacksmith_Hit", 0, 0);
                Cooldown();
            }


            if (_shaft.transform.position.x >= _barEnd.transform.position.x)
            {
                _running = false;
                _score /= _numberOfCuts;
                Debug.Log(string.Format("Your score is {0}", _score));

                if (currentSwordIndex < CurrentGameState.swords.Length - 1)
                {
                    if (CurrentGameState.swords[currentSwordIndex + 1] != null)
                    {
                        currentSwordIndex++;
                        Play(CurrentGameState.swords[currentSwordIndex].requiresCuts,
                        CurrentGameState.swords[currentSwordIndex].shaftSpeed);
                    }
                    else
                    {
                        CurrentGameState.state = GameState.SEARCHING;
                    }
                }
                else
                {
                    CurrentGameState.state = GameState.SEARCHING;
                }
            }
            else
            {
                _shaft.transform.position = Vector3.MoveTowards(
                    _shaft.transform.position,
                    _barEnd.transform.position,
                    _shaftSpeed * Time.deltaTime);
            }

            ShaftState();
        }
        else if (currentSwordIndex < 0)
        {
            if (CurrentGameState.swords[0] != null)
            {
                currentSwordIndex = 0;
                Play(CurrentGameState.swords[currentSwordIndex].requiresCuts,
                    CurrentGameState.swords[currentSwordIndex].shaftSpeed);
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
                new Vector2(cut, _spawnCutsY.transform.position.y), 
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
    private void ShaftState()
    {
        bool shouldGlow = false;
        foreach(var cut in _cutsList)
        {
            if ((cut - _shaft.transform.position.x < SHAFT_GLOW_DISTANCE) &&
               (cut - _shaft.transform.position.x > 0))
            {
                shouldGlow = true;
            }
        }
        _shaft.GetComponent<Animator>().SetBool("Glow", shouldGlow);
    }
    private void ResetBlacksmith()
    {
        Destroy(_shaft);
        foreach(Transform cut in _cutsParent.transform)
        {
            Destroy(cut.gameObject);
        }
    }
    public void SpawnBlast()
    {
        Instantiate(
            _blastTemplate,
            new Vector2(_shaft.transform.position.x, _spawnBlastY.transform.position.y),
            Quaternion.identity);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SmithyManager : MonoBehaviour
{
    private const float MARGIN_BETWEEN_CUTS = 1f;
    private const int GENERATOR_TIMEOUT_AFTER = 40;
    private const float HIT_COOLDOWN = 0.05f;
    private const float SHAFT_GLOW_DISTANCE = 0.6f;
    private const float DELAY_BETWEEN_SWORDS = 0.5f;
    private const float DELAY_FOR_BAR_ANIM = 1f;

    public List<Sword> swords;
    public int currentSwordIndex = -1;

    private int _numberOfCuts = 1;
    [SerializeField] private int _secondsToPrepare = 5;
    private float _shaftSpeed = 2f;

    [SerializeField] private GameObject _cutTemplate;
    [SerializeField] private GameObject _shaftTemplate;
    [SerializeField] private GameObject _blastTemplate;

    [SerializeField] private GameObject _bar;
    [SerializeField] private GameObject _barStart;
    [SerializeField] private GameObject _barEnd;
    [SerializeField] private GameObject _spawnCutsY;
    [SerializeField] private GameObject _spawnBlastY;

    [SerializeField] private Animator _blacksmithAnim;
    
    [SerializeField] private GameObject _cutsParent;

    [SerializeField] private Text _uiText;
    [SerializeField] private Color _colorGradePoor;
    [SerializeField] private Color _colorGradeOK;
    [SerializeField] private Color _colorGradeGood;
    [SerializeField] private Color _colorGradePerfect;

    private List<float> _cutsList;
    private GameObject _shaft;
    private bool _running = false;
    private bool _cooldown = false;
    private int _clicksLeft = 0;
    private int _score = 0;
    void Play(int numberOfCuts, float shaftSpeed)
    {
        _numberOfCuts = numberOfCuts;
        _shaftSpeed = shaftSpeed;

        _cutsList = new List<float>();

        ResetBlacksmith();
        GenerateCuts();
        DrawCuts();

        _uiText.text = "";
        _uiText.GetComponent<Animator>().Play("Text_Idle");

        _shaft = Instantiate(_shaftTemplate, _barStart.transform.position, Quaternion.identity);
        //_shaft.transform.SetParent(_barStart.transform);

        // this is so fucking retarded 
        var dumb = new GameObject();
        dumb.transform.parent = _bar.transform;
        _shaft.transform.parent = dumb.transform;

        _bar.GetComponent<Animator>().Play("Bar_Show");

        StartCoroutine(Run());
    }

    public void Kickoff()
    {
        if (CurrentGameState.swords[0] == null)
        {
            Debug.LogError("SWORD 0 IS NULL?!");
            return;
        }
        currentSwordIndex = 0;  

        Play(CurrentGameState.swords[currentSwordIndex].requiresCuts,
             CurrentGameState.swords[currentSwordIndex].shaftSpeed);
    }

    void Update()
    {
        // safety
        if (CurrentGameState.state != GameState.SMITHING)
        {
            return;
        }

        if (_running)
        {
            if (_clicksLeft == 0)
            {
                _score /= _numberOfCuts;
                ApplyScore();
                _clicksLeft = -1;
            }
            else if (Input.anyKeyDown && _clicksLeft > 0 && !_cooldown)
            {
                if (_cutsList.Count == 0)
                {
                    Debug.LogError("OUT OF CUTS?!");
                    return;
                }

                _blacksmithAnim.Play("Blacksmith_Hit", 0, 0);                
                Cooldown();
            }

            if (_shaft.transform.position.x >= _barEnd.transform.position.x)
            {
                _running = false;

                if (_clicksLeft > 0)
                {
                    _score = int.MaxValue;
                    ApplyScore();
                }

                StartCoroutine(FinishSword());

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
    }
    void GenerateCuts()
    {
        int count = 0;
        while (_cutsList.Count < _numberOfCuts)
        {
            if (count > GENERATOR_TIMEOUT_AFTER)
            {
                _cutsList.Clear();
                count = 0;
            }

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

            count++;
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
        yield return new WaitForSeconds(DELAY_FOR_BAR_ANIM);
        int secondsLeft = _secondsToPrepare;
        _uiText.color = Color.white;

        while (secondsLeft > 0)
        {
            _uiText.text = secondsLeft.ToString();
            secondsLeft--;
            yield return new WaitForSeconds(1f);
        }
        
            _uiText.text = secondsLeft.ToString();

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
        _score = 0;
    }
    private void ApplyScore()
    {
        Sword currentSword = CurrentGameState.swords[currentSwordIndex];
        Grade grade = Grade.POOR;

        if (_score < 20)
            grade = Grade.PERFECT;
        else if (_score < 30)
            grade = Grade.GOOD;
        else if (_score < 70)
            grade = Grade.OK;

        switch (grade)
        {
            case Grade.POOR:
                {
                    _uiText.text = "POOR";
                    _uiText.color = _colorGradePoor;
                    currentSword.addedUsage = 0;
                    break;
                }
            case Grade.OK:
                {
                    _uiText.text = "OK";
                    _uiText.color = _colorGradeOK;
                    currentSword.addedUsage = 1;
                    break;
                }
            case Grade.GOOD:
                {
                    _uiText.text = "GOOD";
                    _uiText.color = _colorGradeGood;
                    currentSword.addedUsage = 2;
                    break;
                }
            case Grade.PERFECT:
                {
                    _uiText.text = "PERFECT";
                    _uiText.color = _colorGradePerfect;
                    currentSword.addedUsage = 3;
                    break;
                }
        }


    }
    IEnumerator FinishSword()
    {
        _uiText.GetComponent<Animator>().Play("Text_Fade_Out");
        yield return new WaitForSeconds(DELAY_BETWEEN_SWORDS);

        if (currentSwordIndex < CurrentGameState.swords.Length - 1)
        {
            if (CurrentGameState.swords[currentSwordIndex + 1] != null)
            {
                currentSwordIndex++;
                //ResetBlacksmith();
                Play(CurrentGameState.swords[currentSwordIndex].requiresCuts,
                CurrentGameState.swords[currentSwordIndex].shaftSpeed);
            }
            else
            {
                CurrentGameState.state = GameState.AFTERDIALOGUE;
                //ResetBlacksmith();
                
                currentSwordIndex = -1;
                _bar.GetComponent<Animator>().Play("Bar_Hide");
            }
        }
        else
        {
            CurrentGameState.state = GameState.AFTERDIALOGUE;
            //ResetBlacksmith();
            currentSwordIndex = -1;

            _bar.GetComponent<Animator>().Play("Bar_Hide");
        }

    }
    public void MakeACut()
    {

        float nextCut = _cutsList[_cutsList.Count - _clicksLeft];

        float distance = Mathf.Abs(nextCut - _shaft.transform.position.x);
        _score += (int)(distance * 100);
        _clicksLeft--;

        Instantiate(
            _blastTemplate,
            new Vector2(_shaft.transform.position.x, _spawnBlastY.transform.position.y),
            Quaternion.identity);
    }
}

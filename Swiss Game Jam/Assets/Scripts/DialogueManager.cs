using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum DIALOGUE
{
    BLACKSMITH, FAILURE, SUCCESS, THANKS
}
public class DialogueManager : MonoBehaviour
{
    private const float DELAY = 3f;
    private TextMeshPro _textMesh;

    [SerializeField] private List<string> _dialoguesBlacksmith;
    [SerializeField] private List<string> _dialoguesFailure;
    [SerializeField] private List<string> _dialoguesSuccess;
    [SerializeField] private List<string> _dialoguesThanks;

    void Start()
    {
        _textMesh = GetComponent<TextMeshPro>();
    }

    public void ShowDialogue(DIALOGUE dialogue)
    {
        string randomText = "";
        switch (dialogue)
        {
            case DIALOGUE.BLACKSMITH: 
                {
                    randomText = _dialoguesBlacksmith[Random.Range(0, _dialoguesBlacksmith.Count)]; 
                    break; 
                }
            case DIALOGUE.FAILURE:
                {
                    randomText = _dialoguesFailure[Random.Range(0, _dialoguesFailure.Count)];
                    break;
                }
            case DIALOGUE.SUCCESS:
                {
                    randomText = _dialoguesSuccess[Random.Range(0, _dialoguesSuccess.Count)];
                    break;
                }
            case DIALOGUE.THANKS:
                {
                    randomText = _dialoguesThanks[Random.Range(0, _dialoguesThanks.Count)];
                    break;
                }
        }

            StartCoroutine(Show(randomText));
    }

    private IEnumerator Show(string text)
    {
        _textMesh.text = text;
        yield return new WaitForSeconds(DELAY);
        if (_textMesh.text == text)
            _textMesh.text = "";
    }
}

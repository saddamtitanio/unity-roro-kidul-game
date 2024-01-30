using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(3, 10)]
    public string text;
    public bool isChoice;
    public string rightChoiceText;
    public string wrongChoiceText;
}

public class Dialogue1 : MonoBehaviour
{
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI choicesText;
    public float fadeInSpeed = 1.5f;
    public float fadeOutSpeed = 1.5f;
    public string nextScene;

    [FormerlySerializedAs("dialogueLines")]
    public DialogueLine[] dialogue;

    private bool isAnimating;
    private int currentLineIndex = 0;
    private bool isDialogueTriggered = false;

    void Start()
    {
        speakerNameText.alpha = 0f;
        dialogueText.alpha = 0f;
        choicesText.alpha = 0f;
    }

    void Update()
    {
        if (!isDialogueTriggered && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartDialogue());
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !isAnimating)
        {
            if (currentLineIndex < dialogue.Length)
            {
                if (dialogue[currentLineIndex].isChoice)
                {
                    StartCoroutine(HandleChoiceWithFade());
                }
                else
                {
                    StartCoroutine(ContinueDialogue());
                }
            }
        }
    }

    IEnumerator StartDialogue()
    {
        isDialogueTriggered = true;

        SetSpeakerName(dialogue[currentLineIndex].speakerName);
        SetDialogueText(dialogue[currentLineIndex].text);
        SetChoicesText("");

        isAnimating = true;
        while (speakerNameText.alpha < 1f || dialogueText.alpha < 1f)
        {
            speakerNameText.alpha += Time.deltaTime * fadeInSpeed;
            dialogueText.alpha += Time.deltaTime * fadeInSpeed;
            yield return null;
        }
        isAnimating = false;
    }

    IEnumerator ContinueDialogue()
    {
        isAnimating = true;
        while (speakerNameText.alpha > 0f || dialogueText.alpha > 0f)
        {
            speakerNameText.alpha -= Time.deltaTime * fadeOutSpeed;
            dialogueText.alpha -= Time.deltaTime * fadeOutSpeed;
            yield return null;
        }

        currentLineIndex++;
        if (currentLineIndex < dialogue.Length)
        {
            SetSpeakerName(dialogue[currentLineIndex].speakerName);
            SetDialogueText(dialogue[currentLineIndex].text);
        }
        else
        {
            Debug.Log("End of dialogue!");
            if (!string.IsNullOrEmpty(nextScene))
            {
                SceneManager.LoadScene(nextScene);
            }
        }

        while (speakerNameText.alpha < 1f || dialogueText.alpha < 1f)
        {
            speakerNameText.alpha += Time.deltaTime * fadeInSpeed;
            dialogueText.alpha += Time.deltaTime * fadeInSpeed;
            yield return null;
        }
        isAnimating = false;
    }

    void SetSpeakerName(string name)
    {
        speakerNameText.text = name;
    }

    void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }

    void SetChoicesText(string text)
    {
        choicesText.text = text;
        StartCoroutine(FadeChoices());
    }

    IEnumerator FadeChoices()
    {
        float targetAlpha = choicesText.text.Length > 0 ? 1f : 0f;

        while (!Mathf.Approximately(choicesText.alpha, targetAlpha))
        {
            choicesText.alpha = Mathf.MoveTowards(choicesText.alpha, targetAlpha, Time.deltaTime * fadeInSpeed);
            yield return null;
        }
    }

    IEnumerator HandleChoiceWithFade()
    {
        SetChoicesText("1. " + dialogue[currentLineIndex].rightChoiceText + "\n" +
                        "2. " + dialogue[currentLineIndex].wrongChoiceText);
        yield return StartCoroutine(WaitForChoiceInput());
        SetChoicesText("");
    }

    IEnumerator WaitForChoiceInput()
    {
        while (!Input.GetKeyDown(KeyCode.Alpha1) && !Input.GetKeyDown(KeyCode.Alpha2))
        {
            yield return null;
        }
        int chosenOption = Input.GetKeyDown(KeyCode.Alpha1) ? 1 : 2;
        if (chosenOption == 1)
        {
            StartCoroutine(ContinueDialogue());
        }
        else
        {
            if (!string.IsNullOrEmpty(nextScene))
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DialogueController2 : MonoBehaviour
{
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI choicesText;
    public float fadeInSpeed = 1.5f;
    public float fadeOutSpeed = 1.5f;
    public string nextScene;
    public string gameOverScene;

    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        [TextArea(3, 10)]
        public string text;
        public bool isChoice;
        public string rightChoiceText;
        public string wrongChoiceText;
        public Color speakerColor; 
        public float speakerNamePosX = 0f;
    }

    [FormerlySerializedAs("dialogueLines")]
    public DialogueLine[] dialogue;

    private bool isAnimating;
    private int currentLineIndex = 0;
    private bool isDialogueTriggered = false;

    void Start()
    {
        InitializeUI();
    }

    void Update()
    {
        if (!isDialogueTriggered && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartDialogue());
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !isAnimating)
        {
            ProcessDialogueInput();
        }
    }

    void InitializeUI()
    {
        SetUIAlpha(0f);
    }

    void SetUIAlpha(float alpha)
    {
        speakerNameText.alpha = alpha;
        dialogueText.alpha = alpha;
        choicesText.alpha = alpha;
    }

    void ProcessDialogueInput()
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

    IEnumerator StartDialogue()
    {
        isDialogueTriggered = true;

        SetSpeakerAndText(dialogue[currentLineIndex].speakerName, dialogue[currentLineIndex].text, dialogue[currentLineIndex].speakerColor, dialogue[currentLineIndex].speakerNamePosX);
        SetChoicesText("");

        isAnimating = true;
        yield return StartCoroutine(FadeInUI());
        isAnimating = false;
    }

    IEnumerator ContinueDialogue()
    {
        isAnimating = true;

        yield return StartCoroutine(FadeOutUI());

        MoveToNextLineOrScene();

        yield return StartCoroutine(FadeInUI());

        isAnimating = false;
    }

    void MoveToNextLineOrScene()
    {
        currentLineIndex++;
        if (currentLineIndex < dialogue.Length)
        {
            SetSpeakerAndText(dialogue[currentLineIndex].speakerName, dialogue[currentLineIndex].text, dialogue[currentLineIndex].speakerColor, dialogue[currentLineIndex].speakerNamePosX);
        }
        else
        {
            Debug.Log("End of dialogue!");

            if (!string.IsNullOrEmpty(nextScene))
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    void SetSpeakerAndText(string name, string text, Color color, float posX)
    {
        SetSpeakerName(dialogue[currentLineIndex].speakerName, dialogue[currentLineIndex].speakerColor, dialogue[currentLineIndex].speakerNamePosX);

        SetDialogueText(text);
    }

    void SetSpeakerName(string name, Color color, float posX)
    {
        speakerNameText.text = name;
        speakerNameText.color = color;
        RectTransform rectTransform = speakerNameText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(posX, rectTransform.anchoredPosition.y);
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
        SetChoicesText($"1. {dialogue[currentLineIndex].rightChoiceText}\n2. {dialogue[currentLineIndex].wrongChoiceText}");

        yield return StartCoroutine(WaitForChoiceInput());

        SetChoicesText("");
    }

    IEnumerator WaitForChoiceInput()
{
    bool validInput = false;

    while (!validInput)
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            validInput = true;
            if (!string.IsNullOrEmpty(gameOverScene))
            {
                SceneManager.LoadScene(gameOverScene);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            validInput = true;
            StartCoroutine(ContinueDialogue());
        }

        yield return null;
    }
}


    IEnumerator FadeInUI()
    {
        float targetAlpha = 1f;

        while (speakerNameText.alpha < targetAlpha || dialogueText.alpha < targetAlpha)
        {
            speakerNameText.alpha += Time.deltaTime * fadeInSpeed;
            dialogueText.alpha += Time.deltaTime * fadeInSpeed;
            yield return null;
        }
    }

    IEnumerator FadeOutUI()
    {
        float targetAlpha = 0f;

        while (speakerNameText.alpha > targetAlpha || dialogueText.alpha > targetAlpha)
        {
            speakerNameText.alpha -= Time.deltaTime * fadeOutSpeed;
            dialogueText.alpha -= Time.deltaTime * fadeOutSpeed;
            yield return null;
        }
    }
}

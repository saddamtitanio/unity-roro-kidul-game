using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnotherDialogueScript : MonoBehaviour {

  public TMP_Text dialogueText;
  public string[] dialogueLines;

  int currentLine = 0;
  float textFadeDuration = 0.5f;

  void Update() {
    if(Input.GetKeyDown(KeyCode.Space)) {
      StartCoroutine(FadeText());
    }
  }

  IEnumerator FadeText() {
    // Fade out old text
    for(float t=1f; t>=0; t-=Time.deltaTime/textFadeDuration) {
      dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, t);
      yield return null;
    }
    
    // Update text
    DisplayNextLine();

    // Fade in new text
    for(float t=0; t<=1; t+=Time.deltaTime/textFadeDuration) {
      dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, t);
      yield return null;
    }
  }

  void DisplayNextLine() {
    if(currentLine < dialogueLines.Length) {
      dialogueText.text = dialogueLines[currentLine];
      currentLine++;
    } else {
      dialogueText.text = "";
    }
  }

}
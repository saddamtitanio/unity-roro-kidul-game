using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI additionalText;

    public float fadeInDuration = 1.5f;
    public string nextSceneName = "";

    void Start()
    {
        Color initialColor = gameOverText.color;
        initialColor.a = 0f;
        gameOverText.color = initialColor;

        initialColor = additionalText.color;
        initialColor.a = 0f;
        additionalText.color = initialColor;

        StartCoroutine(FadeInText(gameOverText));

        float delay = fadeInDuration;
        StartCoroutine(FadeInText(additionalText, delay));
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadNextScene();
        }
    }

    IEnumerator FadeInText(TextMeshProUGUI textMeshPro, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;
        Color textColor = textMeshPro.color;

        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            textColor.a = alpha;
            textMeshPro.color = textColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        textColor.a = 1f;
        textMeshPro.color = textColor;
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}

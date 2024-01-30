using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    private int buttonIndex = 0;

    public Button[] GetAllButtons()
    {
        return GetComponentsInChildren<Button>();
    }

    void initOptions()
    {
        Button[] options = GetAllButtons();

        AdjustTransparency(options[0], 1f);

        for (int i = 1; i < options.Length; i++)
        {
            AdjustTransparency(options[i], 0.3f);
        }
    }

    void Awake()
    {
        initOptions();
    }

    private void Start()
    {
        buttonIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Button[] options = GetAllButtons();

        if (buttonIndex >= 0 && buttonIndex < options.Length)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                AdjustTransparency(options[buttonIndex], 0.3f);

                buttonIndex = Mathf.Min(buttonIndex + 1, options.Length - 1);

                AdjustTransparency(options[buttonIndex], 1.0f);
            }
        }

        if (buttonIndex >= 0 && buttonIndex < options.Length)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                AdjustTransparency(options[buttonIndex], 0.3f);

                buttonIndex = Mathf.Max(buttonIndex - 1, 0);

                AdjustTransparency(options[buttonIndex], 1.0f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log(options[buttonIndex] + " Clicked!");
        }
    }

    void AdjustTransparency(Button btn, float alpha)
    {
        TMP_Text optionText = btn.GetComponentInChildren<TMP_Text>();
        optionText.color = new Color(optionText.color.r, optionText.color.g, optionText.color.b, alpha);
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateCounter : MonoBehaviour
{
    private TMP_Text movementCounterText;
    public int movementCounter;

    void Start()
    {
        movementCounterText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement playerMovementScript = GameObject.Find("Player").GetComponent<PlayerMovement>();

        if (playerMovementScript != null )
        {
            movementCounter = playerMovementScript.movementCounter;

            movementCounterText.text = movementCounter.ToString();

            if (movementCounter == 0)
            {
                movementCounterText.text = "X";
            }

            if (movementCounter < 0) 
            {
                movementCounterText.text = "23";
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NewBehaviourScript : MonoBehaviour
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Code to execute when pointer enters the button
        Debug.Log("Pointer entered the button!");
        // You can add more functionality here
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Code to execute when pointer exits the button
        Debug.Log("Pointer exited the button!");
        // You can add more functionality here
    }
}

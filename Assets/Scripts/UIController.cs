using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Text _text = null;

    void Start()
    {
        _text.text = "";
        _text.gameObject.SetActive(false);
    }

    // Enable and display text
    public void ShowText(string textToShow)
    {
        _text.text = textToShow;
        _text.gameObject.SetActive(true);
    }

    // Set text to empty and disable
    public void HideText()
    {
        _text.text = "";
        _text.gameObject.SetActive(false);
    }
}

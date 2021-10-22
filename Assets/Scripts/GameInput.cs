using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInput : MonoBehaviour
{
    UIController _uiController = null;

    void Start()
    {
        _uiController = FindObjectOfType<UIController>();
    }

    void Update()
    {
        // Backspace reloads level
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            if(_uiController != null)
            {
                _uiController.HideText();
            }
            ReloadLevel();
        }

        // Escape quits game
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void ReloadLevel()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(activeSceneIndex);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlashImage : MonoBehaviour
{
    Image _image = null;
    Coroutine _currentFlashRoutine = null;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void StartFlash(float secondsPerFlash, float maxAlpha, Color newColor)
    {
        _image.color = newColor;

        // Ensure max alpha < 1
        maxAlpha = Mathf.Clamp(maxAlpha, 0, 1);

        if(_currentFlashRoutine != null)
        {
            StopCoroutine(_currentFlashRoutine);
        }
        _currentFlashRoutine = StartCoroutine(Flash(secondsPerFlash, maxAlpha));
    }

    private IEnumerator Flash(float secondsPerFlash, float maxAlpha)
    {
        // Animate flash in
        float halfFlashDuration = secondsPerFlash / 2;
        for(float t = 0; t <= halfFlashDuration; t += Time.deltaTime)
        {
            Color colorThisFrame = _image.color;
            colorThisFrame.a = Mathf.Lerp(0, maxAlpha, t / halfFlashDuration);
            _image.color = colorThisFrame;
            yield return null;
        }

        // Animate flash out
        for(float t = 0; t <= halfFlashDuration; t += Time.deltaTime)
        {
            Color colorThisFrame = _image.color;
            colorThisFrame.a = Mathf.Lerp(maxAlpha, 0, t / halfFlashDuration);
            _image.color = colorThisFrame;
            yield return null;
        }

        _image.color = new Color32(0, 0, 0, 0);
    }
}

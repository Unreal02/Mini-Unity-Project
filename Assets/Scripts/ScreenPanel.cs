using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenPanel : MonoBehaviour
{
    private static ScreenPanel instance;
    public static ScreenPanel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScreenPanel>();
            }
            return instance;
        }
    }

    private void Start()
    {
        if (Instance != this) { Destroy(gameObject); }
    }

    public const float fadeTime = 0.3f;
    private int fadeDirection; // 1: fade in, -1: fade out
    Image image;

    public void Fade()
    {
        image = GetComponent<Image>();
        _ = StartCoroutine(nameof(_Fade));
    }

    private IEnumerator _Fade()
    {
        fadeDirection = 1;
        yield return new WaitForSeconds(fadeTime);
        SetAlpha(1);
        fadeDirection = -1;
        yield return new WaitForSeconds(fadeTime);
        SetAlpha(0);
        fadeDirection = 0;
    }

    private void Update()
    {
        if (fadeDirection != 0)
        {
            SetAlpha(image.color.a + fadeDirection * Time.deltaTime / fadeTime);
        }
    }

    private void SetAlpha(float value)
    {
        Color color = image.color;
        color.a = value;
        image.color = color;
    }
}

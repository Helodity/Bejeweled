using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public Image FadeImage;

    public IEnumerator DoFadeOut(float duration) {
        Color endColor = FadeImage.color;
        endColor.a = 0;
        yield return StartCoroutine(FadeToColor(duration, endColor));
    }
    public IEnumerator DoFadeIn(float duration) {
        Color endColor = FadeImage.color;
        endColor.a = 1;
        yield return StartCoroutine(FadeToColor(duration, endColor));
    }

    IEnumerator FadeToColor(float duration, Color endColor) {
        float startDuration = duration;
        Color startColor = FadeImage.color;
        while(duration > 0) {
            duration -= Time.deltaTime;
            FadeImage.color = Color.Lerp(endColor, startColor, duration / startDuration);
            yield return new WaitForEndOfFrame();
        }
    }

}

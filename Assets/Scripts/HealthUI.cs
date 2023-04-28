using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] TMP_Text Text;
    [SerializeField] Image HealthBarFill;

    [Header("Color Animation")]
    [SerializeField] Color IdleColor;
    [SerializeField] Color DamageColor;
    [SerializeField] Color HealColor;
    [SerializeField] AnimationCurve fillColorCurve;

    [Header("Drain Animation")]
    [SerializeField] float AnimationTime;
    [SerializeField] AnimationCurve fillScaleCurve;

    public IHealth ToTrack;

    private void Awake() {
        HealthBarFill.color = IdleColor;
    }

    private void Update() {
        if(ToTrack == null)
            return;
        if(Text != null)
            Text.text = ToTrack.GetCurrentHealth().ToString();
    }

    public IEnumerator PlayHealAnimation() {
        yield return StartCoroutine(PlayFillAnimation(HealColor));
    }
    public IEnumerator PlayDamageAnimation() {
        yield return StartCoroutine(PlayFillAnimation(DamageColor));
    }

    IEnumerator PlayFillAnimation(Color startColor) {
        float durationRemaining = AnimationTime;
        Vector3 startScale = HealthBarFill.rectTransform.localScale;
        Vector3 targetScale = new Vector3(ToTrack.GetHealthRatio(), 1, 1);
        while(durationRemaining > 0) {
            HealthBarFill.color = Color.Lerp(IdleColor, startColor, fillColorCurve.Evaluate(durationRemaining / AnimationTime));
            HealthBarFill.rectTransform.localScale = Vector3.Lerp(targetScale, startScale, fillScaleCurve.Evaluate(durationRemaining / AnimationTime));
            durationRemaining -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}

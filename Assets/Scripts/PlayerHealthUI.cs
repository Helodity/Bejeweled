using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    //[SerializeField] SpriteRenderer BarRenderer;
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

    public PlayerStats ToTrack;

    private void Update() {
        if(ToTrack == null)
            return;

        Text.text = ToTrack.Health.ToString();
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
        Vector3 targetScale = new Vector3((float)ToTrack.Health / ToTrack.MaxHealth, 1, 1);
        while(durationRemaining > 0) {
            HealthBarFill.color = Color.Lerp(IdleColor, startColor, fillColorCurve.Evaluate(durationRemaining / AnimationTime));
            HealthBarFill.rectTransform.localScale = Vector3.Lerp(targetScale, startScale, fillScaleCurve.Evaluate(durationRemaining / AnimationTime));
            durationRemaining -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}

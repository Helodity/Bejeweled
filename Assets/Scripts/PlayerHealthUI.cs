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

    float AnimationTimeRemaining;
    Color AnimationColor = Color.white;
    float TargetRatio = 1;
    Vector3 AnimationStartScale = Vector3.one;

    private void Update() {
        if(ToTrack == null)
            return;

        Text.text = ToTrack.Health.ToString();

        if(AnimationTimeRemaining > 0) {
            AnimationTimeRemaining -= Time.deltaTime;
        } else {
            AnimationTimeRemaining = 0;
        }

        Vector3 targetScale = new Vector3(TargetRatio, 1, 1);
        HealthBarFill.color = Color.Lerp(IdleColor, AnimationColor, fillColorCurve.Evaluate(AnimationTimeRemaining / AnimationTime));
        HealthBarFill.rectTransform.localScale = Vector3.Lerp(targetScale, AnimationStartScale, fillScaleCurve.Evaluate(AnimationTimeRemaining / AnimationTime));
    }

    public void StartHealAnimation() {
        AnimationTimeRemaining = AnimationTime;
        TargetRatio = (float)ToTrack.Health / ToTrack.MaxHealth;
        AnimationStartScale = HealthBarFill.rectTransform.localScale;
        AnimationColor = HealColor;
    }
    public void StartDamageAnimation() {
        AnimationTimeRemaining = AnimationTime;
        TargetRatio = (float)ToTrack.Health / ToTrack.MaxHealth;
        AnimationStartScale = HealthBarFill.rectTransform.localScale;
        AnimationColor = DamageColor;
    }
}

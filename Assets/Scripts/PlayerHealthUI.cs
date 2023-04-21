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
    [SerializeField] AnimationCurve fillColorCurve;

    [Header("Drain Animation")]
    [SerializeField] float AnimationTime;
    [SerializeField] AnimationCurve fillScaleCurve;

    [HideInInspector] public PlayerUnit ToTrack;
    float AnimationTimeRemaining;
    float TargetRatio = 1;
    Vector3 AnimationStartScale = Vector3.one;

    private void Update() {
        if(ToTrack == null)
            return;
        Text.text = ToTrack.Health.ToString();
        float curRatio = (float)ToTrack.Health / ToTrack.MaxHealth;
        if(curRatio != TargetRatio) {
            AnimationTimeRemaining = AnimationTime;
            AnimationStartScale = HealthBarFill.rectTransform.localScale;
            TargetRatio = curRatio;
        }
        if(AnimationTimeRemaining > 0) {
            AnimationTimeRemaining -= Time.deltaTime;
        } else {
            AnimationTimeRemaining = 0;
        }
        Vector3 targetScale = new Vector3(TargetRatio, 1, 1);
        HealthBarFill.color = Color.Lerp(IdleColor, DamageColor, fillColorCurve.Evaluate(AnimationTimeRemaining / AnimationTime));
        HealthBarFill.rectTransform.localScale = Vector3.Lerp(targetScale, AnimationStartScale, fillScaleCurve.Evaluate(AnimationTimeRemaining / AnimationTime));
    }
}

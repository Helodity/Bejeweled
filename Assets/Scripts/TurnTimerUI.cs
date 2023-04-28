using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnTimerUI : MonoBehaviour
{
    EnemyUnit toTrack;

    [SerializeField] Color AttackColor = Color.red;
    [SerializeField] Color IdleColor = Color.white;
    Color TargetColor;

    [SerializeField] TMP_Text TurnText;
    [SerializeField] float AttackNearScaleAmplitude = 10;
    [SerializeField] float AttackNearScaleFrequency = 3;
    float AttackNearPhaseOffset;
    float originalTextScale;
    float TextScaleModifier;



    [SerializeField] float ScaleEffectAmplitude = 3f;
    [SerializeField] float ScaleEffectDuration = 0.2f;

    private void Awake() {
        toTrack = GetComponent<EnemyUnit>();
        TurnText.text = toTrack.TurnTimerRemaining.ToString();
        originalTextScale = TurnText.fontSize;
        OnTextChange();
    }

    // Update is called once per frame
    void Update()
    {
        string prevText = TurnText.text;
        TurnText.text = toTrack.TurnTimerRemaining.ToString();

        if(prevText != TurnText.text) {
            OnTextChange();
        }

        if(toTrack.TurnTimerRemaining <= 1) {
            TextScaleModifier += Mathf.Sin(Time.time * AttackNearScaleFrequency + AttackNearPhaseOffset) * AttackNearScaleAmplitude + AttackNearScaleAmplitude;
        }

        TurnText.color = TargetColor;
    }

    private void LateUpdate() {
        TurnText.fontSize = TextScaleModifier + originalTextScale;
        TextScaleModifier = 0;
    }

    public IEnumerator DoScaleEffect() {
        float duration = ScaleEffectDuration;
        AttackNearPhaseOffset = Time.time - AttackNearScaleFrequency / 2;
        while(duration > 0) {
            duration -= Time.deltaTime;
            TextScaleModifier += Mathf.Lerp(0, ScaleEffectAmplitude, duration / ScaleEffectDuration);
            yield return new WaitForEndOfFrame();
        }
    }
    void OnTextChange() {
        if(toTrack.TurnTimerRemaining <= 1) {
            TargetColor = AttackColor;
        } else {
            TargetColor = IdleColor;
        }
    }
}

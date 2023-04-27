using System;
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
    [SerializeField] float TextScaleAmplitude = 10;
    [SerializeField] float TextScaleFrequency = 3;
    float originalTextScale;

    [SerializeField] float ValueUpdateScaleMultiplier = 1.1f;
    [SerializeField] float ValueUpdateAnimationDuration = 0.2f;
    float ValueUpdateAnimationDurationRemaining;

    private void Awake() {
        toTrack = GetComponent<EnemyUnit>();
        originalTextScale = TurnText.fontSize;
        OnTextChange();
    }

    // Update is called once per frame
    void Update()
    {
        if(ValueUpdateAnimationDurationRemaining > 0) {
            ValueUpdateAnimationDurationRemaining -= Time.deltaTime;
        } else {
            ValueUpdateAnimationDurationRemaining = 0;
        }

        string prevText = TurnText.text;
        TurnText.text = toTrack.TurnTimerRemaining.ToString();

        if(prevText != TurnText.text) {
            OnTextChange();
        }

        UpdateTextSize();
        TurnText.color = TargetColor;

    }

    void UpdateTextSize() {
        float targetSize = originalTextScale;
        if(toTrack.TurnTimerRemaining <= 1) {
            targetSize += Mathf.Sin(Time.time * TextScaleFrequency) * TextScaleAmplitude;
        }
        TurnText.fontSize = Mathf.Lerp(targetSize, originalTextScale * ValueUpdateScaleMultiplier, ValueUpdateAnimationDurationRemaining / ValueUpdateAnimationDuration);
    }
    void OnTextChange() {
        if(toTrack.TurnTimerRemaining <= 1) {
            TargetColor = AttackColor;
        } else {
            TargetColor = IdleColor;
        }
        ValueUpdateAnimationDurationRemaining = ValueUpdateAnimationDuration;
    }
}

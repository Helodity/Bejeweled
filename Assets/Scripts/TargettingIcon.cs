using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargettingIcon : MonoBehaviour
{
    [SerializeField] float TurnSpeed;
    [SerializeField] AnimationCurve MovementCurve;
    SpriteRenderer Renderer;

    private void Awake() {
        Renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.Rotate(0, 0, TurnSpeed * Time.deltaTime);
    }
    public IEnumerator FadeTo(Transform target, float duration) {
        Color endColor = Renderer.color;
        Color startColor = Renderer.color;
        startColor.a = 0;
        Renderer.color = startColor;
        transform.position = target.position;

        float startDur = duration;
        while(duration > 0) {
            duration -= Time.deltaTime;
            Renderer.color = Color.Lerp(endColor, startColor, duration / startDur);
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator MoveTo(Transform target, float duration) {
        Vector3 startPos = transform.position;
        float startDur = duration;
        while(duration > 0) {
            duration -= Time.deltaTime;
            transform.position = Vector3.Lerp(target.position, startPos, MovementCurve.Evaluate(duration / startDur));

            yield return new WaitForEndOfFrame();
        }
        transform.position = target.position;
    }

}

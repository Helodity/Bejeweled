using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargettingIcon : MonoBehaviour
{
    [SerializeField] float TurnSpeed;
    [SerializeField] AnimationCurve MovementCurve;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, TurnSpeed * Time.deltaTime);
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

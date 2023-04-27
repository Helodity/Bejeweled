using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Vector3 CenterPosition;

    private void Awake() {
        CenterPosition = transform.position;
    }

    public IEnumerator PlayShakeEffect(float duration, float scale) {
        while(duration > 0) {
            Vector2 offset = Random.insideUnitCircle * duration * scale;
            transform.position = CenterPosition + (Vector3)offset;

            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}

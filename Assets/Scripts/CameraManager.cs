using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] float ShakeTimeRemaining = 0;
    [SerializeField] float ShakeScale = 1;
    Vector3 CenterPosition;


    private void Awake() {
        CenterPosition = transform.position;
    }

    void Update()
    {
        ShakeTimeRemaining -= Time.deltaTime;
        if(ShakeTimeRemaining <= 0) {
            ShakeTimeRemaining = 0;
        }
        Vector2 offset = Random.insideUnitCircle * ShakeTimeRemaining * ShakeScale;
        transform.position = CenterPosition + (Vector3)offset;
    }


    public void StartShakeEffect(float duration, float scale) {
        ShakeTimeRemaining = duration;
        ShakeScale = scale;
    }
}

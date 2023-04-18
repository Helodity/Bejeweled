using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDestructionEffect : MonoBehaviour
{
    ParticleSystem system;
    // Start is called before the first frame update
    float LifeTime;
    void Start()
    {
        system = GetComponent<ParticleSystem>();
        LifeTime = system.startLifetime;
    }
    private void Update() {
        LifeTime -= Time.deltaTime;
        if(LifeTime < 0)
            Destroy(gameObject);
    }
}

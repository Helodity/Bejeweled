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
    }
    private void Update() {

        if(!system.IsAlive()) {
            Destroy(gameObject);
        }
    }
}

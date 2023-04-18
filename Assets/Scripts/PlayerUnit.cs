using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour {

    public int Health;
    public int MaxHealth;
    // List of Abilities


    public void Initialize() {
        MaxHealth = 100;
        Health = 100;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : IHealth {

    int Health;
    int MaxHealth;
    public int Currency;

    public HealthUI HealthUI { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    // List of Abilities


    public void Initialize() {
        MaxHealth = 100;
        Health = 100;
    }

    #region IHealth Implmentation
    public IEnumerator ReceiveDamage(int damage) {
        Health -= damage;
        if(Health <= 0) {
            Health = 0;
            //Die
        }
        yield return null;
    }
    public IEnumerator ReceiveHealing(int healing) {
        Health += healing;
        if(Health > MaxHealth) {
            Health = MaxHealth;
        }
        yield return null;
    }
    public int GetCurrentHealth() {
        return Health;
    }

    public float GetHealthRatio() {
        return (float)Health / MaxHealth;
    }

    public int GetMaxHealth() {
        return MaxHealth;
    }
    #endregion
}
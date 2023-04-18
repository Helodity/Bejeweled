using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour {

    SpriteRenderer Renderer;

    EnemyType Data;
    public int TurnTimerRemaining;
    public int Health;


    private void Awake() {
        Renderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(EnemyType data) {
        Data = data;
        Health = data.MaxHealth;
        TurnTimerRemaining = data.TurnTimer;
    }

    public IEnumerator DoTurn() {
        TurnTimerRemaining--;

        if(TurnTimerRemaining <= 0) {
            TurnTimerRemaining = Data.TurnTimer;
            Debug.Log("enemy attack!!!");
            GameManager.Instance.Player.Health -= Data.Damage;
        }

        

        yield return null;
    }
}

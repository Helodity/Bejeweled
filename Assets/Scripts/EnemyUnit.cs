using System.Collections;
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
        Renderer.sprite = Data.Sprite;
    }

    public IEnumerator DoTurn() {
        TurnTimerRemaining--;

        if(TurnTimerRemaining <= 0) {
            TurnTimerRemaining = Data.TurnTimer;
            GameManager.Instance.DamagePlayer(Data.Damage);
            Debug.Log("enemy attack!!!");
        }
        yield return null;
    }
}

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
        TurnTimerRemaining = data.TurnTimer + Random.Range(-1,2);
        Renderer.sprite = Data.Sprite;
    }

    public IEnumerator DoTurn() {
        TurnTimerRemaining--;

        if(TurnTimerRemaining <= 0) {
            yield return StartCoroutine(GameManager.Instance.DamagePlayer(Data.Damage));
            TurnTimerRemaining = Data.TurnTimer + Random.Range(0, 2);
            Debug.Log("enemy attack!!!");
        }
        yield return null;
    }
}

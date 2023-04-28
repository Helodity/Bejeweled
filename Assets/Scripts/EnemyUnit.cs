using System.Collections;
using UnityEngine;

public class EnemyUnit : MonoBehaviour {

    SpriteRenderer Renderer;

    EnemyType Data;
    public TurnTimerUI turnTimerUI;
    public int TurnTimerRemaining;
    public int Health;


    private void Awake() {
        Renderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown() {
        StartCoroutine(GameManager.Instance.SelectEnemy(this));
    }

    public IEnumerator Initialize(EnemyType data) {
        Data = data;
        Health = data.MaxHealth;
        TurnTimerRemaining = data.TurnTimer + Random.Range(-1,2);
        Renderer.sprite = Data.Sprite;
        yield return null;
    }

    public IEnumerator DoTurn() {
        TurnTimerRemaining--;
        yield return StartCoroutine(turnTimerUI.DoScaleEffect());
        if(TurnTimerRemaining <= 0) {
            yield return StartCoroutine(GameManager.Instance.DamagePlayer(Data.Damage));
            TurnTimerRemaining = Data.TurnTimer + Random.Range(0, 2);
            Debug.Log("enemy attack!!!");
        }
        yield return null;
    }
}

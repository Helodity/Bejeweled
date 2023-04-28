using System.Collections;
using UnityEngine;

public class EnemyUnit : MonoBehaviour, IHealth {

    SpriteRenderer Renderer;

    EnemyType Data;
    public TurnTimerUI turnTimerUI;
    public HealthUI HealthUI;
    public int TurnTimerRemaining;
    public int Health;

    private void Awake() {
        Renderer = GetComponent<SpriteRenderer>();
        HealthUI.ToTrack = this;
    }

    private void OnMouseDown() {
        StartCoroutine(GameManager.Instance.OnEnemyClick(this));
    }

    public IEnumerator Initialize(EnemyType data) {
        Data = data;
        Health = data.MaxHealth;
        TurnTimerRemaining = data.TurnTimer + Random.Range(-1,2);
        Renderer.sprite = Data.Sprite;
        yield return null;
    }

    public IEnumerator DoTurn() {
        if(Health <= 0)
            yield break;
        TurnTimerRemaining--;
        yield return StartCoroutine(turnTimerUI.DoScaleEffect());
        if(TurnTimerRemaining <= 0) {
            yield return StartCoroutine(GameManager.Instance.DamagePlayer(Data.Damage));
            TurnTimerRemaining = Data.TurnTimer + Random.Range(0, 2);
            Debug.Log("enemy attack!!!");
        }
        yield return null;
    }

    IEnumerator OnDie() {
        yield return StartCoroutine(GameManager.Instance.Kill(this));
        Destroy(gameObject);
    }

    #region IHealth Implementation
    public float GetHealthRatio() {
        return (float)Health / Data.MaxHealth;
    }

    public int GetCurrentHealth() {
        return Health;
    }

    public int GetMaxHealth() {
        return Data.MaxHealth;
    }

    public IEnumerator ReceiveDamage(int damage) {
        Health -= damage;
        if(Health < 0) {
            Health = 0;
        }
        yield return StartCoroutine(HealthUI.PlayDamageAnimation());
        if(Health == 0) {
            yield return StartCoroutine(OnDie());
        }
    }

    public IEnumerator ReceiveHealing(int healing) {
        Health += healing;
        if(Health > Data.MaxHealth) {
            Health = Data.MaxHealth;
        }
        yield return StartCoroutine(HealthUI.PlayHealAnimation());
    }
    #endregion
}
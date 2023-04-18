using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    //[SerializeField] SpriteRenderer BarRenderer;
    [SerializeField] TMP_Text Text;
    [SerializeField] Image HealthBarFill;

    private void Update() {
        Text.text = GameManager.Instance.Player.Health.ToString();
        HealthBarFill.rectTransform.localScale = new Vector3((float)GameManager.Instance.Player.Health / GameManager.Instance.Player.MaxHealth,1,1);
    }
}

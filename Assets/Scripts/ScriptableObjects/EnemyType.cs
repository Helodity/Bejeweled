using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemyType", order = 1)]
public class EnemyType : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public int MaxHealth;
    public int TurnTimer;
    public int Damage;
}

using System.Collections;

public interface IHealth
{
    IEnumerator ReceiveDamage(int damage);
    IEnumerator ReceiveHealing(int healing);

    float GetHealthRatio();
    int GetCurrentHealth();
    int GetMaxHealth();
}

using System.Collections;
using UnityEngine;

public class TileContents : MonoBehaviour
{

    public enum ContentType {
        Grass,
        Water,
        Electricity,
        Healing,
        Fire,
        Money,
        SIZE
    }

    [SerializeField] SpriteRenderer Renderer;
    [SerializeField] Sprite[] sprites = null;
    public ContentType contents = ContentType.Grass;

    [SerializeField] AnimationCurve SwapCurve;
    [SerializeField] TileDestructionEffect destructionEffect;

    public void SetContents(ContentType content) {
        if(content == ContentType.SIZE) {
            Debug.LogError("Tried to set content type to SIZE");
            return;
        }
        contents = content;
        Renderer.sprite = sprites[(int)content];
    }

    public IEnumerator DoSwapAnimation(Vector2 target, float duration) {
        float totalDur = duration;
        Vector3 startPos = transform.position;
        Vector3 endPos = target;
        while(duration > 0) {
            duration -= Time.deltaTime;
            transform.position = Vector3.Lerp(endPos, startPos, SwapCurve.Evaluate(duration / totalDur));
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPos;
    }

    public void Clear() {
        Instantiate(destructionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

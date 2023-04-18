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


    float AnimationTime = 0.2f;
    [SerializeField] AnimationCurve animationCurve;
    [SerializeField] TileDestructionEffect destructionEffect;
    Vector2 AnimationStartPos;
    Vector2 AnimationEndPos;
    float AnimationTimeRemaining;

    private void Awake() {
        AnimationEndPos = transform.position;
    }

    public void SetContents(ContentType content) {
        if(content == ContentType.SIZE) {
            Debug.LogError("Tried to set content type to SIZE");
            return;
        }
        if(sprites.Length != (int)ContentType.SIZE) {
            Debug.LogError("Inconsistencies between sprite array and content types!");
            return;
        }
        contents = content;
        Renderer.sprite = sprites[(int)content];
    }
    public void StartSwapAnimation(Vector2 target, float duration) {
        AnimationStartPos = transform.position;
        AnimationTime = duration;
        AnimationTimeRemaining = AnimationTime;
        AnimationEndPos = target;
    }

    // Update is called once per frame
    void Update()
    {
        if(AnimationTimeRemaining > 0) {
            AnimationTimeRemaining -= Time.deltaTime;
            if(AnimationTimeRemaining < 0)
                AnimationTimeRemaining = 0;
        }
        transform.position = Vector3.Lerp(AnimationEndPos, AnimationStartPos, animationCurve.Evaluate(AnimationTimeRemaining / AnimationTime)); 
    }
    public void Clear() {
        Instantiate(destructionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

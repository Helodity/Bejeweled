using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] Color DeselectedColor = new Color(.5f, .5f, .5f, 0.3f);
    [SerializeField] Color HighlightedColor = new Color(.7f, .7f, .7f, 0.3f);
    [SerializeField] Color SelectedColor = new Color(1, 1, 1, 0.5f);
    [SerializeField] SpriteRenderer SelectionRenderer;

    public Vector2Int Position;
    public TileContents Contents;
    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.SelectedTile == this) {
            SelectionRenderer.color = SelectedColor;
        } else if(GameManager.Instance.HighlightedTile == this) {
            SelectionRenderer.color = HighlightedColor;
        } else {
            SelectionRenderer.color = DeselectedColor;
        }
    }

    private void OnMouseEnter() {
        GameManager.Instance.HighlightTile(this);
    }
    private void OnMouseExit() {
        GameManager.Instance.HighlightTile(null);
    }
    private void OnMouseDown() {
        StartCoroutine(GameManager.Instance.SelectTile(this));
    }
}

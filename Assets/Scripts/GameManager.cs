using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Prefabs")]
    [SerializeField] PlayerUnit PlayerPrefab;
    [SerializeField] EnemyUnit EnemyPrefab;
    [SerializeField] Tile TilePrefab;
    [SerializeField] TileContents TileContentsPrefab;
    [SerializeField] List<EnemyType> EnemyTypes;

    [Header("Map")]
    public Vector2Int MapSize = new Vector2Int(5,5);

    [Header("Visuals")]
    [SerializeField] float TileSwapDuration = 0.2f;
    [SerializeField] float TileFallDuration = 0.2f;
    [SerializeField] float HeightPerSpawnedTile = 1.5f;

    [SerializeField] Transform PlayerPosition;
    [SerializeField] List<Transform> EnemyPositions;

    public Tile[,] Map;
    public Tile SelectedTile { get; private set; }
    public Tile HighlightedTile { get; private set; }

    [Header("Units")]
    public PlayerUnit Player;
    public List<EnemyUnit> Enemies;

    bool ReceivingInput = false;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
        CreateMap();
        CreateUnits();
        ReceivingInput = true;
    }
    void CreateMap() {
        Map = new Tile[MapSize.x, MapSize.y];
        for(int x = 0; x < MapSize.x; x++) {
            for(int y = 0; y < MapSize.y; y++) {
                Vector3 position = new Vector3(x - ((float)MapSize.x / 2) + 0.5f, y - ((float)MapSize.y / 2) + 0.5f, 0);
                Tile t = Instantiate(TilePrefab, position, Quaternion.identity, transform);
                t.Position = new Vector2Int(x, y);
                Map[x, y] = t;
                TileContents tc = Instantiate(TileContentsPrefab, position, Quaternion.identity, t.transform);
                tc.SetContents((TileContents.ContentType)Random.Range(0, (int)TileContents.ContentType.SIZE));
                t.Contents = tc;
            }
        }
        List<Tile> toRefresh = GetClearedTiles();
        while(toRefresh.Count > 0) {
            foreach(Tile t in toRefresh) {
                t.Contents.SetContents((TileContents.ContentType)Random.Range(0, (int)TileContents.ContentType.SIZE));
            }
            toRefresh = GetClearedTiles();
        }
    }

    void CreateUnits() {
        Player = Instantiate(PlayerPrefab, PlayerPosition.position, Quaternion.identity);
        Player.Initialize();

        EnemyUnit e = Instantiate(EnemyPrefab, EnemyPositions[0].position, Quaternion.identity);
        e.Initialize(EnemyTypes.First());
        Enemies.Add(e);
    }

    bool IsAdjacent(Tile t1, Tile t2) {
        return t1.Position == new Vector2Int(t2.Position.x, t2.Position.y + 1) ||
            t1.Position == new Vector2Int(t2.Position.x, t2.Position.y - 1) ||
            t1.Position == new Vector2Int(t2.Position.x + 1, t2.Position.y) ||
            t1.Position == new Vector2Int(t2.Position.x - 1, t2.Position.y);
    }

    List<Tile> GetClearedTiles() {
        List<Tile> clearedTiles = new List<Tile>();
        List<Match> matches = new List<Match>();
        //Get all row matches
        for(int x = 0; x < MapSize.x; x++) {
            TileContents.ContentType prevContent = TileContents.ContentType.SIZE;
            List<Tile> curChain = new List<Tile>();
            List<Tile> toAdd = new List<Tile>();
            bool hasMatchToSave = false;
            for(int y = 0; y < MapSize.y; y++) {
                if(prevContent == Map[x, y].Contents.contents) {
                    curChain.Add(Map[x, y]);
                    if(curChain.Count >= 3) {
                        hasMatchToSave = true;
                    }
                } else {
                    if(hasMatchToSave) {
                        matches.Add(new Match(prevContent, curChain.Count));
                        toAdd.AddRange(curChain);
                    }
                    curChain = new List<Tile>() { Map[x, y] };
                    prevContent = Map[x, y].Contents.contents;
                    hasMatchToSave = false;
                }
                clearedTiles.AddRange(toAdd);
            }
            if(hasMatchToSave) {
                matches.Add(new Match(prevContent, curChain.Count));
                toAdd.AddRange(curChain);
            }
        }
        //Get all column matches
        for(int y = 0; y < MapSize.y; y++) {
            TileContents.ContentType prevContent = TileContents.ContentType.SIZE;
            List<Tile> curChain = new List<Tile>();
            List<Tile> toAdd = new List<Tile>();
            bool hasMatchToSave = false;
            for(int x = 0; x < MapSize.x; x++) {
                if(prevContent == Map[x, y].Contents.contents) {
                    curChain.Add(Map[x, y]);
                    if(curChain.Count >= 3) {
                        hasMatchToSave = true;
                    }
                } else {
                    if(hasMatchToSave) {
                        matches.Add(new Match(prevContent, curChain.Count));
                        toAdd.AddRange(curChain);
                    }
                    curChain = new List<Tile>() { Map[x, y] };
                    prevContent = Map[x, y].Contents.contents;
                    hasMatchToSave = false;
                }
                clearedTiles.AddRange(toAdd);
            }
            if(hasMatchToSave) {
                matches.Add(new Match(prevContent, curChain.Count));
                toAdd.AddRange(curChain);
            }
        }
        return clearedTiles.Distinct().ToList();
    }

    IEnumerator HandleLines() {
        List<Tile> clearedTiles = GetClearedTiles();
        int combo = 0;
        while(clearedTiles.Count > 0) {
            combo++;
            //Destroy each tile
            foreach(Tile t in clearedTiles) {
                t.Contents.Clear();
            }

            for(int x = MapSize.x - 1; x >= 0; x--) {
                int spawned = 1;
                for(int y = MapSize.y - 1; y >= 0; y--) {
                    if(!clearedTiles.Contains(Map[x, y]))
                        continue;
                    for(int i = y; i < MapSize.y - 1; i++) {
                        Tile t1 = Map[x, i];
                        Tile t2 = Map[x, i + 1];
                        //Move contents to the new tile
                        t1.Contents = t2.Contents;
                        t2.Contents = null;
                        t1.Contents.transform.SetParent(t1.transform);
                        t1.Contents.StartSwapAnimation(t1.transform.position, TileFallDuration);
                    }
                    Tile top = Map[x, MapSize.y - 1];
                    Vector3 position = new Vector3(x - ((float)MapSize.x / 2) + 0.5f, MapSize.y + spawned * HeightPerSpawnedTile - ((float)MapSize.y / 2) + 0.5f, 0);

                    TileContents tc = Instantiate(TileContentsPrefab, position, Quaternion.identity, top.transform);
                    tc.SetContents((TileContents.ContentType)Random.Range(0, (int)TileContents.ContentType.SIZE));
                    tc.StartSwapAnimation(top.transform.position, TileFallDuration);
                    top.Contents = tc;
                    spawned++;
                }
            }
            yield return new WaitForSeconds(TileFallDuration + 0.2f);

            clearedTiles = GetClearedTiles();
        }
        Debug.Log("comno");

    }

    public IEnumerator SelectTile(Tile t) {
        if(!ReceivingInput)
            yield break;
        if(t == null || t == SelectedTile) {
            SelectedTile = null;
            yield break;
        }
        if(!SelectedTile || !IsAdjacent(SelectedTile, t)) {
            SelectedTile = t;
            yield break;
        }

        yield return StartCoroutine(DoPlayerTurn(SelectedTile, t));
    }
    IEnumerator DoPlayerTurn(Tile t1, Tile t2) {
        ReceivingInput = false;
        yield return StartCoroutine(SwapTiles(t1, t2));
        SelectedTile = null;
        yield return StartCoroutine(HandleLines());
        yield return StartCoroutine(DoEnemyTurn());
        ReceivingInput = true;
    }

    IEnumerator DoEnemyTurn() {
        foreach(EnemyUnit e in Enemies) {
            yield return e.StartCoroutine(e.DoTurn());
        }
    }

    IEnumerator SwapTiles(Tile t1, Tile t2) {
        if(t1 == null || t2 == null) { yield break; }
        //Swap Contents
        TileContents temp = t1.Contents;
        t1.Contents = t2.Contents;
        t2.Contents = temp;

        //Move contents to the new tile
        t1.Contents.transform.SetParent(t1.transform);
        t1.Contents.StartSwapAnimation(t1.transform.position, TileSwapDuration);

        t2.Contents.transform.SetParent(t2.transform);
        t2.Contents.StartSwapAnimation(t2.transform.position, TileSwapDuration);
        yield return new WaitForSeconds(TileSwapDuration + 0.1f);
    }
    public void HighlightTile(Tile t) {
        HighlightedTile = t;
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(MapSize.x, MapSize.y, 1));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(PlayerPosition.position, 0.2f);
        Gizmos.color = Color.red;
        foreach(Transform t in EnemyPositions) {
            Gizmos.DrawWireSphere(t.position, 0.2f);
        }
    }

}
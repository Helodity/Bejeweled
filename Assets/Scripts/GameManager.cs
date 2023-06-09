using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Prefabs")]
    [SerializeField] PlayerStats PlayerPrefab;
    [SerializeField] EnemyUnit EnemyPrefab;
    [SerializeField] Tile TilePrefab;
    [SerializeField] TileContents TileContentsPrefab;

    [Header("Map")]
    [SerializeField] Transform MapParent;
    public Vector2Int MapSize = new Vector2Int(5,5);

    [Header("Visuals")]
    [SerializeField] float TileSwapDuration = 0.2f;
    [SerializeField] float TileFallDuration = 0.2f;
    [SerializeField] float MatchClearDelay = 0.1f;
    [SerializeField] float DamageShakeDuration = 0.2f;
    [SerializeField] float HeightPerSpawnedTile = 1.5f;
    [SerializeField] float TargetSwapDuration = 0.1f;
    [SerializeField] HealthUI HealthUI;
    [SerializeField] CameraManager CameraController;
    [SerializeField] FadeManager FadeController;
    [SerializeField] TargettingIcon TargetIcon;

    [SerializeField] Transform EnemyCenter;
    [SerializeField] float EnemySpawnScale;

    public Tile[,] Map;
    public Tile SelectedTile { get; private set; }
    public Tile HighlightedTile { get; private set; }

    [Header("Units")]
    [SerializeField] List<EnemyWave> WaveList;
    [SerializeField] List<EnemyUnit> Enemies;
    EnemyUnit AttackTarget;

    bool ReceivingInput = false;

    public PlayerStats Player;

    private void Awake() {
        Application.targetFrameRate = 60;
        if(Instance == null) {
            Instance = this;
        }
        CreateMap();
        Player = new PlayerStats();
        Player.Initialize();
        HealthUI.ToTrack = Player;
    }


    private IEnumerator Start() {
        yield return StartCoroutine(FadeController.DoFadeOut(1f));
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(SpawnWave(WaveList[0]));
        AttackTarget = Enemies[0];
        StartCoroutine(TargetIcon.FadeTo(AttackTarget.transform, 0.2f));
        ReceivingInput = true;
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(MapSize.x, MapSize.y, 1));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(EnemyCenter.position, new Vector3(EnemySpawnScale, 1, 1));
    }

    void CreateMap() {
        Map = new Tile[MapSize.x, MapSize.y];
        for(int x = 0; x < MapSize.x; x++) {
            for(int y = 0; y < MapSize.y; y++) {
                Vector3 position = new Vector3(x - ((float)MapSize.x / 2) + 0.5f, y - ((float)MapSize.y / 2) + 0.5f, 0);
                Tile t = Instantiate(TilePrefab, position, Quaternion.identity, MapParent);
                t.Position = new Vector2Int(x, y);
                Map[x, y] = t;
                TileContents tc = Instantiate(TileContentsPrefab, position, Quaternion.identity, t.transform);
                tc.SetContents((TileContents.ContentType)UnityEngine.Random.Range(0, (int)TileContents.ContentType.SIZE));
                t.Contents = tc;
            }
        }
        List<Match> matches = GetMatches();
        List<Tile> clearedTiles = new List<Tile>();
        foreach(Match match in matches) {
            foreach(Tile t in match.Tiles) {
                if(!clearedTiles.Contains(t)) {
                    clearedTiles.Add(t);
                }
            }
        }
        while(clearedTiles.Count > 0) {
            foreach(Tile t in clearedTiles) {
                t.Contents.SetContents((TileContents.ContentType)UnityEngine.Random.Range(0, (int)TileContents.ContentType.SIZE));
            }
            matches = GetMatches();
            clearedTiles = new List<Tile>();
            foreach(Match match in matches) {
                foreach(Tile t in match.Tiles) {
                    if(!clearedTiles.Contains(t)) {
                        clearedTiles.Add(t);
                    }
                }
            }
        }
    }

    IEnumerator SpawnWave(EnemyWave wave) {
        int toSpawn = Math.Min(wave.Enemies.Count, 5);

        for(int i = 0; i < toSpawn; i++) {
            float xOffset = i - (float)toSpawn / 2 + 0.5f;
            xOffset *= (EnemySpawnScale / toSpawn);

            Vector3 position = new Vector3(EnemyCenter.position.x + xOffset, EnemyCenter.position.y, EnemyCenter.position.z);
            EnemyUnit e = Instantiate(EnemyPrefab, position, Quaternion.identity,EnemyCenter);
            yield return StartCoroutine(e.Initialize(wave.Enemies[i]));
            Enemies.Add(e);
            yield return new WaitForSeconds(0.1f);
        }
    }

    bool IsAdjacent(Tile t1, Tile t2) {
        return t1.Position == new Vector2Int(t2.Position.x, t2.Position.y + 1) ||
            t1.Position == new Vector2Int(t2.Position.x, t2.Position.y - 1) ||
            t1.Position == new Vector2Int(t2.Position.x + 1, t2.Position.y) ||
            t1.Position == new Vector2Int(t2.Position.x - 1, t2.Position.y);
    }

    List<Match> GetMatches() {
        List<Match> matches = new List<Match>();
        //Get all row matches
        for(int x = 0; x < MapSize.x; x++) {
            TileContents.ContentType prevContent = TileContents.ContentType.SIZE;
            List<Tile> curChain = new List<Tile>();
            bool hasMatchToSave = false;
            for(int y = MapSize.y - 1; y >= 0; y--) {
                if(prevContent == Map[x, y].Contents.contents) {
                    curChain.Add(Map[x, y]);
                    if(curChain.Count >= 3) {
                        hasMatchToSave = true;
                    }
                } else {
                    if(hasMatchToSave) {
                        matches.Add(new Match(prevContent, curChain));
                        hasMatchToSave = false;
                    }
                    curChain = new List<Tile>() { Map[x, y] };
                    prevContent = Map[x, y].Contents.contents;
                }
            }
            if(hasMatchToSave) {
                matches.Add(new Match(prevContent, curChain));
            }
        }
        //Get all column matches
        for(int y = MapSize.y - 1; y >= 0; y--) {
            TileContents.ContentType prevContent = TileContents.ContentType.SIZE;
            List<Tile> curChain = new List<Tile>();
            bool hasMatchToSave = false;
            for(int x = 0; x < MapSize.x; x++) {
                if(prevContent == Map[x, y].Contents.contents) {
                    curChain.Add(Map[x, y]);
                    if(curChain.Count >= 3) {
                        hasMatchToSave = true;
                    }
                } else {
                    if(hasMatchToSave) {
                        matches.Add(new Match(prevContent, curChain));
                        hasMatchToSave = false;
                    }
                    curChain = new List<Tile>() { Map[x, y] };
                    prevContent = Map[x, y].Contents.contents;
                }
            }
            if(hasMatchToSave) {
                matches.Add(new Match(prevContent, curChain));
            }
        }
        List<Tuple<Match, Match>> toCombine = new List<Tuple<Match, Match>>();
        //Combine matches that share elements and types into corner matches
        for(int m1 = 0; m1 < matches.Count(); m1++) {
            for(int m2 = m1; m2 < matches.Count(); m2++) {
                if(m1 == m2)
                    continue;

                foreach(Tile t in matches[m2].Tiles) {
                    if(matches[m1].Tiles.Contains(t)){
                        toCombine.Add(new Tuple<Match,Match>(matches[m1], matches[m2]));
                    }
                }
            }
        }
        foreach(Tuple<Match, Match> m in toCombine) {
            matches.Remove(m.Item1);
            matches.Remove(m.Item2);
            matches.Add(new Match(m.Item1.Content, new List<Tile>(m.Item1.Tiles.Concat(m.Item2.Tiles)).Distinct().ToList()));
        }

        return matches;
    }

    IEnumerator HandleLines() {
        List<Match> totalMatches = new List<Match>();

        while(true) {
            List<Match> matches = GetMatches();
            if(matches.Count == 0) {
                break;
            }
            //Destroy all the tiles, one match at a time
            totalMatches.AddRange(matches);
            List<Tile> clearedTiles = new List<Tile>();
            foreach(Match match in matches) {
                foreach(Tile t in match.Tiles) {
                    if(!clearedTiles.Contains(t)) {
                        clearedTiles.Add(t);
                        t.Contents.Clear();
                    }
                }
                yield return new WaitForSeconds(MatchClearDelay);
            }
            //Spawn the new contents and animate them falling
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
                        StartCoroutine(t1.Contents.DoSwapAnimation(t1.transform.position, TileFallDuration));
                    }
                    Tile top = Map[x, MapSize.y - 1];
                    Vector3 position = new Vector3(x - ((float)MapSize.x / 2) + 0.5f, MapSize.y + spawned * HeightPerSpawnedTile - ((float)MapSize.y / 2) + 0.5f, 0);

                    TileContents tc = Instantiate(TileContentsPrefab, position, Quaternion.identity, top.transform);
                    tc.SetContents((TileContents.ContentType)UnityEngine.Random.Range(0, (int)TileContents.ContentType.SIZE));
                    StartCoroutine(tc.DoSwapAnimation(top.transform.position, TileFallDuration));
                    top.Contents = tc;
                    spawned++;
                }
            }
            yield return new WaitForSeconds(TileFallDuration + 0.2f);
        }

        yield return StartCoroutine(DoMatchEffects(totalMatches));
    }
    public IEnumerator OnEnemyClick(EnemyUnit u) {
        if(!ReceivingInput)
            yield break;
        ReceivingInput = false;
        yield return StartCoroutine(SelectEnemy(u));
        ReceivingInput = true;
    }
    IEnumerator SelectEnemy(EnemyUnit u) {
        AttackTarget = u;
        yield return StartCoroutine(TargetIcon.MoveTo(u.transform, TargetSwapDuration));
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
        yield return new WaitForSeconds(0.1f);
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
        Coroutine a = StartCoroutine(t1.Contents.DoSwapAnimation(t1.transform.position, TileSwapDuration));

        t2.Contents.transform.SetParent(t2.transform);
        Coroutine b = StartCoroutine(t2.Contents.DoSwapAnimation(t2.transform.position, TileSwapDuration));

        yield return a;
        yield return b;
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator DoMatchEffects(List<Match> matches) {

        int healing = 0;
        int currency = 0;
        int damage = 0;
        //Pile all the damage types together
        foreach(Match m in matches) {
            Debug.Log($"{m.Content} {m.Tiles.Count()} {m.IsCornerMatch}");
            int baseValue = Mathf.Max(1, (m.Tiles.Count() - 1) / 2); 

            switch(m.Content) {
                case TileContents.ContentType.Grass:
                    damage += baseValue * m.Tiles.Count();
                    break;
                case TileContents.ContentType.Water:
                    damage += baseValue * m.Tiles.Count();
                    break;
                case TileContents.ContentType.Electricity:
                    damage += baseValue * m.Tiles.Count();
                    break;
                case TileContents.ContentType.Healing:
                    healing += baseValue * m.Tiles.Count() * matches.Count();
                    break;
                case TileContents.ContentType.Fire:
                    damage += baseValue * m.Tiles.Count();
                    break;
                case TileContents.ContentType.Money:
                    currency += m.Tiles.Count();
                    break;
            }
        }

        List<Coroutine> toWaitfor = new List<Coroutine>();
        if(healing > 0) {
            toWaitfor.Add(StartCoroutine(HealPlayer(healing)));
        }
        if(currency > 0) {
            Player.Currency += currency;
        }
        if(damage > 0) {
            toWaitfor.Add(StartCoroutine(AttackTarget.ReceiveDamage(damage)));
        }
        foreach(Coroutine c in toWaitfor) {
            yield return c;
        }
        yield return null;
    }


    public void HighlightTile(Tile t) {
        HighlightedTile = t;
    }
    public IEnumerator DamagePlayer(int amount) {
        StartCoroutine(Player.ReceiveDamage(amount));
        Coroutine uiAnim = StartCoroutine(HealthUI.PlayDamageAnimation());
        Coroutine shakeAnim = StartCoroutine(CameraController.PlayShakeEffect(DamageShakeDuration, amount * 0.1f));

        yield return uiAnim;
        yield return shakeAnim;
    }
    public IEnumerator HealPlayer(int amount) {
        StartCoroutine(Player.ReceiveHealing(amount));
        Coroutine uiAnim = StartCoroutine(HealthUI.PlayHealAnimation());

        yield return uiAnim;
    }
    public IEnumerator Kill(EnemyUnit unit) {
        Enemies.Remove(unit);
        if(Enemies.Count == 0) {
            Debug.Log("So like, wave is done!");
            yield break;
        }
        if(AttackTarget == unit) {
            yield return StartCoroutine(SelectEnemy(Enemies[0]));
        }
        yield return null;
    }
}
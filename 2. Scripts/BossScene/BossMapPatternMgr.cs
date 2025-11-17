using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossMapPatternMgr : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject chainPrefab;
    [SerializeField] private GameObject darkFallingPrefab;
    [SerializeField] private GameObject eyesOfDoomPrefab;
    [SerializeField] private GameObject[] monsterPrefabs;

    [Header("Pattern Cooldowns")]
    [SerializeField] private float randomPatternCooldown = 5f;
    [SerializeField] private float chainSpawnCooldown = 7.5f;
    [SerializeField] private float darkFallingCooldown = 15f;
    [SerializeField] private float eyesOfDoomCooldown = 30f;

    [Header("Chain Settings")]
    [SerializeField] private float chainSpawnDelay = 0.75f;
    [SerializeField] private int maxChainCount = 5;

    [Header("Dark Falling Settings")]
    [SerializeField] private float spawnInterval = 0.35f;
    [SerializeField] private float startX = 29f;
    [SerializeField] private float endX = -29f;
    [SerializeField] private float xInterval = 3f;
    [SerializeField] private float spawnY = 18f;

    [Header("Monster Settings")]
    [SerializeField] private float monsterSpawnY = -3.5f;
    [SerializeField] private float monsterSpawnRangeX = 15f;

    private Player player;
    private MoroKhan boss;
    private readonly List<GameObject> spawnedMonsters = new();

    private float patternTimer;
    private float chainTimer;
    private float darkFallingTimer;
    private float eyesOfDoomTimer;
    private bool hasSpawned;

    private void Start()
    {
        boss = FindAnyObjectByType<MoroKhan>();
        player = FindAnyObjectByType<Player>();

        ResetTimers();
    }

    private void Update()
    {
        if (PlayerMove.inst.IsInteractionState || boss == null) return;

        UpdateTimers();

        if (patternTimer <= 0f)
        {
            TryExecuteRandomPattern();
            patternTimer = randomPatternCooldown;
        }

        if (boss.curHP == boss.maxHP / 2 && !hasSpawned)
            SummonMonsters();

        if (boss.isDie)
            CleanupOnBossDeath();
    }

    #region Timer Management
    private void ResetTimers()
    {
        patternTimer = randomPatternCooldown;
        chainTimer = 0f;
        darkFallingTimer = 0f;
        eyesOfDoomTimer = 0f;
    }

    private void UpdateTimers()
    {
        patternTimer -= Time.deltaTime;
        chainTimer -= Time.deltaTime;
        darkFallingTimer -= Time.deltaTime;
        eyesOfDoomTimer -= Time.deltaTime;
    }
    #endregion

    #region Pattern Execution
    private void TryExecuteRandomPattern()
    {
        if (!BossMapMgr.Inst.CutSceneOver || GameMgr.inst.isPlayerDie || boss.isDie)
            return;

        // 쿨타임이 끝난 패턴만 후보에 넣기
        List<Action> availablePatterns = new();

        if (chainTimer <= 0) availablePatterns.Add(() =>
        {
            StartCoroutine(SpawnChains());
            chainTimer = chainSpawnCooldown;
        });

        if (darkFallingTimer <= 0) availablePatterns.Add(() =>
        {
            StartCoroutine(SpawnDarkFallingObjects());
            darkFallingTimer = darkFallingCooldown;
        });

        if (eyesOfDoomTimer <= 0) availablePatterns.Add(() =>
        {
            SpawnEyesOfDoom();
            eyesOfDoomTimer = eyesOfDoomCooldown;
        });

        // 실행 가능한 패턴이 있으면 랜덤 실행
        if (availablePatterns.Count > 0)
        {
            int index = Random.Range(0, availablePatterns.Count);
            availablePatterns[index].Invoke();
        }
    }
    #endregion

    #region Chain Pattern
    private IEnumerator SpawnChains()
    {
        int spawned = 0;

        while (spawned < maxChainCount)
        {
            while (PlayerMove.inst.IsInteractionState) yield return null;

            SpawnChain();
            spawned++;

            yield return new WaitForSeconds(chainSpawnDelay);
        }
    }

    private void SpawnChain()
    {
        float randomZRot = Random.Range(-20f, 20f);
        Quaternion rotation = Quaternion.Euler(0, 0, randomZRot);

        Vector2 spawnPos = new Vector2(player.transform.position.x, chainPrefab.transform.position.y);

        GameObject chain = Instantiate(chainPrefab, spawnPos, rotation);
        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.BossAttack);

        if (boss.isDie) Destroy(chain);
    }
    #endregion

    #region Dark Falling Pattern
    private IEnumerator SpawnDarkFallingObjects()
    {
        if (boss.isDie || darkFallingPrefab == null) yield break;

        for (float x = startX; x >= endX; x -= xInterval)
        {
            while (PlayerMove.inst.IsInteractionState) yield return null;

            Vector2 pos = new(x, spawnY);
            GameObject df = Instantiate(darkFallingPrefab, pos, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);

            if (boss.isDie) Destroy(df);
        }
    }
    #endregion

    #region Eyes Of Doom Pattern
    private void SpawnEyesOfDoom()
    {
        if (eyesOfDoomPrefab == null) return;

        float randomX = Random.Range(-19f, 19f);
        Vector2 spawnPos = new(randomX, eyesOfDoomPrefab.transform.position.y);

        Instantiate(eyesOfDoomPrefab, spawnPos, Quaternion.identity);
    }
    #endregion

    #region Monster Summon
    private void SummonMonsters()
    {
        for (int i = 0; i < monsterPrefabs.Length; i++)
        {
            float randomX = Random.Range(-monsterSpawnRangeX, monsterSpawnRangeX);
            Vector2 pos = new(randomX, monsterSpawnY);

            GameObject mon = Instantiate(monsterPrefabs[i], pos, Quaternion.identity);
            spawnedMonsters.Add(mon);
        }

        hasSpawned = true;
    }
    #endregion

    #region Cleanup
    private void CleanupOnBossDeath()
    {
        foreach (BossPatternEyesCtrl eyes in FindObjectsOfType<BossPatternEyesCtrl>())
            Destroy(eyes.gameObject);

        foreach (GameObject mon in spawnedMonsters)
            if (mon != null) Destroy(mon);

        spawnedMonsters.Clear();
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] GameObject platformPrefab;
    [SerializeField] GameObject fallPlatformPrefab;
    [SerializeField] Transform platformsHolder;
    [SerializeField] GameObject startPlatform;
    [SerializeField] float platformSpawnOffset = 10f;

    List<Platform> platforms = new List<Platform>();

    GameObject previousPlatform;
    GameObject currentPlatform;
    Platform platform;

    Transform currentPlatformTrans;
    Transform previousPlatformTrans;

    public Transform PreviousPlatformTrans { get { return previousPlatform.transform; } }
    public Transform StartPlatformTrans { get { return startPlatform.transform; } }

    public static event System.Action OnLastTowerReproduced;

    private void OnEnable()
    {
        GameManager.OnGameOver += GameOver;
        GameManager.OnGameStart += Init;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= GameOver;
        GameManager.OnGameStart -= Init;
    }

    private void Start()
    {
        previousPlatform = startPlatform;
        currentPlatform = startPlatform;

        PoolerMono.Instance.Push(platformPrefab, 30);
        PoolerMono.Instance.Push(fallPlatformPrefab);

        ReproduceLastTower();
    }

    private void Init()
    {
        PoolerMono.Instance.DeactivatePool(fallPlatformPrefab);

        foreach (var item in platforms)
            item.gameObject.SetActive(false);

        platforms.Clear();

        previousPlatform = startPlatform;
        currentPlatform = startPlatform;
    }

    private void ReproduceLastTower()
    {
        var lastPlatforms = DataManager.Instance.LastTowerWrapper.platforms;

        if (lastPlatforms.Count < 1)
            return;

        Platform platform;

        foreach (var item in lastPlatforms)
        {
            platform = PoolerMono.Instance.Pull(platformPrefab).GetComponent<Platform>();
            platform.transform.position = item.position;
            platform.transform.localScale = item.scale;
            platform.SetColor(item.color);
            platforms.Add(platform);

            platform.gameObject.SetActive(true);
        }

        previousPlatform = platforms[platforms.Count - 1].gameObject;
        currentPlatform = platforms[platforms.Count - 1].gameObject;

        OnLastTowerReproduced?.Invoke();
    }

    public void SpawnPlatform()
    {
        previousPlatform = currentPlatform;
        currentPlatform = PoolerMono.Instance.Pull(platformPrefab);
        platform = currentPlatform.GetComponent<Platform>();
        platforms.Add(platform);
        currentPlatform.transform.SetParent(platformsHolder);
        platform.Init(previousPlatform.transform, platformSpawnOffset);
    }

    public bool CheckPlatformCollide()
    {
        bool collide = true;

        previousPlatformTrans = previousPlatform.transform;
        currentPlatformTrans = currentPlatform.transform;

        float distanceX = Mathf.Abs(previousPlatformTrans.position.x - currentPlatformTrans.position.x);
        float maxDistanceX = previousPlatformTrans.localScale.x / 2f + currentPlatformTrans.localScale.x / 2f;
        bool noCollisionX = maxDistanceX <= distanceX;

        float distanceZ = Mathf.Abs(previousPlatformTrans.position.z - currentPlatformTrans.position.z);
        float maxDistanceZ = previousPlatformTrans.localScale.z / 2f + currentPlatformTrans.localScale.z / 2f;
        bool noCollisionZ = maxDistanceZ <= distanceZ;

        if (noCollisionX || noCollisionZ)
            collide = false;

        return collide;
    }

    private void GameOver()
    {
        currentPlatform.SetActive(false);
        SaveLastTower();
    }

    private void SaveLastTower()
    {
        List<PlatformData> platformsData = new List<PlatformData>();
        PlatformData platformData;

        foreach (var item in platforms)
        {
            if (item.gameObject.activeInHierarchy)
            {
                platformData.color = item.Color;
                platformData.position = item.transform.position;
                platformData.scale = item.transform.localScale;
                platformsData.Add(platformData);
            }
        }

        DataManager.Instance.LastTowerWrapper.platforms = platformsData;
    }
}

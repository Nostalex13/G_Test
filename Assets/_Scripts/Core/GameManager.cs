using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] PlatformManager platformSpawnManager;
    [SerializeField] Camera mainCamera;

    public Camera MainCamera { get { return mainCamera; } }
    public PlatformManager PlatformManager { get { return platformSpawnManager; } }

    public static event System.Action OnGameStart;
    public static event System.Action OnGameOver;
    public static event System.Action OnPlatformsCollided;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        TapController.OnTap += GameStart;
    }

    private void OnDisable()
    {
        TapController.OnTap -= GameStart;
        Platform.OnPlatformStop -= CheckPlatformPosition;
    }

    private void GameStart()
    {
        DataManager.Instance.Score = 0;
        OnGameStart?.Invoke();

        platformSpawnManager.SpawnPlatform();

        TapController.OnTap -= GameStart;
        Platform.OnPlatformStop -= CheckPlatformPosition;
        Platform.OnPlatformStop += CheckPlatformPosition;
    }

    private void CheckPlatformPosition(GameObject platform)
    {
        if (platformSpawnManager.CheckPlatformCollide())
        {
            OnPlatformsCollided?.Invoke();
            platformSpawnManager.SpawnPlatform();
            DataManager.Instance.Score++;
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        OnGameOver?.Invoke();
        TapController.OnTap += GameStart;
    }
}

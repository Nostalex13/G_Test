using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    private static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataManager();
                _instance.Init();
            }

            return _instance;
        }
    }

    public LastTowerWrapper LastTowerWrapper { get; set; }

    int _score;
    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            OnScoreChange?.Invoke(_score);

            if (_score > HighScore)
                HighScore = _score;
        }
    }
    public int HighScore { get; private set; }

    const string HIGH_SCORE = "HighScore";
    const string LAST_TOWER = "LastTower";

    public static event System.Action<int> OnScoreChange;
    
    private void Init()
    {
        _score = 0;

        if (PlayerPrefs.HasKey(HIGH_SCORE))
        {
            HighScore = PlayerPrefs.GetInt(HIGH_SCORE);
        }
        else
        {
            HighScore = 0;
            PlayerPrefs.SetInt(HIGH_SCORE, HighScore);
        }

        if (PlayerPrefs.HasKey(LAST_TOWER))
        {
            LastTowerWrapper = JsonUtility.FromJson<LastTowerWrapper>(PlayerPrefs.GetString(LAST_TOWER));
        }
        else
        {
            LastTowerWrapper = new LastTowerWrapper();
        }

        AddListeners();
    }

    private void AddListeners()
    {
        ApplicationActivity.OnAppFocus += ApplicationFocus;
    }

    private void ApplicationFocus(bool pause)
    {
        if (!pause)
        {
            SaveData();
        }
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(HIGH_SCORE, HighScore);

        if (LastTowerWrapper.platforms.Count > 0)
            PlayerPrefs.SetString(LAST_TOWER, JsonUtility.ToJson(LastTowerWrapper));
    }
}

[System.Serializable]
public class LastTowerWrapper
{
    public List<PlatformData> platforms = new List<PlatformData>();
}

[System.Serializable]
public struct PlatformData
{
    public Vector3 position;
    public Vector3 scale;
    public Color color;
}

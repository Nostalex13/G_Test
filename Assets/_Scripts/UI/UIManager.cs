using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] TweenAlphaUI menuTextHolderTween;
    [SerializeField] TweenAlphaUI currentScoreTween;

    [Space]
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] TextMeshProUGUI currentScoreText;

    private void OnEnable()
    {
        GameManager.OnGameOver += GameOver;
        GameManager.OnGameStart += GameStart;
        DataManager.OnScoreChange += UpdateScore;
    }

    private void OnDisable()
    {
        GameManager.OnGameOver -= GameOver;
        GameManager.OnGameStart -= GameStart;
        DataManager.OnScoreChange -= UpdateScore;
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        menuTextHolderTween.gameObject.SetActive(true);
        currentScoreTween.gameObject.SetActive(true);
        currentScoreTween.ResetToBeginning();
        highScoreText.text = DataManager.Instance.HighScore.ToString();
    }

    private void GameStart()
    {
        menuTextHolderTween.ResetToEnding();
        menuTextHolderTween.PlayReverse();
        currentScoreTween.PlayForward();
    }

    private void GameOver()
    {
        highScoreText.text = DataManager.Instance.HighScore.ToString();
        currentScoreTween.ResetToEnding();
        currentScoreTween.PlayReverse();
        menuTextHolderTween.PlayForward();
    }

    private void UpdateScore(int score)
    {
        currentScoreText.text = score.ToString();
    }
}

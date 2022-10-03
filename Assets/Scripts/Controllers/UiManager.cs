using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    [Header("UIs")]
    public Image lifeBar;
    public Image projectBar;
    public TMP_Text tmpCoins;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;
    public TMP_Text scoreText;

    [Header("---Game Win---")]
    public TMP_Text score_text;
    public TMP_Text coin_text;

    [Header("Variaveis de controle")]
    public float life_current;
    public float project_current;

    //Scripts
    public SpawnProjectile _spawnProjectile;
    public PlayerRun _playerRun;
    private void Start()
    {
        //Procura Scripts
        _playerRun = FindObjectOfType<PlayerRun>();
        _spawnProjectile = FindObjectOfType<SpawnProjectile>();
        //Ajusta os valores iniciais da UI
        life_current = _playerRun.maxLife;
        project_current = _spawnProjectile.maxProjectile;
        lifeBar.fillAmount = life_current;
        projectBar.fillAmount = project_current;
        gameOverPanel.SetActive(false);
        gameWinPanel.SetActive(false);
    }

    public void UpdateLife(float lives)
    {
        lifeBar.fillAmount = lives / _playerRun.maxLife;
    }
    public void UpdateProjectile(float project)
    {
        projectBar.fillAmount = project / _spawnProjectile.maxProjectile;
    }
    public void UpdateCoins(int coin)
    {
        tmpCoins.text = coin.ToString();
    }
    public void SceneEntrada()
    {
        GameController._gameController.EndGame();
        Time.timeScale = 1;
    }
    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score + "M";
    }

    public void UpdateGameWin(float score, int coin)
    {
        score_text.text = $"Socre: {score.ToString("N0")} M";
        coin_text.text = coin.ToString();
    }
}

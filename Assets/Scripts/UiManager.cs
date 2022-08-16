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
}

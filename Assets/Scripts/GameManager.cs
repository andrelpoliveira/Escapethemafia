using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [HideInInspector]
    public int distance;
    [HideInInspector]
    public int distance_max;
    [HideInInspector]
    public int coin;
    [HideInInspector]
    public int coin_max;

    [Header("---Panel---")]
    public GameObject panel_game;
    public GameObject panel_game_over;

    [Header("---HUD--")]
    public TMP_Text distance_txt;
    public TMP_Text coin_txt;

    [Header("---Game Over--")]
    public TMP_Text distance_go_txt;
    public TMP_Text coin_go_txt;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        distance_txt.text = "0 M";
        coin_txt.text = "0";
        if (!PlayerPrefs.HasKey("distance"))
        {
            PlayerPrefs.SetInt("distance", distance);
        }
        else
        {
            distance_max = PlayerPrefs.GetInt("distance");
        }

        if (!PlayerPrefs.HasKey("coin"))
        {
            PlayerPrefs.SetInt("coin", coin);
        }
        else
        {
            coin_max = PlayerPrefs.GetInt("coin");
        }
    }

    // controle de distancia e pontos
    public void UpdateHUD()
    {
        distance = Mathf.RoundToInt(Player.instance.transform.position.z);
        distance_txt.text = $"{distance} M";
        coin_txt.text = coin.ToString("N0");
    }

    // game over
    public void GameOver()
    {
        Time.timeScale = 0;
        panel_game.SetActive(false);
        panel_game_over.SetActive(true);

        if (distance > PlayerPrefs.GetInt("distance"))
        {
            PlayerPrefs.SetInt("distance", distance);
            distance_max = distance;
        }

        if (coin > PlayerPrefs.GetInt("coin"))
        {
            PlayerPrefs.SetInt("coin", coin);
            coin_max = coin;
        }

        distance_go_txt.text = $"Distância: {distance} \nDistância Máxima: {distance_max}";
        coin_go_txt.text = $"Moedas: {coin} \nMoedas Máxima: {coin_max}";
    }

    // Jogar novamente
    public void PlayAgain()
    {
        panel_game.SetActive(true);
        panel_game_over.SetActive(false);
        Player.instance.LifeAlteration("Recovery", 100);
        Player.instance.ShootAlteration("Recovery", 100);
        Time.timeScale = 1;
    }
}

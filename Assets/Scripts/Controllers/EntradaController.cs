using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntradaController : MonoBehaviour
{
    [Header("UI's")]
    public GameObject fadeInPanel;
    public GameObject painelMission;
    public TMP_Text[] description, reward, progress;
    public TMP_Text coinsTxt, costTxt;
    public GameObject[] rewardButton;
    public GameObject[] characters;

    //Controle dos Personagens
    private int characterIndex = 0;
    private void Start()
    {
        painelMission.GetComponent<Animator>().SetBool("Interaction", false);
        fadeInPanel.SetActive(false);
        SetMission();
        Updatecoins(GameController._gameController.coins);
    }

    public void Updatecoins(int coins)
    {
        coinsTxt.text = coins.ToString();
    }
    //StartGame Fase 01
    public void NextLevel()
    {
        if(GameController._gameController.characterCost[characterIndex] <= GameController._gameController.coins)
        {
            GameController._gameController.coins -= GameController._gameController.characterCost[characterIndex];
            GameController._gameController.characterCost[characterIndex] = 0;
            GameController._gameController.Save();
            painelMission.GetComponent<Animator>().SetBool("Interaction", true);
            StartCoroutine(FadeInPanel());
            GameController._gameController.StartGame(characterIndex);
        }
        
    }
    //Definição das missões
    public void SetMission()
    {
        for (int i = 0; i < 2; i++)
        {
            MissionBase mission = GameController._gameController.GetMission(i);
            description[i].text = mission.GetMissionDescription();
            reward[i].text = "Reward: " + mission.reward;
            progress[i].text = mission.progress + mission.currentProgress + "/" + mission.max;
            if (mission.GetMissionComplete())
            {
                rewardButton[i].SetActive(true);
            }
        }
        GameController._gameController.Save();
    }
    //Corrotina para iniciar o fadeIn
    IEnumerator FadeInPanel()
    {
        yield return new WaitForSeconds(1.2f);
        characters[characterIndex].SetActive(false);
        fadeInPanel.SetActive(true);
    }

    public void GetReward(int missionIndex)
    {
        GameController._gameController.coins += GameController._gameController.GetMission(missionIndex).reward;
        Updatecoins(GameController._gameController.coins);
        rewardButton[missionIndex].SetActive(false);
        GameController._gameController.GenerateMission(missionIndex);
    }
    //Seleção de Personagens
    public void ChangeCharacter(int index)
    {
        characterIndex += index;
        if(characterIndex >= characters.Length)
        {
            characterIndex = 0;
        }
        else if(characterIndex < 0)
        {
            characterIndex = characters.Length - 1;
        }

        for (int i = 0; i < characters.Length; i++)
        {
            if (i == characterIndex)
                characters[i].SetActive(true);
            else
                characters[i].SetActive(false);
        }

        string cost = "";
        if(GameController._gameController.characterCost[characterIndex] != 0)
        {
            cost = GameController._gameController.characterCost[characterIndex].ToString();
        }
        costTxt.text = "Cost: " + cost;
    }
}

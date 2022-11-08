using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Random = UnityEngine.Random;

[Serializable]
public class PlayerData
{
    public int coins;
    public int[] max;
    public int[] progress;
    public int[] currentProgress;
    public int[] reward;
    public string[] missionType;
    public int[] characterCost;
}
public class GameController : MonoBehaviour
{
    //Script estático
    public static GameController _gameController;
    //Variáveis públicas para armazenamento das missões
    public int coins;
    //[HideInInspector]
    public int temp_coins;
    //[HideInInspector]
    public float temp_score;
    //Variável de custo dos personagens
    public int[] characterCost;
    public int characterIndex;
    //Missions Controle e Save
    private MissionBase[] missions;
    private string filePath;
    //Cenas do game
    public string scene;
    public string[] sceneName;
    //controle para vitoria
    private int plrs;
    public PlayerRun plr;
    //controle para continue
    public bool is_continue;
    
    private void Awake()
    {
        //Método para manter somente 1 GameController
        if (_gameController == null) { _gameController = this; } else if (_gameController != this) { Destroy(gameObject); }
        DontDestroyOnLoad(gameObject);
        //Caminho de save
        filePath = Application.persistentDataPath + "/playerInfo.dat";
        //Controle das Missões
        missions = new MissionBase[2];
        //Load e inicialização
        if (File.Exists(filePath)) 
        { 
            Load(); 
        }
        else
        {
            for (int i = 0; i < missions.Length; i++)
            {
                GameObject newMission = new GameObject("Mission" + i);
                newMission.transform.SetParent(transform);
                MissionType[] missionType = { MissionType.SingleRun, MissionType.TotalMeters, MissionType.CoinSingleRun };
                int randomType = Random.Range(0, missionType.Length);
                if (randomType == (int)MissionType.SingleRun)
                {
                    missions[i] = newMission.AddComponent<SingleRun>();
                }
                else if (randomType == (int)MissionType.TotalMeters)
                {
                    missions[i] = newMission.AddComponent<TotalMeters>();
                }
                else if (randomType == (int)MissionType.CoinSingleRun)
                {
                    missions[i] = newMission.AddComponent<CoinSingleRun>();
                }
                missions[i].Create();
            }
        }
        
    }
    //Salvar em modo binário
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);

        PlayerData data = new PlayerData();

        data.coins = coins;
        data.max = new int[2];
        data.progress = new int[2];
        data.currentProgress = new int[2];
        data.reward = new int[2];
        data.missionType = new string[2];
        data.characterCost = new int[characterCost.Length];

        for (int i = 0; i < 2; i++)
        {
            data.max[i] = missions[i].max;
            data.progress[i] = missions[i].progress;
            data.currentProgress[i] = missions[i].currentProgress;
            data.reward[i] = missions[i].reward;
            data.missionType[i] = missions[i].missionType.ToString();
        }

        for (int i = 0; i < characterCost.Length; i++)
        {
            data.characterCost[i] = characterCost[i];
        }

        bf.Serialize(file, data);
        file.Close();
    }
    //Load dos dados binários
    void Load()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(filePath, FileMode.Open);

        PlayerData data = (PlayerData)bf.Deserialize(file);
        file.Close();

        coins = data.coins;

        for (int i = 0; i < 2; i++)
        {
            GameObject newMission = new GameObject("Mission" + i);
            newMission.transform.SetParent(transform);
            if(data.missionType[i] == MissionType.SingleRun.ToString())
            {
                missions[i] = newMission.AddComponent<SingleRun>();
                missions[i].missionType = MissionType.SingleRun;
            }
            else if(data.missionType[i] == MissionType.TotalMeters.ToString())
            {
                missions[i] = newMission.AddComponent<TotalMeters>();
                missions[i].missionType = MissionType.TotalMeters;
            }
            else if(data.missionType[i] == MissionType.CoinSingleRun.ToString())
            {
                missions[i] = newMission.AddComponent<CoinSingleRun>();
                missions[i].missionType = MissionType.CoinSingleRun;
            }

            missions[i].max = data.max[i];
            missions[i].progress = data.progress[i];
            missions[i].currentProgress = data.currentProgress[i];
            missions[i].reward = data.reward[i];
        }

        for (int i = 0; i < data.characterCost.Length; i++)
        {
            characterCost[i] = data.characterCost[i];
        }
        
    }
  
    //Start Game
    public void StartGame(int charIndex)
    {
        characterIndex = charIndex;
        StartCoroutine(LoadSceneGame());
    }
    //End Game
    public void EndGame()
    {
        temp_coins = 0;
        temp_score = 0;
        StartCoroutine(LoadSceneEntrada());
    }

    //Restart Game
    public void RestartGame()
    {
        SceneManager.LoadScene(sceneName[1]);
    }

    //troca de cena para o jogo offline
    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName[1]);
    }

    //Corrotina para chamar a fase01
    IEnumerator LoadSceneGame()
    {
        yield return new WaitForSeconds(20f);
        NetworkController.instance.ChangeScene();
    }
    //Corrotina para chamar a Entrada
    IEnumerator LoadSceneEntrada()
    {
        yield return new WaitForSeconds(.3f);
        SceneManager.LoadScene(sceneName[0]);
    }
    //Retorna as missões na tela de entrada
    public MissionBase GetMission(int index)
    {
        return missions[index];
    }
    //Inicia as missões
    public void StartMissions()
    {
        for (int i = 0; i < 2; i++)
        {
            missions[i].RunStart();
        }
    }
    public void GenerateMission(int i)
    {
        Destroy(missions[i].gameObject);

        GameObject newMission = new GameObject("Mission" + i);
        newMission.transform.SetParent(transform);
        MissionType[] missionType = { MissionType.SingleRun, MissionType.TotalMeters, MissionType.CoinSingleRun };
        int randomType = Random.Range(0, missionType.Length);
        if (randomType == (int)MissionType.SingleRun)
        {
            missions[i] = newMission.AddComponent<SingleRun>();
        }
        else if (randomType == (int)MissionType.TotalMeters)
        {
            missions[i] = newMission.AddComponent<TotalMeters>();
        }
        else if (randomType == (int)MissionType.CoinSingleRun)
        {
            missions[i] = newMission.AddComponent<CoinSingleRun>();
        }
        missions[i].Create();

        FindObjectOfType<EntradaController>().SetMission();
    }

    //public void GameWin(int value)
    //{
    //    plrs += value;
    //    plr = FindObjectOfType(typeof(PlayerRun)) as PlayerRun;

    //    if (plrs == 2)
    //    {
    //        plr.WinGame();
    //    }
    //}
}

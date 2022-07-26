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
    public Animator painelSettings;
    public TMP_Text[] description, reward, progress;
    public TMP_Text coinsTxt, costTxt;
    public GameObject[] rewardButton;
    public GameObject[] characters;
    public GameObject btn_message;
    public TMP_Text text_cancel;

    //controle de rotação
    Vector3 get_mouse;
    bool is_setting;

    //Controle dos Personagens
    [HideInInspector]
    public int characterIndex = 0;
    AudioManager audio_manager;
    GameController game_controller;
    private bool settingOpen;
    private NetworkController network;

    private void Start()
    {
        audio_manager = AudioManager.instance;
        game_controller = GameController._gameController;
        painelMission.GetComponent<Animator>().SetBool("Interaction", false);
        fadeInPanel.SetActive(false);
        SetMission();
        Updatecoins(game_controller.coins);
        audio_manager.PlayMusic(audio_manager.start_game, true);
        network = NetworkController.instance;

        if (game_controller.characterCost[characterIndex] == 0)
        {
            costTxt.text = "Selected";
            btn_message.SetActive(false);
        }
    }

    public void Updatecoins(int coins)
    {
        coinsTxt.text = coins.ToString();
    }
    //StartGame Fase 01
    public void NextLevel()
    {
        /// aqui vai fazer a conexão no servidor e buscar os jogadores
        /// depois de 5 segundos vai para jogo offline

        network.Connect();

        if (game_controller.characterCost[characterIndex] <= game_controller.coins)
        {
            game_controller.coins -= game_controller.characterCost[characterIndex];
            game_controller.characterCost[characterIndex] = 0;
            game_controller.Save();
            painelMission.GetComponent<Animator>().SetBool("Interaction", true);
            StartCoroutine(FadeInPanel());
            //game_controller.StartGame(characterIndex);
        }
    }

    //Defini��o das miss�es
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
        game_controller.Save();
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
        game_controller.coins += GameController._gameController.GetMission(missionIndex).reward;
        Updatecoins(game_controller.coins);
        rewardButton[missionIndex].SetActive(false);
        game_controller.GenerateMission(missionIndex);
    }
    //Sele��o de Personagens
    public void ChangeCharacter(int index)
    {
        characterIndex += index;
        if (characterIndex >= characters.Length)
        {
            characterIndex = 0;
        }
        else if (characterIndex < 0)
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
        if (game_controller.characterCost[characterIndex] != 0 && game_controller.coins < game_controller.characterCost[characterIndex])
        {
            cost = game_controller.characterCost[characterIndex].ToString();
            costTxt.text = "Cost: " + cost;
            btn_message.SetActive(true);
        }
        else
        {
            costTxt.text = "Selected";
            btn_message.SetActive(false);
        }
    }

    public void ControlSettings()
    {
        if (is_setting == false)
        {
            is_setting = true;
            painelSettings.SetTrigger("On");

            if (audio_manager.is_mute)
            {
                text_cancel.gameObject.SetActive(true);
            }
            else
            {
                text_cancel.gameObject.SetActive(false);
            }

            StartCoroutine(CloseSetting());
        }
    }

    //fecha as configurações
    IEnumerator CloseSetting()
    {
        yield return new WaitForSeconds(3f);
        painelSettings.SetTrigger("Off");
        yield return new WaitForSeconds(1f);
        is_setting = false;
    }
    //chama paginas de intert
    public void Facebook()
    {
        Application.OpenURL("https://www.facebook.com/SmartSharkCS/");
    }

    //chama saida do jogo
    public void Exit()
    {
        Application.Quit();
    }

    //tira som
    public void Mute()
    {
        audio_manager.is_mute = !audio_manager.is_mute;
        audio_manager.music.mute = !audio_manager.music.mute;
        audio_manager.fx.mute = !audio_manager.fx.mute;

        if (audio_manager.is_mute)
        {
            text_cancel.gameObject.SetActive(true);
        }
        else
        {
            text_cancel.gameObject.SetActive(false);
        }
    }

    //rotação do personagem
    public void Rotate()
    {
        Vector3 mouse = Input.mousePosition;
        if (get_mouse.x > mouse.x)
        {
            characters[characterIndex].transform.Rotate(0, (characters[characterIndex].transform.position.y + mouse.x + 200) * Time.deltaTime, 0);
        }
        else
        {
            characters[characterIndex].transform.Rotate(0, (characters[characterIndex].transform.position.y - mouse.x) * Time.deltaTime, 0);
        }
    }

    //pega mouse
    public void GetMouse()
    {
        get_mouse = Input.mousePosition;
    }

    //volta do personagem para posição inicial
    public void InitialRotate()
    {
        StartCoroutine(initialRotate());
    }

    IEnumerator initialRotate()
    {
        yield return new WaitForSeconds(1f);
        characters[characterIndex].transform.rotation = Quaternion.Lerp(characters[characterIndex].transform.rotation, Quaternion.Euler(0, 180, 0), 1);
    }
}

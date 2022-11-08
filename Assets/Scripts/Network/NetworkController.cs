using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Hastable = ExitGames.Client.Photon.Hashtable;


// controle de todas as funções do multiplayer
public class NetworkController : MonoBehaviourPunCallbacks
{
    public static NetworkController instance;
    private EntradaController entradaController;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        entradaController = FindObjectOfType(typeof(EntradaController)) as EntradaController;
    }

    // conecta no photon
    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // conectou no servidor da photon
    public override void OnConnected()
    {
        print("entrou no servidor");
    }

    // conecta no master
    public override void OnConnectedToMaster()
    {
        print("entrou no master");
        CreateRoom("test");
    }

    // cria lobby
    public void CreateLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    // entra no lobby
    public override void OnJoinedLobby()
    {
        print("entrou no lobby");
    }

    // cria sala
    public void CreateRoom(string name_room)
    {
        RoomOptions room_options = new RoomOptions();

        room_options.IsOpen = true;
        room_options.IsVisible = true;
        room_options.MaxPlayers = 3;

        PhotonNetwork.JoinOrCreateRoom(name_room, room_options, TypedLobby.Default);
        print("cria sala");
    }

    // desconexão do player
    public override void OnDisconnected(DisconnectCause cause)
    {
        print($"desconectou devido {cause}");
    }

    // falha ao entrar na sala
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print($"erro ao entrar na sala {returnCode} menssagem {message}");
    }

    // entrou na sala
    public override void OnJoinedRoom()
    {
        print($"nome da sala {PhotonNetwork.CurrentRoom.Name}");
        print($"players {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach (var plr in PhotonNetwork.CurrentRoom.Players)
        {
            print($"players {plr}");

        }
        GameController._gameController.StartGame(entradaController.characterIndex);
    }

    // troca de cena
    public void ChangeScene()
    {
        print("entrou na troca de cena");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            print("entrou na troca de cena online");
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(2);
            }
        }
        else
        {
            print("entrou na troca de cena offline");
            PhotonNetwork.LeaveRoom();
        }
    }

    // saida da sala
    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
        GameController._gameController.ChangeScene();
    }

    //instancia player
    public void InstantiatePlayer(string name, Vector3 pos, Quaternion quad)
    {
        PhotonNetwork.Instantiate(name, pos, quad);
        //PhotonNetwork.CurrentRoom.
    }

    //desconecta player
    public void Disconect()
    {
        PhotonNetwork.Disconnect();
    }

    //tira player da sala
    public void RemovePlayer()
    {
        PhotonNetwork.LeaveRoom();
    }
}

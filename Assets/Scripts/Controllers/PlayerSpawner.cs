using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject[] players;
    public GameObject[] enemy;
    public GameObject[] persons;

    // Start is called before the first frame update
    void Awake()
    {
        //carrega cena offline
        if (SceneManager.GetActiveScene().name == "TesteGamePlayFred")
        {
            Instantiate(players[GameController._gameController.characterIndex], transform.position, Quaternion.identity);
            persons[GameController._gameController.characterIndex].SetActive(true);
            // teste
            int temp = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (i != GameController._gameController.characterIndex)
                {
                    if (temp == 0)
                    {
                        GameObject obj = Instantiate(enemy[i], transform.position, Quaternion.identity);
                        obj.transform.position = new Vector3(-4, 1, 0);
                        temp++;
                    }
                    else if (temp == 1)
                    {
                        GameObject obj = Instantiate(enemy[i], transform.position, Quaternion.identity);
                        obj.transform.position = new Vector3(4, 1, 0);
                        temp++;
                    }

                }
            }
        }
        //carrega cena online
        else
        {
            NetworkController.instance.InstantiatePlayer(players[GameController._gameController.characterIndex].name, new Vector3(Random.Range(-4, 4), 1, 0), Quaternion.identity);
        }
    }
}

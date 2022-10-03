using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject[] players;
    public GameObject[] enemy;
    public GameObject[] persons;

    // Start is called before the first frame update
    void Awake()
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
}

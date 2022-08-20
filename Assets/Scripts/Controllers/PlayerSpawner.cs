using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject[] players;
    public GameObject[] persons;

    // Start is called before the first frame update
    void Awake()
    {
        Instantiate(players[GameController._gameController.characterIndex], transform.position, Quaternion.identity);
        persons[GameController._gameController.characterIndex].SetActive(true); 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track1 : MonoBehaviour
{
    [Header("Obstacles")]
    public GameObject[] obstacles;
    public Vector2 numberOfObstacles;
    public List<GameObject> newObstacles;

    [Header("Coins")]
    public GameObject coin;
    public Vector2 numberOfCoins;
    public List<GameObject> newCoins;

    [Header("Heart")]
    public GameObject heart;
    public Vector2 numberOfHearts;
    public List<GameObject> newHearts;

    [Header("Projectile")]
    public GameObject ammunition;
    public Vector2 numberOfAmmunition;
    public List<GameObject> newAmunnitions;

    // Start is called before the first frame update
    void Start()
    {
        int newNumberOfObstacles = (int)Random.Range(numberOfObstacles.x, numberOfObstacles.y);
        int newNumberOfCoins = (int)Random.Range(numberOfCoins.x, numberOfCoins.y);
        int newNumberOfHearts = (int)Random.Range(numberOfHearts.x, numberOfHearts.y);
        int newNumberOfAmunnitions = (int)Random.Range(numberOfAmmunition.x, numberOfAmmunition.y);

        for (int i = 0; i < newNumberOfObstacles; i++)
        {
            newObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform));
            newObstacles[i].SetActive(false);
        }
        for (int i = 0; i < newNumberOfCoins; i ++)
        {
            newCoins.Add(Instantiate(coin, transform));
            newCoins[i].SetActive(false);
        }
        for (int i = 0; i < newNumberOfHearts; i++)
        {
            newHearts.Add(Instantiate(heart, transform));
            newHearts[i].SetActive(false);
        }
        for (int i = 0; i < newNumberOfAmunnitions; i++)
        {
            newAmunnitions.Add(Instantiate(ammunition, transform));
            newAmunnitions[i].SetActive(false);
        }

        PositionObstacles();
        PositionCoins();
        PositionHearts();
        PositionAmmunitions();
    }
    //Positionamento dos obstáculos fixos
    void PositionObstacles()
    {
        for (int i = 0; i < newObstacles.Count; i++)
        {
            float posZMin = (330f / newObstacles.Count) + (330f / newObstacles.Count) * i;
            float posZMax = (330f / newObstacles.Count) + (330f / newObstacles.Count) * i + 1;
            newObstacles[i].transform.localPosition = new Vector3(0, 1, Random.Range(posZMin, posZMax));
            newObstacles[i].SetActive(true);
            if(newObstacles[i].GetComponent<ChangeLane>() != null)
            {
                newObstacles[i].transform.localPosition = new Vector3(0, 1f, Random.Range(posZMin, posZMax));
                newObstacles[i].GetComponent<ChangeLane>().PositionLane();
            }
        }
    }
    void PositionCoins()
    {
        float minZPos = 10f;

        for (int i = 0; i < newCoins.Count; i++)
        {
            float maxZPos = minZPos + 5f;
            float randomZPos = Random.Range(minZPos, maxZPos);
            newCoins[i].transform.localPosition = new Vector3(transform.position.x, 1, randomZPos);
            newCoins[i].SetActive(true);
            newCoins[i].GetComponent<ChangeLane>().PositionLane();
            minZPos = randomZPos + 1;
        }
    }
    //Posição dos hearts
    void PositionHearts()
    {
        float minZPos = 10f;

        for (int i = 0; i < newHearts.Count; i++)
        {
            float maxZPos = minZPos + 20f;
            float randomZPos = Random.Range(minZPos, maxZPos);
            newHearts[i].transform.localPosition = new Vector3(transform.position.x, 1, randomZPos);
            newHearts[i].SetActive(true);
            newHearts[i].GetComponent<ChangeLane>().PositionLane();
            minZPos = randomZPos + 1;
        }
    }
    void PositionAmmunitions()
    {
        float minZPos = 20f;

        for (int i = 0; i < newAmunnitions.Count; i++)
        {
            float maxZPos = minZPos + 30f;
            float randomZPos = Random.Range(minZPos, maxZPos);
            newAmunnitions[i].transform.localPosition = new Vector3(transform.position.x, 1, randomZPos);
            newAmunnitions[i].SetActive(true);
            newAmunnitions[i].GetComponent<ChangeLane>().PositionLane();
            minZPos = randomZPos + 1;
        }
    }
    //Reposicionamento dos tracks
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            transform.position = new Vector3(0, 0, transform.position.z + 330 * 2);
            PositionObstacles();
            PositionCoins();
            PositionHearts();
            PositionAmmunitions();
        }
    }
}

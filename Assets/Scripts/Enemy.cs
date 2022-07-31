using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private float timer;
    private float life_current;
    private float convert_float;

    [Header("---Vida---")]
    public float life;
    public Image life_bar;
    public float fire_delay;
    public Transform gun_position;
    public GameObject bullet_prefab;

    private void Start()
    {
        life_current = life;
        convert_float = 100;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            timer += Time.deltaTime;

            if(timer > fire_delay)
            {
                timer = 0;
                Fire();
            }
        }
    }

    // tiro
    public void Fire()
    {
        Instantiate(bullet_prefab, gun_position.position, Quaternion.identity);
    }

    // gerenciador de vida
    public void TakeDamage(int damage)
    {
        life_current -= damage;

        if(life_current <= 0)
        {
            this.gameObject.SetActive(false);
        }

        life_bar.fillAmount = life_current / convert_float;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnProjectileEnemy : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject firepoint;
    public List<GameObject> vfx = new List<GameObject>();
    public int maxProjectile = 5;

    [Header("UI")]
    private GameObject effectToSpawn;

    //Variáveis Privadas
    public int currentProjectile;

    // Start is called before the first frame update
    void Start()
    {
        effectToSpawn = vfx[0];
        currentProjectile = maxProjectile;
    }
    //Spawn do projétil
    public void SpawnFx()
    {
        GameObject vfx;
        if (currentProjectile > 0)
        {
            if (firepoint != null)
            {
                currentProjectile--;
                vfx = Instantiate(effectToSpawn, firepoint.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.Log("No Fire Point");
            }
        }
        else
        {
            currentProjectile = 0;
        }
    }
}

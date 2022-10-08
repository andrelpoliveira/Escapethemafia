using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    public float speed;
    public float fireRate;

    public GameObject muzzlePrefab;
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(muzzlePrefab != null) 
        { 
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            var psMuzzle = muzzleVFX.GetComponent<ParticleSystem>();
            if(psMuzzle != null) { Destroy(muzzleVFX, psMuzzle.main.duration); } else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
            timer += Time.deltaTime;
            if(timer >= 15f) { Destroy(gameObject); }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player" || collision.transform.tag == "Enemy")
        {
            if(collision.transform.GetComponent<PlayerRun>() != null)
            {
                collision.transform.GetComponent<PlayerRun>().Damage();
                print("dano no player");
            }
            else if (collision.transform.GetComponent<EnemyRun>() != null)
            {
                collision.transform.GetComponent<EnemyRun>().Damage();
                print("dano no enemy");
            }
        }
        speed = 0;
        Destroy(gameObject);
    }
}

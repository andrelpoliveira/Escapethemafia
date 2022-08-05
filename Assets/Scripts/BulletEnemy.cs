using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    private Rigidbody rigidbody;
    private float time;

    public float speed;
    public int damage;
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigidbody.AddForce(-Vector3.forward * speed, ForceMode.Impulse);
        time += Time.deltaTime;
        
        if(time > duration)
        {
            Destroy(transform.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Destroy(transform.gameObject);
            Player.instance.LifeAlteration("Damage", damage);
        }
    }
}

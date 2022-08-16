using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    [Header("Objetos")]
    public GameObject   originalObject;
    public GameObject   fracturedObject;
    public GameObject   explosionVFX;
    public BoxCollider  colTrigger;
    [Header("Variaveis de Controle")]
    public float        explosionMinForce = 5;
    public float        explosionMaxForce = 100;
    public float        explosionForceRadius = 10;
    public float        fragScaleFactor = 1;
    public int          health;

    private GameObject  fractObj;

    void Update()
    {
        
    }

    void FractExplode()
    {
        if(originalObject != null)
        {
            originalObject.SetActive(false);
            if(fracturedObject != null)
            {
                fractObj = Instantiate(fracturedObject, originalObject.transform.position, Quaternion.identity) as GameObject;

                foreach(Transform t in fractObj.transform)
                {
                    var rb = t.GetComponent<Rigidbody>();

                    if(rb != null)
                    {
                        rb.AddExplosionForce(Random.Range(explosionMinForce, explosionMaxForce), originalObject.transform.position, explosionForceRadius);
                    }
                    StartCoroutine(Shrink(t, 2));
                }

                Destroy(fractObj, 5);

                if(explosionVFX != null)
                {
                    GameObject exploVFX = Instantiate(explosionVFX) as GameObject;
                    Destroy(exploVFX, 7);
                }
            }
        }
    }

    private void Reset()
    {
        Destroy(fractObj);
        Destroy(this.gameObject);
    }

    IEnumerator Shrink( Transform t, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 newScale = t.localScale;

        while(newScale.x >= 0)
        {
            newScale -= new Vector3(fragScaleFactor, fragScaleFactor, fragScaleFactor);

            t.localScale = newScale;
            yield return new WaitForSeconds(0.05f);
            Reset();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Projectile" && health < 2) 
        { 
            health++; 
        }
        else if(other.tag == "Projectile" && health == 2)
        {
            colTrigger.enabled = false;
            FractExplode();
        }
    }
}

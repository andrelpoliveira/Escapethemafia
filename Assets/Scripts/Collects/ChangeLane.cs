using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLane : MonoBehaviour
{
    public AudioClip sfx;
   public void PositionLane()
    {
        int randomLane = Random.Range(-2, 2);
        transform.position = new Vector3(randomLane, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(this.gameObject.GetComponent<AudioSource>() != null)
            {
                GetComponent<AudioSource>().PlayOneShot(sfx);
            }
        }
    }
}

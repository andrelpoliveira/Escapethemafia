using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLane : MonoBehaviour
{
    public AudioClip sfx;
   public void PositionLane()
    {
        int id_random = Random.Range(0, 5);
        int[] randomLane = { -4, -2, 0, 2, 4 };
        transform.position = new Vector3(randomLane[id_random], transform.position.y, transform.position.z);
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

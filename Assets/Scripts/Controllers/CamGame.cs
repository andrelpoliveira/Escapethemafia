using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamGame : MonoBehaviour
{
    public GameObject player;
    //public Transform followTarget;
    private CinemachineVirtualCamera vcam;
    private bool is_player;
    // Start is called before the first frame update
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null && is_player == false)
        {
            is_player = true;
            player = GameObject.FindGameObjectWithTag("Player");
            vcam.LookAt = player.transform;
            vcam.Follow = player.transform;
        }

        if(player == null && is_player == true)
        {
            Destroy(this.gameObject);
        }
    }
}

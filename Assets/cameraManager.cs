using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraManager : MonoBehaviour
{
    Transform playerT;
    public player player;

    Vector3 startUp;
    void Start()
    {
        playerT= player.transform;
        startUp = transform.up;
    }

    void Update()
    {
        //transform.LookAt(player.position);
       transform.position=Vector3.Slerp(transform.position, playerT.position - playerT.forward * 10,0.1f);
       var look= Vector3.Slerp(transform.position+ transform.forward, transform.position + playerT.forward, 0.1f);
        //look = transform.position + player.forward;
        if (player.isAbsMove) {
            transform.LookAt(look, startUp);
        }
        else {
            var lookUp = Vector3.Slerp(transform.up, playerT.up, 0.01f);
            transform.LookAt(look, lookUp);
        }
    }
}

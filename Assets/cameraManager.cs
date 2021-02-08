using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraManager : MonoBehaviour
{
    public Transform player;
    Vector3 look;
    // Start is called before the first frame update
    void Start()
    {
        look = transform.position + player.forward;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(player.position);
       transform.position=Vector3.Slerp(transform.position, player.position - player.forward * 10,0.1f);
        look= Vector3.Slerp(transform.position+ transform.forward, transform.position + player.forward, 0.1f);
        //look = transform.position + player.forward;
        transform.LookAt(look, transform.up);

    }
}

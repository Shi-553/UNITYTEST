using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rua : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int rua = 0;
    }

    // Update is called once per frame
    void Update()
    {
        rua++;
        if (rua==10)
        {
            rua = 0;
        }
    }
}

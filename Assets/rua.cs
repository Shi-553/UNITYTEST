using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rua : MonoBehaviour{

        int aa = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        aa++;
        if (aa == 10)
        {
            aa = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class asteroid : MonoBehaviour
{


    public GameObject player;
    Vector3 dir;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        dir = player.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(dir*Time.deltaTime);
    }
}

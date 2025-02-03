using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_movement : MonoBehaviour
{

    [SerializeField] private GameObject ghostCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private float camSpeed;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        FollowGhostCam();
        ghostCamera.transform.position = player.transform.position;
    }

    void FollowGhostCam()
    {
        var posToGo = ghostCamera.transform.position;
        posToGo.z = -10;
        var selfPos = this.transform.position;

        //rb.velocity = camSpeed*(posToGo - selfPos);
        rb.AddForce((posToGo - selfPos)*camSpeed);
    }
}

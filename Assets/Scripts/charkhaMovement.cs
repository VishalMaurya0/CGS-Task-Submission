using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charkhaMovement : MonoBehaviour
{

    [SerializeField] private GameObject mainBody;
    [SerializeField] private Vector3 pathInitialPoint;
    [SerializeField] private Vector3 pathFinalPoint;
    [SerializeField] private float speed;

    [SerializeField] private GameObject initial;
    [SerializeField] private GameObject final;

    bool goForward = true;

    private Rigidbody2D rb;


    void Start()
    {
        pathInitialPoint = initial.transform.position;
        pathFinalPoint = final.transform.position;
        mainBody.gameObject.transform.position = pathInitialPoint;
        rb = mainBody.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        Movement(pathInitialPoint, pathFinalPoint);
    }

    void Movement(Vector3 pathInitialPoint, Vector3 pathFinalPoint)
    {

        if (goForward)
        {
            rb.velocity = (pathFinalPoint - pathInitialPoint) / Vector3.Distance(pathFinalPoint, pathInitialPoint) * speed;
            if (Vector3.Distance(mainBody.gameObject.transform.position, pathInitialPoint) > Vector3.Distance(pathFinalPoint, pathInitialPoint))
            { goForward = false; }
        }

        if (!goForward)
        {
            rb.velocity = -(pathFinalPoint - pathInitialPoint) / Vector3.Distance(pathFinalPoint, pathInitialPoint) * speed;
            if (Vector3.Distance(mainBody.gameObject.transform.position, pathFinalPoint) > Vector3.Distance(pathInitialPoint, pathFinalPoint))
            { goForward = true; }
        }
    }
}

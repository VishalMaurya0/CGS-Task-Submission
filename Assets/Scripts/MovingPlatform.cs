using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    [SerializeField] private GameObject mainBody;
    [SerializeField] private Vector3 pathInitialPoint;
    [SerializeField] private Vector3 pathFinalPoint;
    [SerializeField] private Vector2 directionVector;
    [SerializeField] private float speed;
    [SerializeField] private float force;
    [SerializeField] private float correctionForce;
    [SerializeField] private GameObject initial;
    [SerializeField] private GameObject final;
    public GameObject player;
    public bool playerCollided = false;
    [SerializeField] private float playerForce;

    [SerializeField]private bool goForward = true;

    private Rigidbody2D rb;
    public Rigidbody2D rb2;




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
        pathInitialPoint = initial.transform.position;
        pathFinalPoint = final.transform.position;
        directionVector = pathFinalPoint - pathInitialPoint;
        Correction(directionVector);
        if (playerCollided)
        {
            PlayerCollided();
        }
    }

    

    void Movement(Vector3 pathInitialPoint, Vector3 pathFinalPoint)
    {
        if (goForward)
        {
                rb.AddForce((pathFinalPoint - mainBody.transform.position) / Vector3.Distance(pathFinalPoint, mainBody.transform.position) * force * Time.deltaTime * 165);

            if (rb.velocity.magnitude > speed)
            {
                rb.velocity = (rb.velocity/rb.velocity.magnitude)*speed;
            }
            if (Vector3.Distance(mainBody.gameObject.transform.position, pathInitialPoint) > Vector3.Distance(pathFinalPoint, pathInitialPoint))
            { 
                goForward = false;
                rb.velocity = Vector3.zero;
            }
        }
        
        if (!goForward)
        {
                rb.AddForce((pathInitialPoint - mainBody.transform.position) / Vector3.Distance(pathInitialPoint, mainBody.transform.position) * force * Time.deltaTime * 165);

            if (rb.velocity.magnitude > speed)
            {
                rb.velocity = (rb.velocity / rb.velocity.magnitude) * speed;
            }
            if (Vector3.Distance(mainBody.gameObject.transform.position, pathFinalPoint) > Vector3.Distance(pathInitialPoint, pathFinalPoint))
            {   
                goForward = true;
                rb.velocity = Vector3.zero;
            }
        }
    }

    void Correction(Vector2 directionVector)
    {
        Vector3 correctionVector = new Vector2(0 , correctionForce * Time.deltaTime * 165);
        if ((mainBody.transform.position - pathFinalPoint).normalized != correctionVector.normalized)
            if (Slope(mainBody.transform.position - pathFinalPoint) > Slope(directionVector))
            {
                rb.AddForce(correctionVector);
            }
            else if (Slope(mainBody.transform.position - pathFinalPoint) < Slope(directionVector))
            {
                rb.AddForce(-correctionVector);
            }
    }

    void PlayerCollided()
    {
        if (rb.velocity.x > 0)
        {
            if (rb2.velocity.x < rb.velocity.x)
                rb2.AddForce(new Vector2(playerForce * Time.deltaTime * 165, 0));
        }else if(rb.velocity.x < 0)
        {
            if (rb2.velocity.x > rb.velocity.x)
                rb2.AddForce(new Vector2(-playerForce * Time.deltaTime * 165, 0));
        }
    }

    float Slope(Vector2 vector)
    {
        return (vector.y) / (vector.x);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class path : MonoBehaviour
{
    [SerializeField] private GameObject pathObject;
    [SerializeField] private GameObject initial;
    [SerializeField] private GameObject final;
    [SerializeField] private Vector3 pathInitialPoint;
    [SerializeField] private Vector3 pathFinalPoint;
    [SerializeField] private float separation;
    [SerializeField] private Vector3 currentPos;
    [SerializeField]List<GameObject> addedObjects;
    [SerializeField]List<Vector3> objPos;
    int noOfObjects;

    private Rigidbody2D rb;
    void Start()
    {
        pathInitialPoint = initial.transform.position;
        pathFinalPoint = final.transform.position;
        currentPos = pathInitialPoint;
        noOfObjects = (int)(Vector3.Distance(pathFinalPoint, pathInitialPoint) / separation);
        for (int i = 0; i < noOfObjects; i++)
        {
            addedObjects.Add(Instantiate(pathObject, currentPos, Quaternion.identity));
            currentPos = currentPos + (pathFinalPoint - pathInitialPoint) / Vector3.Distance(pathFinalPoint, pathInitialPoint) * separation;
            objPos.Add(currentPos);
        }
    }

    // Update is called once per frame
    void Update()
    {
        pathInitialPoint = initial.transform.position;
        pathFinalPoint = final.transform.position;
        currentPos = initial.transform.position;

        for (int i = 0; i < noOfObjects; i++)
        {

            currentPos = currentPos + (pathFinalPoint - pathInitialPoint) / Vector3.Distance(pathFinalPoint, pathInitialPoint) * separation;
            objPos[i] = (currentPos);
        }
        for (int j = 0; j < noOfObjects; j++)
        {
            addedObjects[j].transform.position = objPos[j];
        }
    }

    
}

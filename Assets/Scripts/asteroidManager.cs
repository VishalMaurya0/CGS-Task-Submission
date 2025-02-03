using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class asteroidManager : MonoBehaviour
{

    public GameObject asteroid;
    public GameObject sign;
    public GameObject player;

    private GameObject signToRemove;
    private GameObject asteroidMoving;


    void Start()
    {
        StartCoroutine(timer());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(5);
        spawnAsteroid();
        yield return new WaitForSeconds(1f);
        Destroy(signToRemove);
        yield return new WaitForSeconds(3.9f);
        Destroy(asteroidMoving);
    }

    void spawnAsteroid()
    {
        signToRemove = Instantiate(sign, player.transform.position + new Vector3(Random.Range(-13.3f, 13.3f), 7f, 0), Quaternion.identity);
        asteroidMoving = Instantiate(asteroid, player.transform.position - (player.transform.position-signToRemove.transform.position)*2 , Quaternion.identity);

        StartCoroutine(timer());
    }
}

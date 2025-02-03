using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bomb : MonoBehaviour
{
    public GameObject Bomb;
    [SerializeField] private GameObject dir1;
    [SerializeField] private GameObject canon;
    GameObject bombToDestroy;
    [SerializeField] private float timer;
    [SerializeField] private float speed;

    void Start()
    {
        StartCoroutine(Timer(timer));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Timer(float timer)
    {
        yield return new WaitForSeconds(timer);
        spawnBomb();
    }

    void spawnBomb()
    {
        bombToDestroy = Instantiate(Bomb, transform.position , Quaternion.identity);
        bombToDestroy.SetActive(true);
        Rigidbody2D rb = bombToDestroy.GetComponent<Rigidbody2D>();

        rb.velocity = (Vector3.Normalize(dir1.transform.position - transform.position))*speed;
        StartCoroutine(Timer(timer));
    }
}

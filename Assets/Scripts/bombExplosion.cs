using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombExplosion : MonoBehaviour
{

    [SerializeField] private ParticleSystem bombPS;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(bombPS, transform.position, Quaternion.identity).Play();
        Instantiate(bombPS, transform.position, Quaternion.identity).Play();
        this.gameObject.SetActive(false);
    }
}

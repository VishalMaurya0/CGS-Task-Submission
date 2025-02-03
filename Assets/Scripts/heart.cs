using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heart : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private ParticleSystem heartParticleSys;
    Player_controller playerController;

    void Start()
    {
        playerController = player.GetComponent<Player_controller>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)          //-----------ground and wall collision check-------------//
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            if(playerController.hearts < 5)playerController.hearts++;
            heartParticleSys.Play();
        }
    }
}

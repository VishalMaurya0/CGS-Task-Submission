using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingPlatformCollisionDetection : MonoBehaviour
{

    [SerializeField] private GameObject movingPlatform;
    MovingPlatform mp;

    void Start()
    {
        mp = movingPlatform.GetComponent<MovingPlatform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            mp.playerCollided = true;
            mp.player = collision.gameObject;
            mp.rb2 = mp.player.GetComponent<Rigidbody2D>();
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            mp.playerCollided = false;
        }
    }
}

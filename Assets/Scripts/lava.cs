using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class lava : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject globalVolume;
    private Rigidbody2D rb;
    [SerializeField] private float speed;

    [SerializeField] private Volume volume;
    Bloom bloom;
    void Start()
    {



        Volume volume = globalVolume.GetComponent<Volume>();

        volume.profile.TryGet<Bloom>(out bloom);

    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) 
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0,-1)*speed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
            rb = player.GetComponent<Rigidbody2D>();
            bloom.tint.value = Color.red;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(1);
        }
    }
}

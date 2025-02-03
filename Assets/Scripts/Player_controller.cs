using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_controller : MonoBehaviour
{
    public int checkpoint;
    public GameObject[] Checkpoints;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 currentVelocity;
    [SerializeField] private float a;
    [SerializeField] private float b;
    [SerializeField] private int Grounded = 2;
    [SerializeField] private float jumpForce;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private float damageForce;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private bool wallSlide = false;
    [SerializeField] private bool onWall = false;
    [SerializeField] private bool onMovingPlatform = false;
    [SerializeField] private int totalHearts;
    [SerializeField] public int hearts;
    [SerializeField] public int coins;
    [SerializeField] private bool gracePeriod_damage = false;
    GameObject movingPlatform;
    private bool doOnceInSomeSeconds = true;
    private GameObject currentDamageObject;
    public GameObject camerA;
    public cameraShake cameraShake;


    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private BoxCollider2D selfCollider;
    public ParticleSystem deathParticleSys;
    public ParticleSystem damageParticleSys;
    public ParticleSystem coinParticleSys;
    public ParticleSystem trailParticle;

    void Start()
    {
        checkpoint = (int)Variables.Saved.Get("Checkpoint");

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        selfCollider = GetComponent<BoxCollider2D>();
        hearts = totalHearts;
        var emmision = damageParticleSys.emission;
        emmision.rateOverTime = 0;
        cameraShake = camerA.GetComponent<cameraShake>();

        GoToCheckpoint();
    }

    void Update()
    {
        checkpoint = (int)Variables.Saved.Get("Checkpoint");

        Movement();
        Animate();
        DoParticle();
        if(gracePeriod_damage && doOnceInSomeSeconds)
        {
            doOnceInSomeSeconds = false;
            StartCoroutine(GracePeriodDamage());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)          //----------------ground and wall collision check-------------//
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Grounded = 2;
            cameraShake.CamShake(0);
        }
        if (collision.gameObject.CompareTag("Lava"))
        {
            currentDamageObject = collision.gameObject;
            Damage(1);
        }
        if (collision.gameObject.CompareTag("Lava3"))
        {
            currentDamageObject = collision.gameObject;
            Damage(3);
            cameraShake.CamShake(2);
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            wallSlide = true;
            onWall = true;
        }
        if (collision.gameObject.CompareTag("Moving Platform"))
        {
            onMovingPlatform = true;
            movingPlatform = collision.gameObject;
            Grounded = 2;
            cameraShake.CamShake(0);
        }
        
        if (collision.gameObject.CompareTag("Checkpoint1"))
        {
            if (checkpoint < 1)
                checkpoint = 1;
        }
    }
    void OnCollisionStay2D(Collision2D collision)          //-----------ground and wall collision check-------------//
    {
        if (collision.gameObject.CompareTag("Lava"))
        {
            Damage(1);
        }
    }


    void OnCollisionExit2D(Collision2D collision)          //-----------wall collision check-------------//
    {
        if (collision.gameObject.CompareTag("Wall") )
        {
            wallSlide = false;
            onWall = false;
        }

        if (collision.gameObject.CompareTag("Moving Platform"))
        {
            onMovingPlatform = false;
        }
    }


    void Damage(int value)
    {
        if (!gracePeriod_damage)
        {
            hearts= hearts - value;
            cameraShake.CamShake(1);
            Damage_Jump();
            if (hearts > 0)
            {
                anim.SetTrigger("damage");
            }
            gracePeriod_damage = true;
        }
        if (hearts <= 0)
        {
            anim.SetBool("death", true);
            trailParticle.gameObject.SetActive(false);
            rb.velocity = Vector3.zero;
            deathParticleSys.gameObject.SetActive(true);
            damageParticleSys.gameObject.SetActive(false);
            deathParticleSys.gameObject.transform.localPosition = this.gameObject.transform.localPosition - new Vector3(0,0.8f,0);
            deathParticleSys.Play();
            StartCoroutine (AfterDeathWait());
        }
    }

    void Animate()                          //------------------Animation------------------//
    {
        if (Grounded == 2)
        {
            anim.SetBool("falling", false);
            anim.SetBool("jumping", false);
        }
        if (rb.velocity.y<-.01)
        {
            anim.SetBool("jumping", false);
            anim.SetBool("falling", true);
        }else if(rb.velocity.y>.01)
        {
            anim.SetBool("falling", false);
        }

        //anim.SetBool("wallSlide", wallSlide);
    }

    void Movement()
    {

        float x = Input.GetAxis("Horizontal");


        if (Input.GetAxis("Horizontal") != 0)      //------------------Scale Changing and animation-------------------//
        {
            anim.SetBool("walking", true);
            if (x < -0.01f)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (x > 0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else if (Input.GetAxis("Horizontal") == 0)  //-----------------walk animation-----------------------//
        {
            anim.SetBool("walking", false);
        }

        if (Input.GetButtonDown("Jump") && (Grounded >= 1 || onWall))       ////-------------jump and animation--------------////
        {
            if (!onWall && Grounded >= 1)                                     //------------wall jump------------------//
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                Grounded--;
                anim.SetBool("jumping", true);
            }
            else if (onWall) {
                Wall_Jump();
                onWall = false;
            }
        }

        if (rb.velocity.y < -.01 && wallSlide)              //------------------wall slide----------------//
        {
             rb.gravityScale = 0;
             rb.velocity = new Vector2(rb.velocity.x, wallSlideSpeed);
        }
        else { rb.gravityScale = 10; }

        
        
        rb.velocity = rb.velocity + new Vector2(x * moveSpeed * Time.deltaTime, 0);                  //---------------------movement----------------//
        currentVelocity= rb.velocity;
                  

    }

    void DoParticle()
    {
        if (hearts < totalHearts)
        {
            var emmision = damageParticleSys.emission;
            emmision.rateOverTime = (totalHearts - hearts) * 20;
        }
    }

    void Wall_Jump()
    {
        rb.AddForce(new Vector2(wallJumpForce * (this.transform.localScale.x), jumpForce/1.5f), ForceMode2D.Impulse);
    }
    void Damage_Jump()
    {
        rb.AddForce((transform.position -currentDamageObject.gameObject.transform.position)/ Vector2.Distance(transform.position , currentDamageObject.gameObject.transform.position) * damageForce, ForceMode2D.Impulse);
        rb.gravityScale = 10;
        wallSlide = !true;
    }
    
    IEnumerator GracePeriodDamage()
    {      
        yield return new WaitForSeconds(2);
        gracePeriod_damage = false;
        doOnceInSomeSeconds = true;
    }
    
    IEnumerator AfterDeathWait()
    {      
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
    }

    void GoToCheckpoint()
    {
        transform.position = Checkpoints[checkpoint-1].transform.position;
    }
}

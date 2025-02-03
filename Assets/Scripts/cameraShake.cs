using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class cameraShake : MonoBehaviour
{

    Animator anim;
    [SerializeField] private GameObject camController;
    void Start()
    {
        anim = camController.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CamShake(int value)
    {
        switch (value)
        {
            case 0: anim.SetTrigger("CamShakeL"); break;
            case 1: anim.SetTrigger("CamShake"); break;
            case 2: anim.SetTrigger("CamShakeS"); break;
        }
    }
}

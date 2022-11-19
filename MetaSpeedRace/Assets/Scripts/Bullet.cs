using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody bulletRigidbody;
    [SerializeField] float bulletSpeed=16f;
    [SerializeField] bool destroyed = false;
    [SerializeField] float LifeSpan=1f;


    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        
        bulletRigidbody.velocity = transform.forward * bulletSpeed;
        Invoke("DestroyThis",LifeSpan);
    }
    private void OnTriggerEnter(Collider other)
    {
        DestroyThis();
    }

    void DestroyThis()
    {
        if (!destroyed)
        {
            destroyed = true;
            Destroy(gameObject);
        }
    }
}

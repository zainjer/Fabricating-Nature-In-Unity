using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEventsListener : MonoBehaviour
{

    public delegate void onCollision(Collision collision);
    public delegate void onTrigger(Collider collider);


    public event onCollision onCollisionEnter;
    public event onCollision onCollisionStay;
    public event onCollision onCollisionExit;

    public event onTrigger onTriggerEnter;
    public event onTrigger onTriggerStay;
    public event onTrigger onTriggerExit;

    private void OnCollisionEnter(Collision collision)
    {
        onCollisionEnter?.Invoke(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        onCollisionStay?.Invoke(collision);
    }


    private void OnCollisionExit(Collision collision)
    {
        onCollisionExit?.Invoke(collision);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        onTriggerStay?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke(other);
    }
}


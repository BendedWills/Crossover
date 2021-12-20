using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public PlayerMovement movement;

    void OnCollisionEnter(Collision collisionInfo)
    {
        Debug.Log("yep");
        if (collisionInfo.collider.tag == "GravityReset")
        {
            movement.constantGravity = false;
        }
    }
}

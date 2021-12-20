using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2PlayerInit : MonoBehaviour, PlayerInitializer
{
    public void InitPlayer(GameObject player, GameObject camera, PlayerMovement movement, MouseLook mouseLook)
    {
        movement.SetEnabled(true);
        mouseLook.SetEnabled(true);
        movement.enableMovement = false;

        player.AddComponent<PlayerCollision>();
        player.GetComponent<PlayerCollision>().movement = movement;
    }
}

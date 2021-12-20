using UnityEngine;

public interface PlayerInitializer
{
    void InitPlayer(GameObject player, GameObject camera, PlayerMovement movement, MouseLook mouseLook);
}

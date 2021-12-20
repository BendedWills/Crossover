using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public GameObject destroyedObject;

    public void Destroy()
    {
        Instantiate(destroyedObject, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}

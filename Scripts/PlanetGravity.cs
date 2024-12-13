using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    public Transform planet;
    public bool alignToPlanet = true;

    // Gravity strength
    [SerializeField] float gravityConstant = 9.8f;
    Rigidbody r;


    void Start()
    {
        r = GetComponent<Rigidbody>();
    
    }

    void FixedUpdate()
    {
        // Position between the orgin of planet and orgin of player object
        Vector3 toCenter = planet.position - transform.position;
        toCenter.Normalize();

        // Gravity force
        r.AddForce(toCenter * gravityConstant, ForceMode.Acceleration);

        if (alignToPlanet)
        {

            Quaternion q = Quaternion.FromToRotation(transform.up, -toCenter);
            q = q * transform.rotation;
            // Slerp for smooth rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, q, 1);
        }
    }
}
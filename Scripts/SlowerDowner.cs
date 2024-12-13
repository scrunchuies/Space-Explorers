using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace slowDownTime
{
    public class SlowerDowner : MonoBehaviour
    {
        // Points and slow-down properties
        public GameObject pointA;
        public GameObject pointB;
        public static float slowDownAmount = 1f;
        public float slowDownLength = 2f;

        public void slowMotion(float slowDownFactor)
        {
            Time.timeScale = slowDownFactor;
            slowDownAmount = slowDownFactor;
            Time.fixedDeltaTime = Time.timeScale * 0.01f;
            
        }

        public float getSlowDownFactor()
        {
            return slowDownAmount;
        }

        void Update()
        {
            // Calculate the distance between pointA and pointB
            float distance = Vector3.Distance(pointA.transform.position, pointB.transform.position);

            // Slow down the time based on the distance (if less than 10)
            if (distance < 10)
            {
                slowMotion(distance / 10);
            }
            else
            {
                // Reset to normal time scale when out of range
                slowMotion(1f);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Private

    [Header("===== Gameobject Rotation Speed =====")]
    [SerializeField] Vector3 objectRotation;

    // Update is called once per frame
    void Update()
    {
        // Update objectRotation with the values set in the Inspector
        

        // Rotate the object continuously based on the speed
        transform.Rotate(objectRotation * 100 * Time.deltaTime);
        
    }
}

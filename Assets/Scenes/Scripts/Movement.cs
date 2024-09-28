using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Private
    Rigidbody rb;
    AudioSource audioSource;

    [SerializeField] float mainThrust = 100;
    [SerializeField] float rotationThrust = 1f;

    // Public



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessThrust();
        ProcessRotation();
    }

    // User Defined Functions
    void ProcessThrust()
    {
        if (Input.GetKey(KeyCode.Space)) { 
            rb.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
        
    }

    void ProcessRotation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            ApplyRotation(rotationThrust);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            ApplyRotation(-rotationThrust);
        }
    }

    private void ApplyRotation(float rotateThisFrame)
    {
        rb.freezeRotation = true; // Freezing Rotation so we can Manually Rotate
        transform.Rotate(Vector3.forward * rotateThisFrame * Time.deltaTime);
        rb.freezeRotation = false; // Unfreezing roatation so the Physics System can take Over

    }
}

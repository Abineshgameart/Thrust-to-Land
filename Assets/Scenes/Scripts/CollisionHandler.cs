using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEditor.VersionControl;

public class CollisionHandler : MonoBehaviour
{
    // Parivate
    AudioSource audioSource;
    CinemachineVirtualCamera virtualCamera;  // Add this
    [SerializeField]ParticleSystem particleMain;
    

    [SerializeField] float levelLoadDelay = 3f;
    [SerializeField] AudioClip CrashingAudio;
    [SerializeField] AudioClip FinishAudio;

    bool isTransitioning = false;
    bool collisionDisable = false;

    // Public
    public Slider fuelBar;
    public float fuel = 100f;  // Initial fuel level (out of 100)
    public float fullFuelTimeInSeconds = 5f; // 2 minutes as full fuel time
    private float fuelConsumptionRate;  // Fuel consumption rate per second
    private float consumptionMultiplier = 0.5f; // Increase this factor to consume fuel faster

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>(); // Find the virtual camera
        
        // Calculate fuel consumption rate: 100% fuel over 120 seconds
        fuelConsumptionRate = 100f / fullFuelTimeInSeconds;
        if (fuelBar != null)
        {
            // Set the initial value of the fuel bar
            fuelBar.maxValue = 100f;  // Ensure the slider has the correct range
            fuelBar.minValue = 0f;
            fuelBar.value = fuel;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) && fuel > 0)
        {
            FuelReduction();
        }
        RespondToDebugKeys();
        if (fuel <= 0)
            OutOfFuel();
    }

    void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisable = !collisionDisable; // Toggle Collision
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collisionDisable)  { return; }

        switch(collision.gameObject.tag)
        {
            case "Start":
                Debug.Log("This is Start Point");
                break; 
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartCrashSequence();
                break;
        }

     }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Fuel")
        {
            other.gameObject.SetActive(false);  // Disable the fuel object (hide it)
            FuelAdding();
        }
 
        fuelBar.value = fuel;
    }

    void FuelReduction()
    {
        // Decrease fuel based on the fuel consumption rate and the consumption multiplier
        float fuelDecrease = fuelConsumptionRate * Time.deltaTime * consumptionMultiplier;
        fuel -= fuelDecrease;

        // Clamp the fuel value to ensure it doesn't go below 0
        fuel = Mathf.Clamp(fuel, 0f, 100f);

        // Update the fuel bar slider
        if (fuelBar != null)
        {
            fuelBar.value = fuel;
        }

        // Debug log to track fuel changes
        Debug.Log($"Fuel: {fuel}, Decreased by: {fuelDecrease}");
    }
    void FuelAdding()
    {
        fuel += 35f;
        if (fuel > 100f)
            fuel = 100f;
            
        if (GetComponent<Movement>().enabled == false)
        {
            GetComponent<Movement>().enabled = true;
        }
    }

    void OutOfFuel()
    {
         particleMain.Stop();
         GetComponent<Movement>().enabled = false;
         // Debug.Log("Out of fuel!");
         // You can add more logic here, e.g., disabling controls or triggering a game event
        
    }

    void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(FinishAudio);
        GetComponent<Movement>().enabled = false;
        Invoke("LoadNextLevel", levelLoadDelay);
    }
    void StartCrashSequence()
    {
        isTransitioning = true;
        particleMain.Stop();
        audioSource.Stop();
        audioSource.PlayOneShot(CrashingAudio);
        GetComponent<Movement>().enabled = false;
        virtualCamera.Follow = null;  // Detach the camera from following the player
        virtualCamera.LookAt = null;  // Detach the camera from looking at the player

        Invoke("ReloadLevel", levelLoadDelay);
    }

    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

}

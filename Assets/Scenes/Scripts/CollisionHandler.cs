using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    // Parivate
    AudioSource audioSource;
    CinemachineVirtualCamera virtualCamera;  // Add this

    [SerializeField] float levelLoadDelay = 3f;
    [SerializeField] AudioClip CrashingAudio;
    [SerializeField] AudioClip FinishAudio;

    bool isTransitioning = false;

    bool collisionDisable = false;

    // Public

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>(); // Find the virtual camera
    }

    private void Update()
    {
        RespondToDebugKeys();
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

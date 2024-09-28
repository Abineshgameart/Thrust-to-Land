using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Start":
                Debug.Log("This is Start Point");
                break;
            case "Finish":
                Debug.Log("This is Finish Point");
                break;
            default:
                ReloadLevel();
                break;
        }
     }

    void ReloadLevel()
    {
        SceneManager.LoadScene(0);
    }
}

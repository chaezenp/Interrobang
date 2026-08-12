using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenu : MonoBehaviour
{

    public void OnCreditsPressed()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    public void OnCreditsBackPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

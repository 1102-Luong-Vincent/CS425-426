using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionUI : MonoBehaviour
{
    public GameObject confirmationPanel;
    public string sceneToLoad;

    bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            confirmationPanel.SetActive(true);

            Time.timeScale = 0f;
        }
    }

    public void OnConfirmButton()
    {
        Time.timeScale = 1f;
        confirmationPanel.SetActive(false);
        FadeTransition.Instance.FadeToScene(sceneToLoad);
    }

    public void OnCancelButton()
    {
        confirmationPanel.SetActive(false);
        Time.timeScale = 1f;
        playerInRange = false;
    }
}

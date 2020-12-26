using UnityEngine;
using UnityEngine.SceneManagement;

public class UiBaseButtonManager : MonoBehaviour
{
    public void GoBackToModeSelection()
    {
        SceneManager.LoadScene("OfflineOnline");
    }
}

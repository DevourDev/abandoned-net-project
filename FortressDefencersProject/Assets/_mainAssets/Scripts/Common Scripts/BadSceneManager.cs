using UnityEngine;

namespace FD.Utils
{
    public class BadSceneManager : MonoBehaviour
    {
        public void SwitchToScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        public void SwitchToScene(int sceneID)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneID, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}

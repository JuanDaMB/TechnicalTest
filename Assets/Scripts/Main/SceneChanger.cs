using SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main
{
    public class SceneChanger : MonoBehaviour
    {
        public SaveScriptableData data;
        public void ChangeScene(int scene)
        {
            data.load = false;
            SceneManager.LoadScene(scene);
        }

        public void LoadScene(int scene)
        {
            data.load = true;
            SceneManager.LoadScene(scene);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}

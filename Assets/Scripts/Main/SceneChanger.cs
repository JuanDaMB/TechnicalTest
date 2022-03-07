using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main
{
    public class SceneChanger : MonoBehaviour
    {
        public void ChangeScene(int scene)
        {
            SceneManager.LoadScene(scene);
        }
    }
}

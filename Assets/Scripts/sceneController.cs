using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneController : MonoBehaviour
{
    public void loadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}

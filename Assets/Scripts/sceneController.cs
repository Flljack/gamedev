using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneController : MonoBehaviour
{
    public void loadScene(int index)
    {
        Application.LoadLevel(index);
    }
}

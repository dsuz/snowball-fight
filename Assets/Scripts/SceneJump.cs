using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneJump : MonoBehaviour
{
    [SerializeField] string m_sceneName;
    void Start()
    {
        
    }
    public void NextScene()
    {
        SceneManager.LoadScene(m_sceneName);
    }
}

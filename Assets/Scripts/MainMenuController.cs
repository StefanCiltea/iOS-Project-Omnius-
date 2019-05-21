using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void gotoScene(string sceneName){
        StartCoroutine(LoadScene(sceneName));
    }

    IEnumerator LoadScene(string sceneName){
        // transitionAnim.SetTrigger("end");
        yield return new WaitForSeconds(0);
        SceneManager.LoadScene(sceneName);
    }

    
}

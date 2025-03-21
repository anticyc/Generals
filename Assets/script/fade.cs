using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class fade : MonoBehaviour
{
    public Animator animator;
    public int sceneid;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.DontDestroyOnLoad(animator);
    }
    public void jumptomap()
    {
        StartCoroutine(LoadScene(sceneid));

    }
    IEnumerator LoadScene(int index)
    {
        animator.SetBool("fadein", true);
        animator.SetBool("fadeout", false);
        yield return new WaitForSeconds(2);
        AsyncOperation async = SceneManager.LoadSceneAsync(index);
    }
}

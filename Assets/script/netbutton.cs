using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;
public class netbutton : MonoBehaviour
{
    // Start is called before the first frame update
    public NetworkManager manager;
    public Button host;
    public Button client;
    public Animator animator;
    // public Button server;
    void Start()
    {

        DontDestroyOnLoad(host);
        DontDestroyOnLoad(client);
        DontDestroyOnLoad(animator);
        DontDestroyOnLoad(this);
    }
    void Update()
    {
    }
    public void StartHostAsync()
    {
        StartCoroutine(LoadSceneAndStartHost(1, "host"));
    }
    public void StartClientAsync()
    {
        StartCoroutine(LoadSceneAndStartHost(1, "client"));
    }

    private IEnumerator LoadSceneAndStartHost(int sceneBuildIndex, string cmd)
    {
        // 异步加载场景
        animator.SetBool("fadein", true);
        animator.SetBool("fadeout", false);
        yield return new WaitForSeconds(2);
        if (cmd == "host")
        {
            Debug.Log("host");
            manager.StartHost();
        }
        else if (cmd == "client")
        {
            Debug.Log("client");
            manager.StartClient();
        }
    }

    public void StartClient()
    {
        Debug.Log("client");
        manager.StartClient();
    }
}

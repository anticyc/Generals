using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class quitgame : MonoBehaviour
{
    private NetworkManager nwm;
    public bool isquit = false;

    // Start is called before the first frame update
    void Start()
    {
        nwm = GameObject.Find("networkmanager").GetComponent<NetworkManager>();
    }
    public void quitroom()
    {

        if (GameObject.Find("object(Clone)").GetComponent<Clickmovearmy>().AlivePlayerNum == 0) { Debug.Log("alive1"); nwm.StopHost(); }
        else { nwm.StopClient(); }
    }

    void Update()
    {
        if (isquit) quitroom();
    }
}

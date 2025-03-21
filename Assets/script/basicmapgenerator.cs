using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System;
public class basicmapgenerator : NetworkBehaviour
{
    public GameObject playerlistelement;
    public GameObject idtile;
    public GameObject gamemaps;
    [SyncVar] public int playernum;
    public GameObject player;
    private string[] playertilelist ={"blueplayer","redplayer","greenplayer",
                                    "yellowplayer","brownplayer","purpleplayer","orangeplayer","pinkplayer"};

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            Debug.Log("start");
            GameObject maps = Instantiate(gamemaps, null);
            NetworkServer.Spawn(maps);
        }
        if (isClientOnly) { StartCoroutine(msgboxani("You Have Entered the Room")); StopCoroutine(msgboxani("You Have Entered The Room")); }
        else { StartCoroutine(msgboxani("You Have Created A Room")); StopCoroutine(msgboxani("You Have Created A Room")); }
        if (isServer) playernum = (int)GameObject.Find("gamedata").GetComponent<plslider>().slider.value;
        for (int i = 0; i < playernum; i++)
        {
            GameObject idchoice = Instantiate(idtile, null);
            idchoice.name = "player" + i.ToString();
            idchoice.transform.Find("button").name = "playerconfirm" + i.ToString();
            idchoice.transform.Find("Tiledemo").GetComponent<Image>().sprite
                       = GameObject.Find(playertilelist[i]).GetComponent<SpriteRenderer>().sprite;
            idchoice.transform.Find("playerid").GetComponent<Text>().text = i.ToString();
            idchoice.transform.SetParent(GameObject.Find("id_tile_list").transform);
            idchoice.transform.position = new Vector3(0, 0, 19);
        }

        for (int i = 0; i < playernum; i++)
        {
            GameObject list = GameObject.Find("playerlist");
            GameObject playerlist = Instantiate(playerlistelement, list.transform.position, list.transform.rotation);
            playerlist.name = "miniplayerlist" + i.ToString();
            playerlist.transform.Find("playerid").GetComponent<Text>().text = i.ToString();
            playerlist.transform.Find("Tiledemo").GetComponent<Image>().sprite
                       = GameObject.Find(playertilelist[i]).GetComponent<SpriteRenderer>().sprite;
            playerlist.transform.SetParent(GameObject.Find("playerlist").transform);
        }
    }

    /* 定义了一个名为 msgboxani 的协程函数，用于在Unity中显示一个消息框，
     * 并在3秒后自动隐藏。该函数接受一个字符串参数 msgtext，用于设置消息框中的文本内容。*/
    IEnumerator msgboxani(string msgtext)
    {
        GameObject msg = GameObject.Find("messagebox");
        msg.transform.Find("messagetext").GetComponent<Text>().text = msgtext;
        msg.GetComponent<Animator>().SetBool("msgin", true);
        yield return new WaitForSeconds(3);
        msg.GetComponent<Animator>().SetBool("msgin", false);
        msg.GetComponent<Animator>().SetBool("msgout", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkClient.isConnected) Debug.Log("isconnect");
    }
}

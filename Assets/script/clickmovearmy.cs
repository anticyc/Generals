using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Mirror;
using System.Numerics;
using System.Collections.Generic;
using StinkySteak.NetcodeBenchmark;
using System.Collections;
using UnityEngine.SceneManagement;

public class Clickmovearmy : NetworkBehaviour
{
    private Tilemap shademap;
    private Tilemap tilemap;
    private Tilemap Iconmap;
    public TileBase mountain;
    public TileBase swamp;
    public TileBase graytile;
    public TileBase capital;
    public TileBase city;
    public TileBase shade;
    private Camera maincamera;
    public TileBase user;
    public TileBase lighthouse;
    public TileBase trap;
    private Vector3Int pre_pos; //last shade
    public int Lighthouse_last_called = 0;
    private bool lighthouse_pos_ready = false;
    private bool keyboard_lighthouse = false;
    private bool trap_pos_ready = false;
    public readonly SyncList<Vector3Int> trap_pos = new SyncList<Vector3Int>();
    private Vector3Int Mypos;
    private GameObject msgbox;
    private int preturn = -1;
    private bool islose = false;
    public int AlivePlayerNum;
    private NetworkManager nwm;
    private GameObject msg;
    private Animator fade;
    private Button quitbutton;
    // Start is called before the first frame update
    void Start()
    {
        nwm = GameObject.Find("networkmanager").GetComponent<NetworkManager>();
        msg = GameObject.Find("messagebox");
        fade = GameObject.Find("image").GetComponent<Animator>();
        quitbutton = GameObject.Find("quit").GetComponent<Button>();
        quitbutton.onClick.AddListener(
            () =>
        {
            StartCoroutine(QuitGame());
        }
        );

        // 获取地图组件
        tilemap = GameObject.Find("map").GetComponent<Tilemap>();
        // 获取图标地图组件
        Iconmap = GameObject.Find("iconmap").GetComponent<Tilemap>();
        // 获取阴影地图组件
        shademap = GameObject.Find("shademap").GetComponent<Tilemap>();
        // 获取主摄像机组件
        maincamera = GameObject.Find("mapCamera").GetComponent<Camera>();
        user = GameObject.Find("object(Clone)").GetComponent<Buildmap>().playertile;
        int index = GameObject.Find("object(Clone)").GetComponent<Buildmap>().index;
        Mypos = GameObject.Find("object(Clone)").GetComponent<Buildmap>().playerPos[index];
        pre_pos = Mypos;
        msgbox = GameObject.Find("messagebox");
        AlivePlayerNum = GameObject.Find("object(Clone)").GetComponent<Buildmap>().playernum;
    }

    IEnumerator QuitGame()
    {

        if (isServer && AlivePlayerNum != 1) { StartCoroutine(ToolReady("Host Cannot Quit until Others Quit. Or the Game Will Stop!")); }
        else
        {
            islose = true;
            StartCoroutine(ToolReady("You Have Quited the Room,Waiting For System..."));
            StopCoroutine(ToolReady("You Have Quited the Room,Waiting For System..."));
            int quitindex = GameObject.Find("object(Clone)").GetComponent<Buildmap>().index;
            CmdQuitProcessor(quitindex);

            yield return new WaitForSeconds(5);
            quitbutton.GetComponent<quitgame>().isquit = true;
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdQuitProcessor(int quitindex)
    {
        Rpcquitroom(quitindex);
    }
    [ClientRpc]
    private void Rpcquitroom(int quitindex)
    {
        Destroy(GameObject.Find("miniplayerlist" + quitindex));//需要同步
        AlivePlayerNum--;
    }


    void Update()
    {
        if (AlivePlayerNum == 1 && !islose)
        {
            CmdLoserUpdate("win", 0, user.name); //0可以被替换，不需要这个参数

        }
        if (preturn == GameObject.Find("numcanvas").GetComponent<Numtroops>().TURN) return;
        if (Iconmap.GetTile<TileBase>(Mypos) == city && !islose)
        {
            Debug.Log("be captured"); Destroy(GameObject.Find("mapmask"));
            GameObject obj = GameObject.Find("object(Clone)");
            islose = true;
            int loserindex = obj.GetComponent<Buildmap>().index;
            Debug.Log(loserindex + "   " + user.name);
            CmdLoserUpdate("lose", loserindex, user.name);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            preturn = GameObject.Find("numcanvas").GetComponent<Numtroops>().TURN;
            if (lighthouse_pos_ready)
            {
                // Lighthouse called
                keyboard_lighthouse = true;
                lighthouse_pos_ready = false;
            }
            else if (trap_pos_ready)
            {
                // Trap called
                trap_pos_ready = false;
                // 确认兵力充足
                if (trap_pos.FindIndex(x => x == pre_pos) == -1)
                {
                    // 扣除兵力
                    int capitalsolodiers = int.Parse(GameObject.Find("cell" + Mypos.ToString()).GetComponent<Text>().text);
                    if (capitalsolodiers < 50)
                    {
                        Debug.Log("numtroops in capital is not enough");
                        StartCoroutine(ToolReady("You Don't Have Enough Soldiers in Capital"));
                        return;
                    }
                    GameObject.Find("cell" + Mypos.ToString()).GetComponent<Text>().text = (capitalsolodiers - 50).ToString();
                    GameObject.Find("mask").GetComponent<Tilemap>().SetTile(pre_pos, trap);
                    StartCoroutine(ToolReady("Trap Was Set Here"));
                    // 添加陷阱, 传递给server
                    TrapProcessor(pre_pos);
                }
            }
        }
        // 检测键盘按键按下
        if (Input.GetKeyDown(KeyCode.J))
        {
            preturn = GameObject.Find("numcanvas").GetComponent<Numtroops>().TURN;
            PlaceLighthouse(pre_pos, 4);
            var NEWPOS = NewPosCalc(pre_pos, 4); CmdKeyBoardProcessor(pre_pos, 4, user.name);
            shademap.SetTile(pre_pos, null);
            pre_pos = NEWPOS;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            preturn = GameObject.Find("numcanvas").GetComponent<Numtroops>().TURN;
            PlaceLighthouse(pre_pos, 6);
            var NEWPOS = NewPosCalc(pre_pos, 6); CmdKeyBoardProcessor(pre_pos, 6, user.name); //左
            shademap.SetTile(pre_pos, null);
            pre_pos = NEWPOS;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            preturn = GameObject.Find("numcanvas").GetComponent<Numtroops>().TURN;
            PlaceLighthouse(pre_pos, 2);
            var NEWPOS = NewPosCalc(pre_pos, 2); CmdKeyBoardProcessor(pre_pos, 2, user.name); //上
            shademap.SetTile(pre_pos, null);
            pre_pos = NEWPOS;
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            preturn = GameObject.Find("numcanvas").GetComponent<Numtroops>().TURN;
            PlaceLighthouse(pre_pos, 5);
            var NEWPOS = NewPosCalc(pre_pos, 5); CmdKeyBoardProcessor(pre_pos, 5, user.name); //下
            shademap.SetTile(pre_pos, null);
            pre_pos = NEWPOS;
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            preturn = GameObject.Find("numcanvas").GetComponent<Numtroops>().TURN;
            PlaceLighthouse(pre_pos, 1);
            var NEWPOS = NewPosCalc(pre_pos, 1); CmdKeyBoardProcessor(pre_pos, 1, user.name); //左上
            shademap.SetTile(pre_pos, null);
            pre_pos = NEWPOS;
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            preturn = GameObject.Find("numcanvas").GetComponent<Numtroops>().TURN;
            PlaceLighthouse(pre_pos, 3);
            var NEWPOS = NewPosCalc(pre_pos, 3); CmdKeyBoardProcessor(pre_pos, 3, user.name); //右
            shademap.SetTile(pre_pos, null);
            pre_pos = NEWPOS;
        }
        // 检测鼠标左键点击
        else if (Input.GetMouseButtonDown(0))
        {
            preturn = GameObject.Find("numcanvas").GetComponent<Numtroops>().TURN;
            // 避免模糊引用
            UnityEngine.Vector3 worldPoint = maincamera.ScreenToWorldPoint(new UnityEngine.Vector3(Input.mousePosition.x, Input.mousePosition.y, 87.371f));
            // 将世界坐标转换为瓦片坐标
            Vector3Int tilePos = shademap.WorldToCell(worldPoint);
            if (!tilemap.HasTile(tilePos))
                return;
            if (tilemap.GetTile<TileBase>(tilePos) == mountain)
            {
                // lighthouse_ready
                lighthouse_pos_ready = true;
                // pre_pos 就是 lighthouse_pos
            }
            else if (Iconmap.GetTile<TileBase>(tilePos) == swamp && tilemap.GetTile<TileBase>(pre_pos) == graytile)
            {
                // trap_ready
                trap_pos_ready = true;
                lighthouse_pos_ready = false;
                // pre_pos 就是 trap_pos
            }
            else
            {
                lighthouse_pos_ready = false;
                float magnitude = (pre_pos - tilePos).magnitude;
                if (magnitude == 0)
                {
                    shademap.SetTile(pre_pos, shade);
                    return;
                }
                if (magnitude < 1.5f && (shademap.CellToWorld(tilePos) - shademap.CellToWorld(pre_pos)).magnitude < 7 && tilemap.GetTile<TileBase>(pre_pos) == user) //相邻
                {
                    CmdMouseProcessor(pre_pos, tilePos, user.name);
                }
            }
            shademap.SetTile(pre_pos, null);
            shademap.SetTile(tilePos, shade);
            pre_pos = tilePos;
        }
    }

    // ToolReady(string msgtext) 方法，用于工具准备就绪
    IEnumerator ToolReady(string msgtext)
    {
        Debug.Log("toolready");
        GameObject msg = GameObject.Find("messagebox");
        msg.transform.Find("messagetext").GetComponent<Text>().text = msgtext;
        msg.GetComponent<Animator>().SetBool("msgout", false);
        msg.GetComponent<Animator>().SetBool("msgin", true);

        yield return new WaitForSeconds(3);
        msg.GetComponent<Animator>().SetBool("msgin", false);
        msg.GetComponent<Animator>().SetBool("msgout", true);
    }
    [Command(requiresAuthority = false)]
    public void CmdLoserUpdate(string mode, int loserindex, string otherplayername)
    {
        RpcLoserUpdate(mode, loserindex, otherplayername);
    }
    [ClientRpc]
    private void RpcLoserUpdate(string mode, int loserindex, string otherplayername)
    {

        StartCoroutine(LoserUpdate(mode, loserindex, otherplayername));
    }
    private IEnumerator LoserUpdate(string mode, int loserindex, string otherplayername)
    {
        if (mode == "lose")
        {
            Destroy(GameObject.Find("miniplayerlist" + loserindex));

            AlivePlayerNum--;
            if (user.name != otherplayername)
            {
                Debug.Log("user:" + user.name + "  controler:" + otherplayername);
                GameObject.Find("messagetext").GetComponent<Text>().text = otherplayername + " lose!";
            }
            else
            {
                GameObject.Find("messagetext").GetComponent<Text>().text = "Your Capital Was Captured ! You Lose! \nPress Pause Button to Quit";
                this.enabled = false;
            }
            msgbox.GetComponent<Animator>().SetBool("msgout", false);
            msgbox.GetComponent<Animator>().SetBool("msgin", true);
            yield return new WaitForSeconds(4);
            msgbox.GetComponent<Animator>().SetBool("msgin", false);
            msgbox.GetComponent<Animator>().SetBool("msgout", true);
        }
        else
        {
            if (user.name != otherplayername)
            {
                GameObject.Find("messagetext").GetComponent<Text>().text = otherplayername + "win!";
            }
            else
            {
                GameObject.Find("messagetext").GetComponent<Text>().text = "Congratulations!You are the winner!";
                this.enabled = false;
            }
            msgbox.GetComponent<Animator>().SetBool("msgout", false);
            msgbox.GetComponent<Animator>().SetBool("msgin", true);
            yield return new WaitForSeconds(4);
            msgbox.GetComponent<Animator>().SetBool("msgin", false);
            msgbox.GetComponent<Animator>().SetBool("msgout", true);
            yield return new WaitForSeconds(1.5f);
        }
    }
    private void PlaceLighthouse(Vector3Int tilePos, int direction)
    {
        if (keyboard_lighthouse)
        {
            keyboard_lighthouse = false;
            int CurrentTurn = GameObject.Find("numcanvas").GetComponent<Numtroops>().TURN;
            if (CurrentTurn - Lighthouse_last_called < 100)
            {
                Debug.Log("Lighthouse Build: failed owing to time limit.");
                StartCoroutine(ToolReady("Please Set Lighthouse at Least Every 100 Turns "));
                return;
            }
            int capitalsolodiers = int.Parse(GameObject.Find("cell" + Mypos.ToString()).GetComponent<Text>().text);
            if (capitalsolodiers < 50)
            {
                Debug.Log("numtroops in capital is not enough");
                StartCoroutine(ToolReady("You Don't Have Enough Soldiers in Capital"));
                return;
            }
            GameObject.Find("cell" + Mypos.ToString()).GetComponent<Text>().text = (capitalsolodiers - 50).ToString();
            Lighthouse_last_called = CurrentTurn;
            Vector3Int Direction_POS = tilePos;
            GameObject.Find("mapmask").GetComponent<Mapmask>().cancel(Direction_POS, CurrentTurn);
            GameObject.Find("mask").GetComponent<Tilemap>().SetTile(Direction_POS, lighthouse);
            Debug.Log("lighthouse placed at" + Direction_POS);
            for (int i = 0; i < 4; i++)
            {
                switch (direction)
                {
                    case 1:

                        if (Math.Abs(Direction_POS.y) % 2 == 1)
                            Direction_POS = Direction_POS + new Vector3Int(1, -1, 0);
                        else
                            Direction_POS = Direction_POS + new Vector3Int(0, -1, 0);
                        break;
                    case 2:
                        Direction_POS = Direction_POS + new Vector3Int(1, 0, 0);
                        break;
                    case 3:

                        if (Math.Abs(Direction_POS.y) % 2 == 1)
                            Direction_POS = Direction_POS + new Vector3Int(1, 1, 0);
                        else
                            Direction_POS = Direction_POS + new Vector3Int(0, 1, 0);
                        break;
                    case 4:

                        if (Math.Abs(Direction_POS.y) % 2 == 1)
                            Direction_POS = Direction_POS + new Vector3Int(0, -1, 0);
                        else
                            Direction_POS = Direction_POS + new Vector3Int(-1, -1, 0);
                        break;
                    case 5:
                        Direction_POS = Direction_POS + new Vector3Int(-1, 0, 0);
                        break;
                    case 6:

                        if (Math.Abs(Direction_POS.y) % 2 == 1)
                            Direction_POS = Direction_POS + new Vector3Int(0, 1, 0);
                        else
                            Direction_POS = Direction_POS + new Vector3Int(-1, 1, 0);
                        break;
                }
                GameObject.Find("mapmask").GetComponent<Mapmask>().cancel(Direction_POS, CurrentTurn);
            }
            StartCoroutine(ToolReady("Lighthouse Was Set Here"));
        }
    }



    public bool MoveArmy(Vector3Int from, Vector3Int to, TileBase usertile)
    {
        //数字比较
        string toname = "cell" + to.ToString();
        string fromname = "cell" + from.ToString();
        Text textto = GameObject.Find(toname).GetComponent<Text>();
        Text textfrom = GameObject.Find(fromname).GetComponent<Text>();

        if (textfrom.text == "1") { Debug.Log("1false"); return false; }
        if (Iconmap.GetTile<TileBase>(to) == swamp)
        {
            Debug.Log(trap_pos.FindIndex(pos => pos == to));
            if (trap_pos.FindIndex(pos => pos == to) != -1)
            {
                textto.text = "";
                textfrom.text = "1";
                StartCoroutine("You got into Trap! Be Cautious!");
                return false;
            }
        }
        if (textto.text == "")
        {
            textto.text = (int.Parse(textfrom.text) - 1).ToString();
            textfrom.text = "1";
        }
        else
        {
            int fromnum = int.Parse(textfrom.text);
            int tonum = int.Parse(textto.text);

            textfrom.text = "1";
            if (tilemap.GetTile<TileBase>(to) != usertile)
            {
                if (fromnum > tonum)
                {
                    textto.text = (fromnum - tonum - 1).ToString();
                }
                else
                {
                    textto.text = (tonum + 1 - fromnum).ToString();
                    Debug.Log("less false");
                    return false;//不改色
                }
            }
            else textto.text = (fromnum + tonum - 1).ToString();
        }
        return true;
    }

    [Command(requiresAuthority = false)]
    public void CmdKeyBoardProcessor(Vector3Int tilePos, int direction, string usertilename)
    {
        RpcKeyBoardProcessor(tilePos, direction, usertilename);
    }
    [ClientRpc]
    public void RpcKeyBoardProcessor(Vector3Int tilePos, int direction, string usertilename)
    {
        TileBase otheruser = Resources.Load<TileBase>(usertilename);
        if (!tilemap.HasTile(tilePos) || tilemap.GetTile<TileBase>(tilePos) != otheruser)
            return;
        Vector3Int new_pos = new Vector3Int();
        switch (direction)
        {
            case 1:
                new_pos = tilePos + new Vector3Int(0, -1, 0);
                if (Math.Abs(tilePos.y) % 2 == 1)
                    new_pos = tilePos + new Vector3Int(1, -1, 0);
                break;
            case 2:
                new_pos = tilePos + new Vector3Int(1, 0, 0);
                break;
            case 3:
                new_pos = tilePos + new Vector3Int(0, 1, 0);
                if (Math.Abs(tilePos.y) % 2 == 1)
                    new_pos = tilePos + new Vector3Int(1, 1, 0);
                break;
            case 4:
                new_pos = tilePos + new Vector3Int(-1, -1, 0);
                if (Math.Abs(tilePos.y) % 2 == 1)
                    new_pos = tilePos + new Vector3Int(0, -1, 0);
                break;
            case 5:
                new_pos = tilePos + new Vector3Int(-1, 0, 0);
                break;
            case 6:
                new_pos = tilePos + new Vector3Int(-1, 1, 0);
                if (Math.Abs(tilePos.y) % 2 == 1)
                    new_pos = tilePos + new Vector3Int(0, 1, 0);
                break;
        }

        if (tilemap.HasTile(new_pos) && tilemap.GetTile<TileBase>(new_pos) != mountain)
        {
            if (MoveArmy(tilePos, new_pos, otheruser))
            {
                if (Iconmap.GetTile<TileBase>(new_pos) == capital && tilemap.GetTile<TileBase>(new_pos) != otheruser)
                {
                    Iconmap.SetTile(new_pos, city);
                    TileBase Captured = tilemap.GetTile<TileBase>(new_pos);
                    // 遍历
                    SyncList<Vector3Int> list = GameObject.Find("object(Clone)").GetComponent<Buildmap>().tilePositions;
                    foreach (Vector3Int pos in list)
                    {
                        if (tilemap.GetTile<TileBase>(pos) == Captured)
                        {
                            tilemap.SetTile(pos, otheruser);
                            // cleararound
                            GameObject.Find("mapmask").GetComponent<Mapmask>().cancelaround(pos);
                        }
                    }
                }
                else if (tilemap.GetTile<TileBase>(new_pos) == GameObject.Find("object(Clone)").GetComponent<Buildmap>().playertile)
                {
                    tilemap.SetTile(new_pos, otheruser);
                    GameObject.Find("mapmask").GetComponent<Mapmask>().RemaskAround(new_pos);
                }
                tilemap.SetTile(new_pos, otheruser);
                if (otheruser == user)
                {
                    GameObject.Find("mapmask").GetComponent<Mapmask>().cancelaround(new_pos);
                    shademap.SetTile(new_pos, shade);
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdMouseProcessor(Vector3Int pre_pos, Vector3Int tile_pos, string usertilename)
    {
        RpcMouseProcessor(pre_pos, tile_pos, usertilename);
    }
    [ClientRpc]
    private void RpcMouseProcessor(Vector3Int pre_pos, Vector3Int tile_pos, string usertilename)
    {
        TileBase otheruser = Resources.Load<TileBase>(usertilename);

        if (MoveArmy(pre_pos, tile_pos, otheruser))
        {
            // 吃掉其他玩家的首都，全部收编
            if (Iconmap.GetTile<TileBase>(tile_pos) == capital && tilemap.GetTile<TileBase>(tile_pos) != otheruser)
            {
                Iconmap.SetTile(tile_pos, city);
                TileBase Captured = tilemap.GetTile<TileBase>(tile_pos);
                // 遍历
                SyncList<Vector3Int> list = GameObject.Find("object(Clone)").GetComponent<Buildmap>().tilePositions;
                foreach (Vector3Int pos in list)
                {
                    if (tilemap.GetTile<TileBase>(pos) == Captured)
                    {
                        tilemap.SetTile(pos, otheruser);
                        // cleararound
                        GameObject.Find("mapmask").GetComponent<Mapmask>().cancelaround(pos);
                    }
                }
            }
            else if (tilemap.GetTile<TileBase>(tile_pos) == GameObject.Find("object(Clone)").GetComponent<Buildmap>().playertile)
            {
                // remask
                tilemap.SetTile(tile_pos, otheruser);
                GameObject.Find("mapmask").GetComponent<Mapmask>().RemaskAround(tile_pos);
            }
            if (otheruser == user)
            {
                GameObject.Find("mapmask").GetComponent<Mapmask>().cancelaround(tile_pos);
            }
            tilemap.SetTile(tile_pos, otheruser);
        }
    }
    [Command(requiresAuthority = false)]
    public void TrapProcessor(Vector3Int tilePos)
    {
        trap_pos.Add(tilePos);
    }
    public Vector3Int NewPosCalc(Vector3Int tilePos, int direction)
    {
        Vector3Int new_pos = tilePos;
        switch (direction)
        {
            case 1:
                new_pos = tilePos + new Vector3Int(0, -1, 0);
                if (Math.Abs(tilePos.y) % 2 == 1)
                    new_pos = tilePos + new Vector3Int(1, -1, 0);
                break;
            case 2:
                new_pos = tilePos + new Vector3Int(1, 0, 0);
                break;
            case 3:
                new_pos = tilePos + new Vector3Int(0, 1, 0);
                if (Math.Abs(tilePos.y) % 2 == 1)
                    new_pos = tilePos + new Vector3Int(1, 1, 0);
                break;
            case 4:
                new_pos = tilePos + new Vector3Int(-1, -1, 0);
                if (Math.Abs(tilePos.y) % 2 == 1)
                    new_pos = tilePos + new Vector3Int(0, -1, 0);
                break;
            case 5:
                new_pos = tilePos + new Vector3Int(-1, 0, 0);
                break;
            case 6:
                new_pos = tilePos + new Vector3Int(-1, 1, 0);
                if (Math.Abs(tilePos.y) % 2 == 1)
                    new_pos = tilePos + new Vector3Int(0, 1, 0);
                break;
        }
        return new_pos;
    }
}

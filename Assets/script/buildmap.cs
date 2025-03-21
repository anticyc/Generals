using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using Mirror;
using UnityEditor;
using Unity.Mathematics;
using Unity.VisualScripting;
using System.Threading;
using TMPro;

public class Buildmap : NetworkBehaviour
{
    private Animator anim;
    public TileBase playertile;
    public int index = -1;
    public Tilemap tilemap;  // 指定要操作的Tilemap
    public Tilemap Iconmap;//把会变色的格子（city等会被各种玩家占领的格子）
                           //画在另一个地图上（不然每个玩家都要画个图而且代码里面还要分类？）
    public TileBase graytile;    // 指定要填涂的Tile
    public TileBase mountain;
    public TileBase city;
    public TileBase swamp;
    public TileBase capital;
    public GameObject numcanvas;
    public GameObject mapmask;
    public readonly SyncList<Vector3Int> tilePositions = new SyncList<Vector3Int>();
    public readonly SyncList<Vector3Int> playerPos = new SyncList<Vector3Int>();
    public readonly SyncList<Vector3Int> cityPos = new SyncList<Vector3Int>();
    public readonly SyncList<Vector3Int> swampPos = new SyncList<Vector3Int>();
    public readonly SyncList<Vector3Int> mountainPos = new SyncList<Vector3Int>();
    // 一次性的availablepos用于分配
    public List<Vector3Int> AvailablePos = new List<Vector3Int>();
    public List<Vector3Int> AvailablePosBackup = new List<Vector3Int>();
    [SyncVar] public bool randomized = false; //是否已经随机分配过
    public List<bool> allready = new List<bool>();
    private bool iswaiting = false;
    private bool isclear = false;
    private GameObject chooseidtext;
    private GameObject waitingtext;
    [SyncVar] public int border_length;
    [SyncVar] public int playernum;

    private Button mapready;

    // 在运行时填涂Tile
    public void BuildTile(Vector3Int position, TileBase tile)
    {
        // 将位置添加到tilePositions列表中
        tilePositions.Add(position);
        PaintTile(position, tile);
    }
    public void AdjustCameraHeight()
    {
        Debug.Log("adjust");
        Camera minicamera = GameObject.Find("minimapcamera").GetComponent<Camera>();
        // 根据地图高度和摄像机与中心的距离计算摄像机的高度
        // 使用相似三角形原理，摄像机的高度 = 地图高度的一半 / 摄像机与地图中心的水平距离 * 摄像机的视野角度的一半
        // 注意：这里假设摄像机视野角度是垂直方向，且地图中心在水平面上
        float cameraHeight = minicamera.transform.position.z;
        int border_length = GameObject.Find("object(Clone)").GetComponent<Buildmap>().border_length;
        int max_j = border_length;
        minicamera.fieldOfView = Mathf.Atan(10 * max_j / Mathf.Abs(cameraHeight)) * 2f * Mathf.Rad2Deg;
    }

    // 在运行时填涂Tile，可以安全覆盖颜色
    public void PaintTile(Vector3Int position, TileBase tile)
    {
        if (tilemap && tile)
            tilemap.SetTile(position, tile);
    }
    public void Painticon(Vector3Int position, TileBase tile)
    {
        if (Iconmap && tile)
            Iconmap.SetTile(position, tile);
    }


    void Start()
    {
        chooseidtext = GameObject.Find("chooseid");
        waitingtext = GameObject.Find("waiting");
        waitingtext.SetActive(false);
        chooseidtext.SetActive(true);
        if (isServer)
        {
            if (NetworkClient.active && NetworkServer.active)
                waitingtext.GetComponent<Text>().text = "Please Build Map";
            playernum = (int)GameObject.Find("gamedata").GetComponent<plslider>().slider.value;
            for (int i = 0; i < playernum; i++)
            {
                allready.Add(false);
            }
            startbutton();
        }
        anim = GameObject.Find("image").GetComponent<Animator>();
        numcanvas.SetActive(false);
        mapmask.SetActive(false);
    }
    [Command(requiresAuthority = false)]
    private void getready(int index)
    {
        allready[index] = true;
        for (int i = 0; i < allready.Count; i++)
        {
            if (allready[i] == false) return;
        }
        RpcClientStart();
    }
    [Command(requiresAuthority = false)]
    public void CmdServerClear(int localindex)
    {
        RpcClear(localindex);
    }
    [ClientRpc]
    public void RpcClear(int otherindex)
    {
        GameObject.Find("player" + otherindex).SetActive(false);
    }

    public void startbutton()
    {
        CmdStartmap();
        waitingtext.GetComponent<Text>().text = "Waiting For Clients";
    }
    void Update()
    {
        if (isClient)
        {
            if (index < 0) return;
            waitingtext.SetActive(true);
            if (!isclear) { CmdServerClear(index); isclear = true; }
            if (!iswaiting && tilePositions.Count != 0)
            {
                iswaiting = true;
                getready(index);
            }
        }
    }




    [Command(requiresAuthority = false)]
    public void CmdStartmap()
    {

        Slider slider = GameObject.Find("gamedata").GetComponent<Mapsizeslider>().slider;
        border_length = (int)slider.value;
        int max_j = border_length;

        if (border_length % 2 == 0)
        {
            //先填偶数
            for (int i = 0; i < border_length; i += 2)
            {
                max_j--;
                for (int j = -1 - max_j; j < max_j; j++)
                {
                    if (i == 0) { BuildTile(new Vector3Int(j, i, 0), graytile); continue; }
                    BuildTile(new Vector3Int(j, i, 0), graytile);
                    BuildTile(new Vector3Int(j, -i, 0), graytile);
                }
            }
            //再填奇数列
            max_j = border_length - 1;
            for (int i = 1; i < border_length; i += 2)
            {
                max_j--;
                for (int j = -2 - max_j; j < max_j; j++)
                {
                    BuildTile(new Vector3Int(j, i, 0), graytile);
                    BuildTile(new Vector3Int(j, -i, 0), graytile);
                }
            }
        }
        else
        {
            for (int i = 1; i < border_length; i += 2)
            {
                for (int j = 2 - max_j; j < max_j; j++)
                {

                    BuildTile(new Vector3Int(j, i + 1, 0), graytile);
                    BuildTile(new Vector3Int(j, -i + 1, 0), graytile);
                }
                max_j--;

            }
            //再填奇数列
            max_j = border_length;
            BuildTile(new Vector3Int(0, 1, 0), graytile);
            for (int j = 1; j < max_j; j++)
            {
                BuildTile(new Vector3Int(j, 1, 0), graytile);
                BuildTile(new Vector3Int(-j, 1, 0), graytile);
            }
            max_j--;
            for (int i = 2; i < border_length; i += 2)
            {
                for (int j = 0; j < max_j; j++)
                {
                    if (j == 0)
                    {
                        BuildTile(new Vector3Int(j, i + 1, 0), graytile);
                        BuildTile(new Vector3Int(j, -i + 1, 0), graytile);
                        continue;
                    }
                    BuildTile(new Vector3Int(-j, -i + 1, 0), graytile);
                    BuildTile(new Vector3Int(-j, i + 1, 0), graytile);
                    BuildTile(new Vector3Int(j, i + 1, 0), graytile);
                    BuildTile(new Vector3Int(j, -i + 1, 0), graytile);
                }
                max_j--;
            }
        }
        RandomMap();
    }

    [ClientRpc]
    public void RpcClientStart()
    {
        foreach (var tile_pos in tilePositions)
        {
            PaintTile(tile_pos, graytile);
        }
        foreach (Vector3Int mountain_pos in mountainPos)
        {
            PaintTile(mountain_pos, mountain);
        }
        foreach (Vector3Int city_pos in cityPos)
        {
            Painticon(city_pos, city);
        }
        foreach (Vector3Int player_pos in playerPos)
        {
            Painticon(player_pos, capital);
        }
        foreach (Vector3Int swamp_pos in swampPos)
        {
            Painticon(swamp_pos, swamp);
        }
        Aftergen();
    }


    [Command(requiresAuthority = false)]
    public void ServerPaint(Vector3Int pos, string tilename)
    {
        RpcClientPaint(pos, tilename);
    }
    [ClientRpc]
    public void RpcClientPaint(Vector3Int pos, string tilename)
    {
        TileBase othertile = Resources.Load<TileBase>(tilename);
        tilemap.SetTile(pos, othertile);
    }
    private void Aftergen()
    {
        ServerPaint(playerPos[index], playertile.name);
        Vector3Int playerpos = playerPos[index];
        cam_move(playerpos);
        GameObject.Find("minimapcamera").GetComponent<CameraHeightAdjuster>().enabled = true;
        mapmask.SetActive(true);
        anim.SetBool("fadein", false);
        anim.SetBool("fadeout", true);
        numcanvas.SetActive(true);
        AdjustCameraHeight();
        GameObject.Find("ui Camera").SetActive(false);
        GameObject.Find("object(Clone)").GetComponent<Clickmovearmy>().enabled = true;
    }


    private void cam_move(Vector3Int pos)
    {
        Camera maincam = GameObject.Find("mapCamera").GetComponent<Camera>();
        Vector3 worldpos = tilemap.CellToWorld(pos);
        if (worldpos != null)
        {
            GameObject.Find("mapCamera").GetComponent<Mainview>().newPosition =
                               new Vector3(worldpos.x, worldpos.y, maincam.transform.position.z);
            GameObject.Find("mapCamera").GetComponent<Mainview>().targetatplayer();
        }
    }
    void RandomMap()
    {
        System.Random RD = new System.Random();
        int random_seed;
        // 遍历所有瓦片位置
        foreach (Vector3Int pos in tilePositions)
        {
            // 生成随机数种子
            random_seed = RD.Next(0, 100);
            // probabilities may be changed
            // 如果随机数小于15，则将瓦片涂成山脉
            if (random_seed < 15)
            {
                PaintTile(pos, mountain);
                mountainPos.Add(pos);
            }
            // 如果随机数小于22，则将瓦片涂成城市，并将城市位置添加到列表中
            else if (random_seed < 22)
            {
                Painticon(pos, city);
                cityPos.Add(pos);
            }
            // 如果随机数小于28，则将瓦片涂成沼泽
            else if (random_seed < 28)
            {
                Painticon(pos, swamp);
                swampPos.Add(pos);
            }
            // capital候选
            else
            {
                AvailablePos.Add(pos);
                AvailablePosBackup.Add(pos);
            }
        }
        // generate capital, which cannot be adjacent to each other
        for (int i = 0; i < playernum; i++)
        {
            random_seed = RD.Next(0, 99999);
            if (AvailablePos.Count == 0)
            {
                Debug.Log("no available pos, retrying ... ");
                for (; i < playernum; i++)
                {
                    random_seed = RD.Next(0, 99999);
                    if (AvailablePosBackup.Count == 0)
                    {
                        Debug.Log("map size too small, exiting ... ");
                        throw new System.Exception("Retried. No available position");
                    }
                    Vector3Int pos0 = AvailablePosBackup[random_seed % AvailablePosBackup.Count];
                    playerPos.Add(pos0);
                    AvailablePosBackup.RemoveAt(random_seed % AvailablePosBackup.Count);
                }
                randomized = true;
                return;
            }
            Vector3Int pos = AvailablePos[random_seed % AvailablePos.Count];
            playerPos.Add(pos);
            AvailablePos.RemoveAt(random_seed % AvailablePos.Count);
            AvailablePosBackup.RemoveAt(random_seed % AvailablePos.Count);
            RemoveAdjacent(pos);
        }
        randomized = true;
    }
    void RemoveAdjacent(Vector3Int pos)
    {
        AvailablePos.Remove(pos + new Vector3Int(1, 0, 0));
        AvailablePos.Remove(pos + new Vector3Int(-1, 0, 0));
        AvailablePos.Remove(pos + new Vector3Int(0, 1, 0));
        AvailablePos.Remove(pos + new Vector3Int(0, -1, 0));
        if (pos.y % 2 == 0)
        {
            AvailablePos.Remove(pos + new Vector3Int(-1, 1, 0));
            AvailablePos.Remove(pos + new Vector3Int(-1, -1, 0));
        }
        else
        {
            AvailablePos.Remove(pos + new Vector3Int(1, 1, 0));
            AvailablePos.Remove(pos + new Vector3Int(1, -1, 0));
        }
    }
}
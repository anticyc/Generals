using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TilemapClickHandler : MonoBehaviour
{
    public Tilemap shademap;
    public Tilemap tilemap;
    public TileBase shade;
    public TileBase mountain;
    private Camera maincamera;
    private Vector3Int pre_pos; //last shade
    public Vector3Int topos;

    void Start()
    {
        maincamera = GameObject.Find("mapCamera").GetComponent<Camera>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            KeyBoardProcessor(pre_pos, 4); //左下
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            KeyBoardProcessor(pre_pos, 6);//右下
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            KeyBoardProcessor(pre_pos, 2); //上
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            KeyBoardProcessor(pre_pos, 5); //下
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            KeyBoardProcessor(pre_pos, 1);//左上
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            KeyBoardProcessor(pre_pos, 3);//右上
        }
        // 检测鼠标左键点击
        else if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = maincamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 87.371f));
            // 将世界坐标转换为瓦片坐标
            Vector3Int tilePos = shademap.WorldToCell(worldPoint);
            shademap.SetTile(pre_pos, null);
            if (tilemap.HasTile(tilePos) && tilemap.GetTile<TileBase>(tilePos) != mountain)//在地图内    
            {
                shademap.SetTile(tilePos, shade);

            }
            pre_pos = tilePos;
        }
    }

    void KeyBoardProcessor(Vector3Int tilePos, int direction)
    {
        shademap.SetTile(pre_pos, null);
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
            shademap.SetTile(new_pos, shade);
        }
        pre_pos = new_pos;
    }
}

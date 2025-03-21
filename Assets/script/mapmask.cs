// using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;
using Mirror;

public class Mapmask : NetworkBehaviour
{
    const int INT_MAX_VALUE = 0x7FFFFFFF;
    public Tilemap tilemap;  // 指定要操作的Tilemap
    // public TileBase graytile;    // 指定要填涂的Tile
    public TileBase blind_mask;    // 指定要填涂的Tile
    public TileBase local_player_tile; // 本地玩家的Tile
    private Tilemap maskmap;
    private int RemaskTurn = INT_MAX_VALUE;
    private List<Vector3Int> RemaskPosList = new List<Vector3Int>();

    public void PaintTile(Vector3Int position, TileBase tile)
    {
        if (maskmap && tile)
        {
            maskmap.SetTile(position, tile);
        }
    }
    void Start()
    {
        maskmap = GameObject.Find("mask").GetComponent<Tilemap>();
        local_player_tile = GameObject.Find("object(Clone)").GetComponent<Buildmap>().playertile;
        buildmask();
        int index = GameObject.Find("object(Clone)").GetComponent<Buildmap>().index;
        Vector3Int playerpos = GameObject.Find("object(Clone)").GetComponent<Buildmap>().playerPos[index];
        cancelaround(playerpos);
    }
    void Update()
    {
        // lighthouse
        int Turn = GameObject.Find("numcanvas").GetComponent<Numtroops>().TURN;
        if (Turn >= RemaskTurn)
        {
            foreach (Vector3Int pos in RemaskPosList)
            {
                if (pos == RemaskPosList[0]) { maskmap.SetTile(pos, null); continue; }//把灯塔的tile先去掉
                Remask(pos);
            }
            RemaskPosList.Clear();
            RemaskTurn = INT_MAX_VALUE;
        }

    }
    void buildmask()
    {
        int border_length = GameObject.Find("object(Clone)").GetComponent<Buildmap>().border_length;
        int max_j = border_length;

        if (border_length % 2 == 0)
        {
            //先填偶数列
            for (int i = 0; i < border_length; i += 2)
            {
                max_j--;
                for (int j = -1 - max_j; j < max_j; j++)
                {
                    if (i == 0) { PaintTile(new Vector3Int(j, i, 0), blind_mask); continue; }
                    PaintTile(new Vector3Int(j, i, 0), blind_mask);
                    PaintTile(new Vector3Int(j, -i, 0), blind_mask);
                }
            }

            //再填奇数列
            max_j = border_length - 1;
            for (int i = 1; i < border_length; i += 2)
            {
                max_j--;
                for (int j = -2 - max_j; j < max_j; j++)
                {
                    PaintTile(new Vector3Int(j, i, 0), blind_mask);
                    PaintTile(new Vector3Int(j, -i, 0), blind_mask);
                }
            }
        }
        else
        {
            for (int i = 1; i < border_length; i += 2)
            {
                for (int j = 2 - max_j; j < max_j; j++)
                {
                    PaintTile(new Vector3Int(j, i + 1, 0), blind_mask);
                    PaintTile(new Vector3Int(j, -i + 1, 0), blind_mask);
                }
                max_j--;
            }
            //再填奇数列
            max_j = border_length;
            for (int i = 0; i < border_length; i += 2)
            {
                for (int j = 0; j < max_j; j++)
                {
                    PaintTile(new Vector3Int(-j, -i + 1, 0), blind_mask);
                    PaintTile(new Vector3Int(-j, i + 1, 0), blind_mask);
                    PaintTile(new Vector3Int(j, i + 1, 0), blind_mask);
                    PaintTile(new Vector3Int(j, -i + 1, 0), blind_mask);
                }
                max_j--;
            }
        }
    }
    public void cancel(Vector3Int playerpos, int turn)
    {
        maskmap.SetTile(playerpos, null);
        RemaskPosList.Add(playerpos);
        RemaskTurn = turn + 50; //50轮后remask
    }
    public void Remask(Vector3Int playerpos) // single position
    {
        if (tilemap.GetTile(playerpos) == local_player_tile)
            return;
        //确保这里和周围六格都不是玩家的才可以删除
        if (playerpos.y % 2 == 0)//需要奇偶分类
        {
            if (tilemap.GetTile(playerpos + new Vector3Int(0, 1, 0)) == local_player_tile)
                return;
            if (tilemap.GetTile(playerpos + new Vector3Int(-1, 1, 0)) == local_player_tile)
                return;
            if (tilemap.GetTile(playerpos + new Vector3Int(-1, 0, 0)) == local_player_tile)
                return;
            if (tilemap.GetTile(playerpos + new Vector3Int(1, 0, 0)) == local_player_tile)
                return;
            if (tilemap.GetTile(playerpos + new Vector3Int(-1, -1, 0)) == local_player_tile)
                return;
            if (tilemap.GetTile(playerpos + new Vector3Int(0, -1, 0)) == local_player_tile)
                return;
        }
        else
        {
            if (tilemap.GetTile(playerpos + new Vector3Int(0, 1, 0)) == local_player_tile)
                return;
            if (tilemap.GetTile(playerpos + new Vector3Int(1, 1, 0)) == local_player_tile)
                return;
            if (tilemap.GetTile(playerpos + new Vector3Int(1, 0, 0)) == local_player_tile)
                return;
            if (tilemap.GetTile(playerpos + new Vector3Int(-1, 0, 0)) == local_player_tile)
                return;
            if (tilemap.GetTile(playerpos + new Vector3Int(1, -1, 0)) == local_player_tile)
                return;
            if (tilemap.GetTile(playerpos + new Vector3Int(0, -1, 0)) == local_player_tile)
                return;
        }
        maskmap.SetTile(playerpos, blind_mask);
    }
    public void cancelaround(Vector3Int playerpos)
    {
        maskmap.SetTile(playerpos, null);
        if (playerpos.y % 2 == 0)//需要奇偶分类
        {
            //右上方？
            playerpos.y++;
            if (maskmap.GetTile<TileBase>(playerpos) == blind_mask) maskmap.SetTile(playerpos, null);
            playerpos.y--;
            //右下？
            playerpos.x--;
            playerpos.y++;
            if (maskmap.GetTile<TileBase>(playerpos) == blind_mask) maskmap.SetTile(playerpos, null);
            playerpos.y--;
            playerpos.x++;
            //上
            playerpos.x++;
            if (maskmap.GetTile<TileBase>(playerpos) == blind_mask) maskmap.SetTile(playerpos, null);
            playerpos.x--;
            //下
            playerpos.x--;
            if (maskmap.GetTile<TileBase>(playerpos) == blind_mask) maskmap.SetTile(playerpos, null);
            playerpos.x++;
            //
            //左上方？
            playerpos.y--;
            if (maskmap.GetTile<TileBase>(playerpos) == blind_mask) maskmap.SetTile(playerpos, null);
            playerpos.y++;
            //左下？
            playerpos.x--;
            playerpos.y--;
            if (maskmap.GetTile<TileBase>(playerpos) == blind_mask) maskmap.SetTile(playerpos, null);
        }
        else if (Math.Abs(playerpos.y) % 2 == 1)
        {
            //右上方？
            playerpos.y++;
            if (maskmap.GetTile<TileBase>(playerpos) == blind_mask) maskmap.SetTile(playerpos, null);
            playerpos.y--;
            //右下？
            playerpos.x++;
            playerpos.y++;
            if (maskmap.GetTile<TileBase>(playerpos) == blind_mask) maskmap.SetTile(playerpos, null);
            playerpos.y--;
            playerpos.x--;
            //上
            playerpos.x++;
            if (maskmap.GetTile<TileBase>(playerpos) == blind_mask) maskmap.SetTile(playerpos, null);
            playerpos.x--;
            //下
            playerpos.x--;
            if (maskmap.GetTile<TileBase>(playerpos) == blind_mask) maskmap.SetTile(playerpos, null);
            playerpos.x++;
            //
            //左上方？
            playerpos.y--;
            if (maskmap.GetTile<TileBase>(playerpos) == blind_mask) maskmap.SetTile(playerpos, null);
            playerpos.y++;
            //左下？
            playerpos.x++;
            playerpos.y--;
            if (maskmap.GetTile<TileBase>(playerpos) == blind_mask) maskmap.SetTile(playerpos, null);
        }
    }
    public void RemaskAround(Vector3Int playerpos) // single position
    {
        Remask(playerpos);
        if (playerpos.y % 2 == 0)//需要奇偶分类
        {
            if (tilemap.GetTile(playerpos + new Vector3Int(0, 1, 0)) != local_player_tile)
                Remask(playerpos + new Vector3Int(0, 1, 0));
            if (tilemap.GetTile(playerpos + new Vector3Int(-1, 1, 0)) != local_player_tile)
                Remask(playerpos + new Vector3Int(-1, 1, 0));
            if (tilemap.GetTile(playerpos + new Vector3Int(-1, 0, 0)) != local_player_tile)
                Remask(playerpos + new Vector3Int(-1, 0, 0));
            if (tilemap.GetTile(playerpos + new Vector3Int(1, 0, 0)) != local_player_tile)
                Remask(playerpos + new Vector3Int(1, 0, 0));
            if (tilemap.GetTile(playerpos + new Vector3Int(-1, -1, 0)) != local_player_tile)
                Remask(playerpos + new Vector3Int(-1, -1, 0));
            if (tilemap.GetTile(playerpos + new Vector3Int(0, -1, 0)) != local_player_tile)
                Remask(playerpos + new Vector3Int(0, -1, 0));
        }
        else
        {
            if (tilemap.GetTile(playerpos + new Vector3Int(0, 1, 0)) != local_player_tile)
                Remask(playerpos + new Vector3Int(0, 1, 0));
            if (tilemap.GetTile(playerpos + new Vector3Int(1, 1, 0)) != local_player_tile)
                Remask(playerpos + new Vector3Int(1, 1, 0));
            if (tilemap.GetTile(playerpos + new Vector3Int(1, 0, 0)) != local_player_tile)
                Remask(playerpos + new Vector3Int(1, 0, 0));
            if (tilemap.GetTile(playerpos + new Vector3Int(-1, 0, 0)) != local_player_tile)
                Remask(playerpos + new Vector3Int(-1, 0, 0));
            if (tilemap.GetTile(playerpos + new Vector3Int(1, -1, 0)) != local_player_tile)
                Remask(playerpos + new Vector3Int(1, -1, 0));
            if (tilemap.GetTile(playerpos + new Vector3Int(0, -1, 0)) != local_player_tile)
                Remask(playerpos + new Vector3Int(0, -1, 0));
        }
        // Remasked here
    }
}
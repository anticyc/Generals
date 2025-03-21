using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Mirror;

public class Numtroops : NetworkBehaviour
{
    public Tilemap icon;
    public TileBase city;
    public Tilemap map;
    public TileBase gray;
    public TileBase capital;
    public TileBase mountain;
    public TileBase swamp;
    public Font font;
    public List<GameObject> numtext = new List<GameObject>();
    public List<GameObject> capt_num = new List<GameObject>();
    public List<GameObject> city_num = new List<GameObject>();
    public List<GameObject> swamp_num = new List<GameObject>();
    public float repeatRate;
    public int TURN;
    public GameObject cellprefab;

    // Start is called before the first frame update
    void Start()
    {
        TURN = 0;
        SyncList<Vector3Int> pos = GameObject.Find("object(Clone)").GetComponent<Buildmap>().tilePositions;
        foreach (Vector3Int tilepos in pos)
        {
            if (map.GetTile<TileBase>(tilepos) != mountain)
            {

                string name = "cell" + tilepos.ToString();
                GameObject cell = Instantiate(cellprefab, null);
                cell.name = name;
                // 添加Text组件
                Text num = cell.GetComponent<Text>();
                num.fontSize = 30;
                num.font = font;
                RectTransform rectTransform = cell.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(50, 50);
                rectTransform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
                num.text = "";
                num.alignment = TextAnchor.MiddleCenter;

                // 还需要设置垂直居中，确保Text组件所在的RectTransform的pivot设置为0.5, 0.5
                // 这样文本会在RectTransform的中心位置
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                num.transform.SetParent(GameObject.Find("numcanvas").transform, false);
                if (icon.GetTile<TileBase>(tilepos) != capital && icon.GetTile<TileBase>(tilepos) != city && icon.GetTile<TileBase>(tilepos) != swamp)
                    numtext.Add(cell);
                else
                {
                    if (icon.GetTile<TileBase>(tilepos) == capital)
                    {
                        capt_num.Add(cell);
                    }
                    else if (icon.GetTile<TileBase>(tilepos) == city)
                    {
                        num.text += "40"; //cell 视情况动态添加
                        city_num.Add(cell);
                    }
                    else if (icon.GetTile<TileBase>(tilepos) == swamp)
                    {
                        swamp_num.Add(cell);
                    }
                }
                num.transform.position = icon.CellToWorld(tilepos);
            }

        }
        // 开始重复调用UpdateFunction，首次调用在repeatRate秒后，之后每隔repeatRate秒调用一次
        if (isServer)
        {
            InvokeRepeating("UpdateFunction", 25 * repeatRate, 25 * repeatRate);
            InvokeRepeating("capitalupdate", repeatRate, repeatRate);
        }
    }
    [ClientRpc]
    void UpdateFunction()
    {
        // 在这里写你想要定期执行的代码
        foreach (GameObject num in numtext)
        {
            num_update(num);
        }
        foreach (GameObject num in swamp_num)
        {
            Vector3Int tilepos = icon.WorldToCell(num.transform.position);
            // 如果该瓦片不是灰色
            if (map.GetTile<TileBase>(tilepos) != gray)
            {
                Text temp = num.GetComponent<Text>();
                if (temp.text != "1")
                {
                    int troopnum = int.Parse(temp.text);
                    troopnum--;
                    temp.text = "" + troopnum;
                }
                else
                {
                    temp.text = "";
                    map.SetTile(tilepos, gray);
                }
            }
        }
    }
    [ClientRpc]
    void capitalupdate()
    {
        TURN++;
        foreach (GameObject num in capt_num)
        {
            num_update(num);
        }
        foreach (GameObject num in city_num)
        {
            // 必须有人涉足
            num_update(num);
        }
    }
    void num_update(GameObject num)
    {
        // 将物体的世界坐标转换为瓦片坐标
        Vector3Int tilepos = icon.WorldToCell(num.transform.position);
        // 如果该瓦片不是灰色
        if (map.GetTile<TileBase>(tilepos) != gray)
        {
            Text temp = num.GetComponent<Text>();
            if (temp.text != "")
            {
                int troopnum = int.Parse(temp.text);
                troopnum++;
                temp.text = "" + troopnum;
            }
            else temp.text = "1";
        }
    }
}

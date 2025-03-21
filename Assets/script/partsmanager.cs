using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Telepathy;

public class partsmanager : MonoBehaviour
{
    private ToggleGroup toggleGroup;
    public Button button;
    public Image tiledemo;
    public Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        button.gameObject.SetActive(false);
        toggle.isOn = false;
        button.onClick.AddListener(OnClick);
        toggleGroup = GameObject.Find("id_tile_list").GetComponent<ToggleGroup>();
        toggle.group = toggleGroup;
    }

    // Update is called once per frame
    void Update()
    {
        if (toggle.isOn)
        {
            button.gameObject.SetActive(true);
        }
        else
        {
            button.gameObject.SetActive(false);
        }
    }
    public void OnClick()
    {
        string name = button.name;
        string numberPart = string.Empty;
        foreach (char c in name)
        {
            if (char.IsDigit(c))
            {
                numberPart += c;
            }
        }
        string tilename = tiledemo.sprite.name;
        int index = int.Parse(numberPart);
        int playernum = GameObject.Find("basicmapgenerator").GetComponent<basicmapgenerator>().playernum;
        GameObject.Find("object(Clone)").GetComponent<Buildmap>().playertile = Resources.Load<Tile>(tilename);
        GameObject.Find("chooseid").SetActive(false);
        GameObject.Find("object(Clone)").GetComponent<Buildmap>().index = index;
    }
}

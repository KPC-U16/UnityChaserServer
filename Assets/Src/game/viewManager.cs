using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.UI;

public class viewManager : MonoBehaviour
{
    public GameObject tileObject = null;
    public Text turnText = null;
    GameObject[,] viewObjList;

    Sprite[] texture;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Act(int[,] diff)
    {
        if (diff == null) return;

        for (int a=0;a<diff.GetLength(0);a++)
        {
            string type;
            type = "none";
            if (diff[a,2] == 2) type = "block";
            if (diff[a,2] == 3) type = "item";
            if (diff[a,2] == 4) type = "cool";
            if (diff[a,2] == 5) type = "hot";

            viewObjList[diff[a,0],diff[a,1]].GetComponent<tileImg>().ImgChange(type,texture);
        }
    }

    public void SetView(string cName,string hName,int turn,int[,] map,Sprite[] tex)
    {
        texture = new Sprite[tex.Length];
        Array.Copy(tex,texture,tex.Length);
        ShowMap(map);
        SetTurn(turn);
        ShowName(cName,hName);
    }

    public void SetTurn(int turn)
    {
        turnText.text = turn.ToString();
    }

    void ShowName(string cName,string hName)
    {
        
    }

    void ShowMap(int[,] map)
    {
        int yMax = map.GetLength(0);
        int xMax = map.GetLength(1);

        int xSize;
        int ySize = 30;


        if (xMax == 15)
        {
            xSize = 56;
        }
        else
        {
            xSize = 40;
        }

        viewObjList = new GameObject[xMax,yMax];

        for (int y=0;y<yMax;y++)
        {
            for (int x=0;x<xMax;x++)
            {
                GameObject prefab = Instantiate(tileObject);
                prefab.transform.SetParent(gameObject.transform);
                prefab.transform.localScale = new Vector3(xSize,ySize,0);
                prefab.transform.localPosition = new Vector3(x*xSize,y*ySize,0);

                string type;
                type = "none";
                if (map[yMax-1-y,x] == 2) type = "block";
                if (map[yMax-1-y,x] == 3) type = "item";
                if (map[yMax-1-y,x] == 4) type = "cool";
                if (map[yMax-1-y,x] == 5) type = "hot";
                prefab.GetComponent<tileImg>().ImgChange(type,texture);
                int xBuf = x;
                int yBuf = y;

                viewObjList[x,yMax-1-y] = prefab;
            }
        }
    }
}

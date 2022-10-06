using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.UI;

public class viewManager : MonoBehaviour
{
    public GameObject tileObject = null;
    public GameObject characterObject = null;
    public Text turnText = null;
    GameObject[,] viewObjList;

    GameObject cool;
    GameObject hot;

    charMove coolSrc;
    charMove hotSrc;

    Sprite[] texture;
    RuntimeAnimatorController[] animators = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Act(int[,] diff,string command)
    {
        if (diff == null) return;

        for (int a=0;a<diff.GetLength(0);a++)
        {
            string type;

            type = "none";
            if (diff[a,2] == 2) type = "block";
            if (diff[a,2] == 4) 
            {
                //coolの移動モーション関数を呼ぶ
                coolSrc.Act(command);
                cool.transform.SetParent(viewObjList[diff[a,0],diff[a,1]].transform);
                cool.transform.localPosition = new Vector3(0,0,0);
            }
            if (diff[a,2] == 5)
            {
                //hotの移動モーション関数を呼ぶ
                hotSrc.Act(command);
                hot.transform.SetParent(viewObjList[diff[a,0],diff[a,1]].transform);
                hot.transform.localPosition = new Vector3(0,0,0);
            }

            viewObjList[diff[a,0],diff[a,1]].GetComponent<tileImg>().ImgChange(type);
        }
    }

    public void SetView(string cName,string hName,int turn,int[,] map,Sprite[] tex,RuntimeAnimatorController[] anim)
    {
        texture = new Sprite[tex.Length];
        Array.Copy(tex,texture,tex.Length);
        if (anim != null)
        {
            animators = new RuntimeAnimatorController[anim.Length];
            Array.Copy(anim,animators,anim.Length);
        }
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
        int[] coolPos = new int[2];
        int[] hotPos = new int[2];

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
                if (map[yMax-1-y,x] == 4)
                {
                    coolPos[0] = x;
                    coolPos[1] = yMax-1-y;
                }
                if (map[yMax-1-y,x] == 5)
                {
                    hotPos[0] = x;
                    hotPos[1] = yMax-1-y;
                }
                tileImg tile = prefab.GetComponent<tileImg>();
                tile.SetView(animators,texture);
                tile.ImgChange(type);
                int xBuf = x;
                int yBuf = y;

                viewObjList[x,yMax-1-y] = prefab;
            }
        }

        //cool,hotを生成しposの位置のオブジェクトの子にする
        cool = Instantiate(characterObject);
        cool.transform.SetParent(viewObjList[coolPos[0],coolPos[1]].transform);
        cool.transform.localScale = new Vector3(1,1,0);
        cool.transform.localPosition = new Vector3(0,0,0);
        coolSrc = cool.GetComponent<charMove>();
        coolSrc.Set(texture[3],animators[1],xSize,ySize);

        hot = Instantiate(characterObject);
        hot.transform.SetParent(viewObjList[hotPos[0],hotPos[1]].transform);
        hot.transform.localScale = new Vector3(1,1,0);
        hot.transform.localPosition = new Vector3(0,0,0);
        hotSrc = hot.GetComponent<charMove>();
        hotSrc.Set(texture[4],animators[2],xSize,ySize);
    }
}

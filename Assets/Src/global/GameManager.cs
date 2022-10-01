using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    ClientControl cool;
    ClientControl hot;

    public ShowData coolShowData;
    public ShowData hotShowData;

    public ConnectButton coolConButton;
    public ConnectButton hotConButton;

    public StartButton startButton;

    MapManager mapManager;
    viewManager viewManager;

    public Sprite[] texture;


    private bool gameStarted = false;

    void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async void SceneLoaded(Scene nextScene,LoadSceneMode mode)
    {
        if (nextScene.name == "game-demo")
        {
            viewManager = GameObject.Find("BoardBack").GetComponent<viewManager>();
            viewManager.SetView(cool.name,hot.name,mapManager.getTurn(),mapManager.getMapData(),texture);
            await Action(true);
        }
    }

    public void SetTexture(Sprite[] tex)
    {
        texture = new Sprite[tex.Length];
        Array.Copy(tex,texture,tex.Length);
    }

    public async void ConWait(string team,string port)
    {
        bool success;
        Debug.Log(team);
        Debug.Log(port);

        switch (team)
        {
            case "Cool":
                coolShowData.Reflect("TCP接続待ち状態",null,null);
                cool = new ClientControl(int.Parse(port));
                success = await cool.Ready();
                if (success)
                {
                    coolShowData.Reflect("準備完了",cool.name,cool.getClientIP());
                    startButton.Ready(team);
                }
            break;
            case "Hot":
                hotShowData.Reflect("TCP接続待ち状態",null,null);
                hot = new ClientControl(int.Parse(port));
                success = await hot.Ready();
                if (success)
                {
                    hotShowData.Reflect("準備完了",hot.name,hot.getClientIP());
                    startButton.Ready(team);
                }
            break;
        }

        StartCoroutine(CheckConnected(team));
    }

    public void ConClose(string team)
    {
        Debug.Log("ConClose");
        switch (team)
        {
            case "Cool":
                coolShowData.Reflect("非接続","不明","不明");
                cool.End();
                cool = null;
                startButton.NotReady(team);
            break;
            case "Hot":
                hotShowData.Reflect("非接続","不明","不明");
                hot.End();
                hot = null;
                startButton.NotReady(team);
            break;
        }
        
    }

    IEnumerator CheckConnected(string team)
    {
        while (!gameStarted)
        {
            yield return new WaitForSeconds(1.0f);
            switch (team)
            {
                case "Cool":
                    if (cool == null) yield break;

                    if (!cool.IsConnected())
                    {
                        coolShowData.Reflect("非接続","不明","不明");
                        coolConButton.InitButton();
                        startButton.NotReady(team);
                        yield break;
                    }
                break;
                case "Hot":
                    if (hot == null) yield break;

                    if (!hot.IsConnected())
                    {
                        hotShowData.Reflect("非接続","不明","不明");
                        hotConButton.InitButton();
                        startButton.NotReady(team);
                        yield break;
                    }
                break;
            }
        }
    }

    public void GameStart(string mapPath)
    {
        mapManager = new MapManager();
        mapManager.setMap(mapPath);
        SceneManager.LoadScene("game-demo");
    }

    void GameEnd()
    {
        Debug.Log("game end");
    }

    async Task Action(bool isCool)
    {
        string recieve;
        int[] returnData;
        string team;
        int turn;

        while (true)
        {
            team = isCool ? "Cool" : "Hot";
            Debug.Log(mapManager.getTurn());
            
            switch (team)
            {
                case "Cool":
                    Debug.Log(team);
                    recieve = await cool.Send("@"); //行動開始命令
                    returnData = mapManager.ActChar(team,recieve);
                    recieve = await cool.Send(string.Join("",returnData));
                    returnData = mapManager.ActChar(team,recieve);
                    recieve = await cool.Send(string.Join("",returnData));
                    if (recieve == "#\r\n") Debug.Log("OK");
                    break;
                case "Hot":
                    Debug.Log(team);
                    recieve = await hot.Send("@"); //行動開始命令
                    returnData = mapManager.ActChar(team,recieve);
                    recieve = await hot.Send(string.Join("",returnData));
                    returnData = mapManager.ActChar(team,recieve);
                    recieve = await hot.Send(string.Join("",returnData));
                    if (recieve == "#\r\n") Debug.Log("OK");
                    break;
            }

            await Task.Delay(500);
            viewManager.Act(mapManager.getDifference());
            turn = mapManager.getTurn();
            viewManager.SetTurn(turn);

            if (turn == 0)
            {
                GameEnd();
                break;
            }
            if (!mapManager.getIsContinue())
            {
                GameEnd();
                break;
            }

            isCool = !isCool;
        }

    }
}

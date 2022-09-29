using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    ClientControl cool;
    ClientControl hot;

    public ShowData coolShowData;
    public ShowData hotShowData;

    public ConnectButton coolConButton;
    public ConnectButton hotConButton;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
                }
            break;
            case "Hot":
                hotShowData.Reflect("TCP接続待ち状態",null,null);
                hot = new ClientControl(int.Parse(port));
                success = await hot.Ready();
                if (success)
                {
                    hotShowData.Reflect("準備完了",hot.name,hot.getClientIP());
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
            break;
            case "Hot":
                hotShowData.Reflect("非接続","不明","不明");
                hot.End();
                hot = null;
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

                    Debug.Log(cool.IsConnected());

                    if (!cool.IsConnected())
                    {
                        coolShowData.Reflect("非接続","不明","不明");
                        coolConButton.InitButton();
                        yield break;
                    }
                break;
                case "Hot":
                    if (hot == null) yield break;

                    if (!hot.IsConnected())
                    {
                        hotShowData.Reflect("非接続","不明","不明");
                        coolConButton.InitButton();
                        yield break;
                    }
                break;
            }
        }
    }

    public void GameStart()
    {

    }
}

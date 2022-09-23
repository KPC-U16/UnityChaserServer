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
                cool = new ClientControl(int.Parse(port));
                success = await cool.Ready();
                if (success)
                {
                    coolShowData.Reflect("準備完了",cool.name,cool.getClientIP());
                }
            break;
            case "Hot":
                hot = new ClientControl(int.Parse(port));
                success = await hot.Ready();
                if (success)
                {
                    hotShowData.Reflect("準備完了",hot.name,hot.getClientIP());
                }
            break;
        }
    }

    public void ConClose(string team)
    {
        Debug.Log("ConClose");
        switch (team)
        {
            case "Cool":
                cool.End();
                cool = null;
            break;
            case "Hot":
                hot.End();
                hot = null;
            break;
        }
        
    }

    public void GameStart()
    {

    }
}

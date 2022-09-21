using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    ClientControl cool;
    ClientControl hot;

    string cName;
    string hName;

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
        Debug.Log(team);
        Debug.Log(port);
        switch (team)
        {
            case "cool":
                cool = new ClientControl(int.Parse(port));
                await cool.Ready();
                Debug.Log(cool.name);
            break;
            case "hot":
                hot = new ClientControl(int.Parse(port));
                await hot.Ready();
            break;
        }
    }
}

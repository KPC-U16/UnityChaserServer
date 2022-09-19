using System.Threading;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Hot,Coolの通信を行います
/// </summary>
public class ClientControl
{
    
    SocketControl client;


    public ClientControl(int Port){
        
        //ソケット通信用のクラスを初期化
        this.client = new SocketControl(Port);
    }

    public async Task ClientReady(){
        await client.StartListening();
    }

    public async Task<string> Write(){


        return "";
    }

    public async Task Read(){

    }


}

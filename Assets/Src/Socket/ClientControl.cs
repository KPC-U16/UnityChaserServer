using System.Data;
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

/// <summary>
/// クライアントにサーバーの準備が完了したことを通知する
/// </summary>
/// <returns>Clientの名前<string></returns>
    public async Task<string> Ready(){
        await client.StartListening();
        return await client.ReceptionAsync();
    }

/// <summary>
/// クライアントへ周囲情報,制御情報を送信する
/// </summary>
/// <param name="instruction"></param>
/// <returns>クライアントからの行動指示 or # (ターンの終了) </returns>
    public async Task<string> Send(string instruction){
        await client.SendAsync(instruction+"\r\n");
        return await client.ReceptionAsync();
    }

/// <summary>
/// クライアントとの通信を終了する
/// </summary>
    public void End(){
        client.Close();

    }


}

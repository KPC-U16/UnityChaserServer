using System;
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
    public string name = null;

    public ClientControl(int Port){
        
        //ソケット通信用のクラスを初期化
        this.client = new SocketControl(Port);
    }

/// <summary>
/// クライアントにサーバーの準備が完了したことを通知する
/// </summary>
/// <returns></returns>
    public async Task<bool> Ready(){
        // Readyを実行中は原則すべての例外から復旧可能なので握り潰す 
        try{
            await client.StartListening();
            this.name = await client.ReceptionAsync(-1);
            return true;
        }
        catch{
            
            return false;
        }
    
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
/// 接続してきたクライアントのIPアドレスを返答する
/// </summary>
/// <returns></returns>
    public string getClientIP(){
        return client.GetRemoteIP();
    }

/// <summary>
/// クライアントとソケットが維持できているか返答する
/// </summary>
/// <returns></returns>
    public bool IsConnected(){
        return client.IsConnected();
    }

/// <summary>
/// クライアントとの通信を終了する
/// </summary>
    public void End(){
        client.Close();

    }


}

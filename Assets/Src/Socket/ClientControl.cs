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
    
    //
    SocketControl client;

    //クライアントから行動命令をサーバーが処理中か
    bool continueCommunication = false;

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
/// クライアントへ周囲情報を送信する
/// </summary>
/// <param name="arround"></param>
/// <returns>クライアントからの行動指示 or ""</returns>
    public async Task<string> Send(int[,] arround){
        
        if (continueCommunication){
            await client.SendAsync("@");
            if (await client.ReceptionAsync() != "gr\r\n"){
                //ClientからGetReadyを受信できなかったとき
                throw new ClientMessageFormatException("GetReadyを正常に受信できませんでした");
            }
            await client.SendAsync(string.Join(",",arround));
            string res = await client.ReceptionAsync();
            
            //フラグ反転
            continueCommunication =! continueCommunication;
            return res;
        }
        //行動命令結果を返答する
        else{
            await client.SendAsync(string.Join(",",arround));
            
            //フラグ反転
            continueCommunication =! continueCommunication;
            return "";
        }
    }

/// <summary>
/// クライアントとの通信を終了する
/// </summary>
    public void End(){
        client.Close();

    }


}

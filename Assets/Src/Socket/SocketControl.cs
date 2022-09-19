using System.Globalization;
using System.Net.Security;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Text;

//https://marunaka-blog.com/cshap-tcplistener-create/2293/

    /// <summary>
    /// 実際にクライアントとの通信を行うTCPセッションを管理します
    /// </summary>
public class SocketControl
{

    TcpListener Listener;

    TcpClient Server;

    public SocketControl(int port){
        
        // 接続を待つエンドポイントを設定
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
        this.Listener = new TcpListener(endPoint);

    }

    ~SocketControl(){

        if (this.Server.Connected)
            Server.Close();

    }

public async Task SendAsync(string request){
        byte[] buffer = new byte[1024];

        buffer = Encoding.UTF8.GetBytes(request);

        // クライアントへリクエストを送信する
        using(NetworkStream stream = this.Server.GetStream()){

        await stream.WriteAsync(buffer, 0, buffer.Length);
        
        }
}

public async Task<string>  ReceptionAsync(){
        byte[] buffer = new byte[1024];
        string request = "";

        // クライアントからリクエストを受信する
        using(NetworkStream stream = this.Server.GetStream()){
        do
        {
            int byteSize = await stream.ReadAsync(buffer,0,buffer.Length);
            request += Encoding.UTF8.GetString(buffer, 0, byteSize);
        }
        while(stream.DataAvailable);
        
        }

        return request;
}

    public async Task<bool> StartListening()
    {

        try
        {
            // 待ち受け開始
            this.Listener.Start();

            Console.WriteLine("接続待機中...");

            // 非同期で接続要求を待機
            this.Server = await this.Listener.AcceptTcpClientAsync();
            
            Console.WriteLine("クライアントからの接続要求を受け入れ");

            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return false;
    }

    /*
    public async Task<string> ReceiveAsync()
    {
        byte[] buffer = new byte[1024];
        string request = "";

        try
        {
            using (NetworkStream stream = this.Server.GetStream())
            {
                // クライアントからリクエストを受信する
                do
                {
                    int byteSize = await stream.ReadAsync(buffer, 0, buffer.Length);
                    request += Encoding.UTF8.GetString(buffer, 0, byteSize);
                }
                while (stream.DataAvailable);
                Console.WriteLine($"クライアントから「{request}」を受信");

                // クライアントへレスポンスを送信する
                var response = "OK";
                buffer = Encoding.UTF8.GetBytes(response);

                await stream.WriteAsync(buffer, 0, buffer.Length);
                Console.WriteLine($"クライアントへ「{response}」を送信");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return request;
    }
    */
}

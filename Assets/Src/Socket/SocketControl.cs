using System.Threading;
using System.Net.Security;
using System.IO;
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
    Task<TcpClient> ListenerTask;
    CancellationTokenSource cancellation = new CancellationTokenSource();

    TcpClient Server;

    NetworkStream stream;

    public SocketControl(int port)
    {
        // 接続を待つエンドポイントを設定
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
        this.Listener = new TcpListener(endPoint);
    }

    ~SocketControl()
    {
        if (this.Server.Connected)
            Server.Close();
    }

    /// <summary>
    /// 受け取った文字列をUTF8でバイナリへ変換し非同期でクライアントへ送信する
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task SendAsync(string request, int SOCKET_TIMEOUT = 10000)
    {
        byte[] buffer = new byte[1024];
        buffer = Encoding.UTF8.GetBytes(request);

        // クライアントへリクエストを送信する
        if (this.stream == null)
        {
            this.stream = this.Server.GetStream();
        }
        try
        {
            Task writeTask = this.stream.WriteAsync(buffer, 0, buffer.Length);
            if (await Task.WhenAny(writeTask, Task.Delay(SOCKET_TIMEOUT)) != writeTask)
                // SOCKET_TIMEOUT ミリ秒待って読み取りが終了しなかったら、タイムアウトにする
                throw new SocketException(10060);
        }
        //ソケット通信にエラーが発生したとき
        catch (SocketException e)
        {
            if (e.ErrorCode == 10060)
            {
                throw new NetworkErrorException("通信がタイムアウトしました");
            }
            else
            {
                throw new NetworkErrorException("ソケット通信にエラーが発生しました", e);
            }
        }
        //ソケットにアクセスできないとき
        catch (IOException e)
        {
            throw new NetworkErrorException("ソケットにアクセスできません", e);
        }
        //想定しないエラーの場合
        catch (Exception e)
        {
            throw new NetworkErrorException("不明なネットワークエラーが発生しました", e);
        }
    }


    /// <summary>
    /// 非同期でクライアントから受信したバイナリをUTF8で文字列へ変換する
    /// </summary>
    /// <returns></returns>
    public async Task<string> ReceptionAsync(int SOCKET_TIMEOUT = 10000)
    {
        byte[] buffer = new byte[1024];
        string request = "";
        Task<int> byteSize;

        // クライアントからリクエストを受信する
        if (this.stream == null)
        {
            this.stream = this.Server.GetStream();
        }
        //NetworkStream stream = this.Server.GetStream();
        do
        {
            byteSize = this.stream.ReadAsync(buffer, 0, buffer.Length);

            try
            {
                if (await Task.WhenAny(byteSize, Task.Delay(SOCKET_TIMEOUT)) != byteSize)
                    // SOCKET_TIMEOUT ミリ秒待って読み取りが終了しなかったら、タイムアウトにする
                    throw new SocketException(10060);
            }
            //ソケット通信にエラーが発生したとき
            catch (SocketException e)
            {
                if (e.ErrorCode == 10060)
                {
                    throw new NetworkErrorException("通信がタイムアウトしました");
                }
                else
                {
                    throw new NetworkErrorException("ソケット通信にエラーが発生しました", e);
                }
            }
            //ソケットにアクセスできないとき
            catch (IOException e)
            {
                throw new NetworkErrorException("ソケットにアクセスできません", e);
            }
            //想定しないエラーの場合
            catch (Exception e)
            {
                throw new NetworkErrorException("不明なネットワークエラーが発生しました", e);
            }

            request += Encoding.UTF8.GetString(buffer, 0, await byteSize);

        }
        while (this.stream.DataAvailable);
        return request;
    }


    /// <summary>
    /// クライアントからの接続を受け付け、承認する
    /// </summary>
    /// <returns></returns>
    public async Task<bool> StartListening()
    {
        try
        {
            // 待ち受け開始
            this.Listener.Start();
            // 非同期で接続要求を待機

            //this.Server = await Task.Run(()=> this.Listener.AcceptTcpClientAsync(),cancellation.Token);
            this.Server = await Task.Run(() => this.Listener.AcceptTcpClientAsync(), cancellation.Token);

            //this.Listener.Stop();
            return true;
        }
        //ソケットにエラーが発生したとき
        catch (SocketException e)
        {
            throw new NetworkErrorException("ソケット通信の確立中にエラーが発生しました", e);
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
    }

    /// <summary>
    /// RemoteEndPointからIPを取り出し、返答する
    /// </summary>
    /// <returns></returns>
    public string GetRemoteIP()
    {
        string remoteAddress = this.Server.Client.RemoteEndPoint.ToString();
        return remoteAddress.Split(":")[0];
    }

    /// <summary>
    /// クライアントにポーリングして接続状態を確認する
    /// </summary>
    /// <returns></returns>
    public bool IsConnected()
    {
        return !(this.Server.Client.Poll(1000,SelectMode.SelectRead) && this.Server.Client.Available == 0 || !this.Server.Connected);
    }
    /// <summary>
    /// TCPソケット通信を終了する
    /// </summary>
    public void Close()
    {
        this.cancellation.Cancel();
        if (stream != null)
        {
            this.stream.Close();
        }
        this.Listener.Stop();
        if (this.Server != null && this.Server.Client.Connected == true)
        {
            this.Server.Close();
        }
    }
}

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class ChaserMap
{
    public string name; //mapの名前
    public int turn; //turn数
    public int[] size; //mapのサイズ
    public int[,] data; //mapの実態(0:何もない, 2:壁, 3:アイテム)
    public int[] hotPosition; //hotの位置
    public int[] coolPosition; //coolの位置

    public ChaserMap() //コンストラクタ すべてにnullを入れる
    {
        this.name = null;
        this.turn = 0;
        this.size = new int[2]{0,0};
        this.data = null;
        this.hotPosition = new int[2]{0,0};
        this.coolPosition = new int[2]{0,0};
    }

    public async Task<string> setMap(string path) //.mapファイルからmapクラスに展開する
    {
        int n = 0; //.mapファイルのDの行を何回読んだかの回数
        try
        {
            using(StreamReader sr = new StreamReader(path, Encoding.UTF8)) //StreamReaderを使ってpathのファイルを読み込む
            {
                while(0 <= sr.Peek())
                {
                    n = CreateMap(await sr.ReadLineAsync(), n); //asyncで非同期的に各行ごとに読み込む
                }
            }
            return "success";
        }
        catch(FileNotFoundException e) //ファイルが開けなくて失敗したとき
        {
            return "Error: file read error";
        }
        catch(IndexOutOfRangeException e) //マップのデータがサイズより大きかったとき
        {
            return "Error: index out of range";
        }
        catch(FormatException e) //ターンやサイズ,キャラクターの位置に数字以外が入っていたとき
        {
            return "Error: file format error";
        }
        catch(Exception e) //失敗したとき
        {
            return "Error: " + e.ToString();
        }
    }

    //ここから このクラス内でのみ使われるメソッド
    private int CreateMap(string mapData, int n) //mapDataをもとに変数に値を代入する  //mapDataのバッファ //n:Dの行を何回読んだかの回数
    {
        string[] arrayBuf; //mapDataのバッファをsplitした時のバッファー
        int[] mapColumnBuf; //mapの列のバッファ
        arrayBuf = mapData.Split(':');
        switch(arrayBuf[0])
        {
            case "N": //mapDataがNの列の時
                this.name = arrayBuf[1]; //.mapファイルに書かれてるファイル名をクラス変数のnameに入れる
                break;
            case "T": //mapDataがTの列の時
                this.turn = int.Parse(arrayBuf[1]); //.mapファイルのターン数をクラス変数のturnに入れる
                break;
            case "S": //mapDataがSの列の時
                this.size = Array.ConvertAll(arrayBuf[1].Split(','), int.Parse); //.mapファイルに書かれてるサイズをカンマでsplitしてクラス変数のsizeに入れる(順番: x,y)
                this.data = new int[this.size[1], this.size[0]]; //size配列をもとに2次元配列のmapの大きさを決定してnewする
                break;
            case "D": //mapDataがDの列の時
                mapColumnBuf = Array.ConvertAll(arrayBuf[1].Split(','), int.Parse); //.mapファイルのDの行を1行ずつ取得し、カンマでsplitしてクラス変数のmapColumnBufに入れる
                for(int i = 0; i < this.size[0]; i++) //mapColumnBufの各列を一つずつmapに入れているfor文
                {
                    this.data[n,i] = mapColumnBuf[i]; //2次元のmap変数に1行ずつ取ってきたデータを書き込む処理
                }
                n++; //Dの行を読んだ時nに加算する
                break;
            case "H": //mapDataがHの列の時
                this.hotPosition = Array.ConvertAll(arrayBuf[1].Split(','), int.Parse); //.mapファイルに書かれてるhotの位置をカンマでsplitしてクラス変数のhotPositionに入れる(順番: x,y)
                break;
            case "C": //mapDataがCの列の時
                this.coolPosition = Array.ConvertAll(arrayBuf[1].Split(','), int.Parse); //.mapファイルに書かれてるcoolの位置をカンマでsplitしてクラス変数のcoolPositionに入れる(順番: x,y) 
                break;
        }
        return n;
    }

    //ゲッター系
    public int getItem() //アイテムの総数を返すメソッド
    {
        int itemAmount = 0;

        for(int i = 0; i < this.size[1]; i++)
        {
            for(int j = 0; j < this.size[0]; j++)
            {
                if(this.data[i,j] == 3)
                {
                    itemAmount++;
                }
            }
        }

        return itemAmount;
    }
}
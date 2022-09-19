using System.IO;

public class ChaserMap
{
    public string name; //mapの名前
    public int turn; //turn数
    public int[] size; //mapのサイズ
    public int[,] map; //mapの実態(0:何もない, 2:壁, 3:アイテム)
    public int[] hotPosition; //hotの位置
    public int[] coolPosition; //coolの位置

    public ChaserMap(string path) //ファイルを読み込むときのコンストラクタ
    {
        try
        {
            using(StreamReader sr = new StreamReader(path, Encoding.UTF-8)) //StreamReaderを使ってpathのファイルを読み込む
            {
                while(0 <= sr.Peek())
                {
                    CreateMap(await sr.ReadLineAsync()); //asyncで非同期的に各行ごとに読み込む
                }
            }
        }
        catch(Exception ex) //失敗したとき
        {
            print("file read erro") //仮置きのエラー文
        }
    }

    public ChaserMap(string name, int turn, int[] size) //ファイルを読み込まないときのコンストラクタ
    {
        this.name = name;
        this.turn = turn;
        this.size = size;
        this.map = new int[this.size[1], this.size[0]];
    }

    //ここから このクラス内でのみ使われるメソッド
    private void CreateMap(string mapData) //mapDataをもとに変数に値を代入する  //mapDataのバッファ
    {
        string[] arrayBuf; //mapDataのバッファをsplitした時のバッファー
        int[] mapColumnBuf; //mapの列のバッファ
        this.arrayBuf = mapData.Split(':');
        switch(this.arrayBuf[0])
        {
            case 'N': //mapDataがNの列の時
                this.name = arrayBuf[1]; //.mapファイルに書かれてるファイル名をクラス変数のnameに入れる
                break;
            case 'T': //mapDataがTの列の時
                this.turn = int.parse(arrayBuf[1]); //.mapファイルのターン数をクラス変数のturnに入れる
                break;
            case 'S': //mapDataがSの列の時
                this.size = Array.ConvertAll(arrayBuf[1].Split(','), int.Parse); //.mapファイルに書かれてるサイズをカンマでsplitしてクラス変数のsizeに入れる(順番: x,y)
                this.map = new int[this.size[1], this.size[0]]; //size配列をもとに2次元配列のmapの大きさを決定してnewする
                break;
            case 'D': //mapDataがDの列の時
                this.mapColumnBuf = Array.ConvertAll(arrayBuf[1].Split(','), int.Parse); //.mapファイルのDの行を1行ずつ取得し、カンマでsplitしてクラス変数のmapColumnBufに入れる
                int n = this.map.GetLength(0); //クラス変数のmapに現在存在している行数をnに入れる
                for(int i = 0; i < this.size[0]; i++) //mapColumnBufの各列を一つずつmapに入れているfor文
                {
                    this.map[n,i] = this.mapColumnBuf[i];
                }
                break;

            case 'H': //mapDataがHの列の時
                this.hotPosition = Array.ConvertAll(arrayBuf[1].Split(','), int.Parse); //.mapファイルに書かれてるhotの位置をカンマでsplitしてクラス変数のhotPositionに入れる(順番: x,y)
                break;
            case 'C': //mapDataがCの列の時
                this.coolPosition = Array.ConvertAll(arrayBuf[1].Split(','), int.Parse); //.mapファイルに書かれてるcoolの位置をカンマでsplitしてクラス変数のcoolPositionに入れる(順番: x,y) 
                break;

        }
    }
}
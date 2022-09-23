using System;

public class MapManager
{
    ChaserMap map = new ChaserMap(); //mapにChaserMap型をnewしてインスタンスを作る

    //mapのset系
    public void setMap(string path)
    {
        map.setMap(path);
    }

    public void setCustomMap(string path, int[] size) //マップ編集時に呼び出される
    {
        if(path) //引数にpathを渡されたら.mapファイルを開いて展開する
        {
            setMap(path);
        }
        else //pathが渡されなかった場合sizeをもとにランダムマップを生成する
        {
            this.setRandomMap(size);
        }
    }

    public void setRandomMap(int[] size)
    {
        if(!size)
        {
            Array.Copy([15,17], size, 2);
        }
        Array.Copy(size, map.size, 2);
        map.data = new int[size[1], size[0]];

    }

    public int[,] getMapData()
    {
        return map.data;
    }

    public int[] getTurn()
    {
        return map.turn;
    }
}
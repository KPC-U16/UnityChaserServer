using System;

public class MapManager
{
    ChaserMap map = new ChaserMap(); //mapにChaserMap型をnewしてインスタンスを作る
    int hotScore = 0; //ホットのスコアを持つ
    int coolScore = 0; //クールのスコアを持つ
    string[,] difference; //mapの直近のデータを持つ
    bool isContinue = true; //ゲームの継続判定
    /*
    mapの数字の解説
    0:通路
    1:キャラクター(敵)
    2:壁
    3:アイテム
    4:cool
    5:hot
    6:壁に埋まったcool
    7:壁に埋まったhot
    */

    //mapのset系
    public void setMap(string path) //mapファイルを用いてmapを生成するメソッド
    {
        map.setMap(path);
    }

    public void setCustomMap(string path, int[] size) //マップ編集時に呼び出される
    {
        if(path) //引数にpathを渡されたら.mapファイルを開いて展開する
        {
            map.setMap(path);
        }
        else //pathが渡されなかった場合sizeをもとにランダムマップを生成する
        {
            this.setRandomMap(size);
        }
    }

    public void setRandomMap(int[] size) //ランダムにmapを生成する
    {
        if(!size) //sizeの値が与えられなかった場合
        {
            Array.Copy([15,17], size, 2); //デフォルト値として15×17マップを用いる
        }
        Array.Copy(size, map.size, 2);
        map.data = new int[size[1], size[0]];

    }

    //ゲッター系

    public int[,] getMapData() //mapのデータを返す
    {
        int[,] returnData = map.data.Clone() as int[,];
        returnData[map.coolPosition[1], map.coolPosition[0]] = 4; //mapデータのcoolの位置を4に置き換える
        returnData[map.hotPosition[1], map.hotPosition[0]] = 5; //mapデータのhotの位置を5に置き換える
        return returnData; //coolを4,hotを5にしたmapデータを送る
    }

    public int[] getTurn() //ゲーム進行中のmapの残りターン数を返す
    {
        return map.turn;
    }

    public int getItem() //ゲーム進行中のmapのアイテムの総数を返す
    {
        return map.getItem();
    }

    public bool getIsContinue()
    {
        return this.isContinue;
    }

    public string[,] getDifference()
    {
        return this.difference;
    }

    //行動のメソッド系

    public int[] ActChar(string character, string command) //キャラクターを行動によってそれぞれのメソッドを呼ぶメソッド
    {
        int[] values = new int[10]{0,0,0,0,0,0,0,0,0,0}; //デフォルトとしてゲームが継続できないときの情報で初期化する
        this.difference = null;

        if(!isContinue) return values; //相手が壁にはまっていたらゲーム終了

        if(command.StartsWith("w")) //walkの時のメソッドを呼ぶ
        {
            values = this.WalkChar(character, command.Substring(1,1));
        }
        else if(command.StartsWith("l")) //lookの時のメソッドを呼ぶ
        {
            values = this.LookChar(character, command.Substring(1,1));
        }
        else if(command.StartsWith("s")) //searchの時のメソッドを呼ぶ
        {
            values = this.SearchChar(character, command.Substring(1,1));
        }
        else if(command.StartsWith("p")) //putの時のメソッドを呼ぶ
        {
            values = this.PutChar(character, command.Substring(1,1));
        }
        else //上記以外はgetReady
        {
            //char(X,Y)に位置を代入する
            if(character.Equals("Cool"))
            {
                charX = map.coolPosition[1];
                charY = map.coolPosition[0];
            }
            else if(character.Equals("Hot"))
            {
                charX = map.hotPosition[1];
                charY = map.hotPosition[0];
            }

            values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
        }
        return values;
    }

    private int[] AroundChar(int charX, int charY, string character) //与えられた座標の周辺情報の取得
    {
        int[] values = new int[10]{1,0,0,0,0,0,0,0,0,0}; //デフォルトとしてゲームが継続できないときの情報で初期化する
        int n = 0; //2重for文の回った回数(valuesの番地)

        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                n++;
                if(charX + i < 0 || charX + i >= map.size[1] || charY + j < 0 || charY + j >= map.size[0]) //マップの範囲外を見たとき
                {
                    values[n] = 0; //0を代入
                }
                else if(character.Equals("Hot") && charX + i == coolPosition[0] && charY + j == coolPosition[1])
                {
                    values[n] = 1;
                }
                else if(character.Equals("Cool") && charX + i == hotPosition[0] && charY + j == hotPosition[1])
                {
                    values[n] = 1;
                }
                else
                {
                    values[n] = map.data[charX + i, charY + j]; //valuesにデータを代入
                }
            }
        }
        return values;
    }

    private int[] WalkChar(string character, string direction) //walkのときのメソッド
    {
        int[] values = new int[10]{0,0,0,0,0,0,0,0,0,0}; //デフォルトとしてゲームが継続できないときの情報で初期化する
        int charX;
        int charY;
        int charNum;
        bool score = false;

        //char(X,Y)に位置を代入する
        if(character.Equals("Cool"))
        {
            charX = map.coolPosition[1];
            charY = map.coolPosition[0];
            charNum = 4;
        }
        else if(character.Equals("Hot"))
        {
            charX = map.hotPosition[1];
            charY = map.hotPosition[0];
            charNum = 5;
        }

        //移動後の値をdChar(X,Y)としてchar(X,Y)で初期化して宣言
        int dCharX = charX;
        int dCharY = charY;

        //方向によって移動量を調整
        switch(direction)
        {
            case "r": //右方向なら
                dCharX = charX + 1; //x+1
                break;
            case "l": //左方向なら
                dCharX = charX - 1; //x-1
                break;
            case "u": //上方向なら
                dCharY = charY - 1; //y-1
                break;
            case "d": //下方向なら
                dCharY = charY + 1; //y+1
                break;
        }

        if(dCharX < 0 || dCharX >= map.size[1] || dCharY < 0 || dCharY >= map.size[0]) //画面外に出たかの判定
        {
            this.isContinue = false; //継続判定をfalseに
            this.difference = new int[1,3]{{charX, charY, 0}}; //差分情報の保存
        }
        else if(map.data[dCharY, dCharX] == 2) //もし壁のマスに進んだら
        {
            this.isContinue = false; //継続判定をfalseに
            values = AroundChar(dCharX, dCharY, character); //一応情報を取得
            values[0] = 0; //ゲーム終了情報の記録
            this.difference = new int[2,3]{{charX, charY, 0},{dCharX, dCharY, charNum + 2}}; //差分情報の保存
        }
        else if(map.data[dCharY, dCharX] == 3) //もしアイテムのマスに進んだら
        {
            map.data[charY, charX] = 2; //もといたマスに壁を設置
            values = AroundChar(dCharX, dCharY, character); //壁になった周辺情報を取得
            score = true; //スコアの加算を有効化
            this.difference = new int[2,3]{{charX, charY, 2},{dCharX, dCharY, charNum}}; //差分情報の保存
        }
        else
        {
            values = AroundChar(dCharX, dCharY, character); //移動後の周辺情報を取得
            this.difference = new int[2,3]{{charX, charY, 0},{dCharX, dCharY, charNum}}; //差分情報の保存
        }

        //char(X,Y)の位置をもとにchaserMap型に代入する
        if(character.Equals("Cool"))
        {
            map.coolPosition[1] = dCharX;
            map.coolPosition[0] = dCharY;
            if(score) this.coolScore++;
        }
        else if(character.Equals("Hot"))
        {
            map.hotPosition[1] = dCharX;
            map.hotPosition[0] = dCharY;
            if(score) this.hotScore++;
        }

        --map.turn;
        return values;
    }

    private int[] LookChar(string character, string direction) //lookのときのメソッド
    {
        int[] values; //デフォルトとしてゲームが継続できないときの情報で初期化する
        int charX;
        int charY;

        //char(X,Y)に位置を代入する
        if(character.Equals("Cool"))
        {
            charX = map.coolPosition[1];
            charY = map.coolPosition[0];
        }
        else if(character.Equals("Hot"))
        {
            charX = map.hotPosition[1];
            charY = map.hotPosition[0];
        }

        //座標をずらしてgetReadyと同じことをする
        switch(direction) 
        {
            case "r":
                charX += 2;
            case "l":
                charX -= 2;
            case "u":
                charY -= 2;
            case "d":
                charY += 2;
        }

        values = AroundChar(charX, charY, character);
        this.difference = null;

        --map.turn;
        return values;
    }

    private int[] SearchChar(string character, string direction) //searchのときのメソッド
    {
        int[] values = new int[10]{1,0,0,0,0,0,0,0,0,0}; //デフォルトとしてゲームが継続できないときの情報で初期化する
        int charX;
        int charY;
        int charNum;

        //char(X,Y)に位置を代入する
        if(character.Equals("Cool"))
        {
            charX = map.coolPosition[1];
            charY = map.coolPosition[0];
        }
        else if(character.Equals("Hot"))
        {
            charX = map.hotPosition[1];
            charY = map.hotPosition[0];
        }

        int dCharX = 0;
        int dCharY = 0;

        switch(direction)
        {
            case "r":
                dCharX = 1;
                break;
            case "l":
                dCharX = -1;
                break;
            case "u":
                dCharY = -1;
                break;
            case "d":
                dCharY = 1;
                break;
        }

        for(int i = 1; i <= 9; i++)
        {
            if(charX + (i * dCharX) < 0 || charX + (i * dCharX) >= map.size[1] || charY + (i * dCharY) < 0 || charY + (i * dCharY) >= map.size[0])
            {
                values[i] = 0;
            }
            else if(character.Equals("Hot") && charX + (i * dCharX) == coolPosition[0] && charY + (i * dCharY)== coolPosition[1])
            {
                values[i] = 1;
            }
            else if(character.Equals("Cool") && charX + (i * dCharX) == hotPosition[0] && charY + (i * dCharY)== hotPosition[1])
            {
                values[i] = 1;
            }
            else
            {
                values[i] = map.data[charY + (i * dCharY), charX + (i * dCharX)];
            }
        }
        this.difference = null;

        --map.turn;
        return values;
    }

    private int[] PutChar(string character, string direction) //putのときのメソッド
    {
        int[] values = new int[10]{0,0,0,0,0,0,0,0,0,0}; //デフォルトとしてゲームが継続できないときの情報で初期化する
        int charX;
        int charY;
        int charNum;

        //char(X,Y)に位置を代入する
        if(character.Equals("Cool"))
        {
            charX = map.coolPosition[1];
            charY = map.coolPosition[0];
            charNum = 4;
        }
        else if(character.Equals("Hot"))
        {
            charX = map.hotPosition[1];
            charY = map.hotPosition[0];
            charNum = 5;
        }

        int dCharX = charX;
        int dCharY = charY;

        switch(direction)
        {
            case "r":
                dCharX = charX + 1;
                break;
            case "l":
                dCharX = charX - 1;
                break;
            case "u":
                dCharY = charY - 1;
                break;
            case "d":
                dCharY = charY + 1;
                break;
        }

        if(dCharX < 0 || dCharX >= map.size[1] || dCharY < 0 || dCharY >= map.size[0])
        {
            values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
            this.difference = null;
        }
        else if(dCharX == coolPosition[0] && dCharY == coolPosition[1])
        {
            this.isContinue = false; //継続判定をfalseに
            map.data[dCharY, dCharX] = 2;
            values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
            this.difference = new int[1,3]{{dCharX, dCharY, charNum + 2}}; //差分情報の保存
        }
        else if(dCharX == hotPosition[0] && dCharY == hotPosition[1])
        {
            this.isContinue = false; //継続判定をfalseに
            map.data[dCharY, dCharX] = 2;
            values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
            this.difference = new int[1,3]{{dCharX, dCharY, charNum + 2}}; //差分情報の保存
        }
        else
        {
            map.data[dCharY, dCharX] = 2;
            values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
            this.difference = new int[1,3]{{dCharX, dCharY, 2}}; //差分情報の保存
        }

        --map.turn;
        return values;
    }
}
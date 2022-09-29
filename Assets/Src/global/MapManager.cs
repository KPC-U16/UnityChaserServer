using System;

public class MapManager
{
    ChaserMap map; //mapにChaserMap型をnewしてインスタンスを作る
    int hotScore; //ホットのスコアを持つ
    int coolScore; //クールのスコアを持つ
    int[,] difference; //mapの直近のデータを持つ
    bool isContinue; //ゲームの継続判定
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

    public MapManager()
    {
        map = new ChaserMap();
        hotScore = 0;
        coolScore = 0;
        isContinue = true;
    }

    //mapのset系
    public async void setMap(string path) //mapファイルを用いてmapを生成するメソッド
    {
        await map.setMap(path);
    }

    public async void setCustomMap(string path, int[] size) //マップ編集時に呼び出される
    {
        if(!string.IsNullOrEmpty(path)) //引数にpathを渡されたら.mapファイルを開いて展開する
        {
            await map.setMap(path);
        }
        else //pathが渡されなかった場合sizeをもとにランダムマップを生成する
        {
            this.setRandomMap(size);
        }
    }

    public void setRandomMap(int[] size) //ランダムにmapを生成する
    {
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

    public int getTurn() //ゲーム進行中のmapの残りターン数を返す
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

    public int[,] getDifference()
    {
        return this.difference;
    }

    //行動のメソッド系

    public int[] ActChar(string character, string command) //キャラクターを行動によってそれぞれのメソッドを呼ぶメソッド
    {
        int[] values = new int[10]{0,0,0,0,0,0,0,0,0,0}; //デフォルトとしてゲームが継続できないときの情報で初期化する
        this.difference = null;
        int charX = -1;
        int charY = -1;

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
                charX = map.coolPosition[0];
                charY = map.coolPosition[1];
            }
            else if(character.Equals("Hot"))
            {
                charX = map.hotPosition[0];
                charY = map.hotPosition[1];
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
                n = n + 1;
                if(charX + j < 0 || charX + j >= map.size[0] || charY + i < 0 || charY + i >= map.size[1]) //マップの範囲外を見たとき
                {
                    values[n] = 2; //2(壁)を代入
                }
                else if(character.Equals("Hot") && charX + j == map.coolPosition[0] && charY + i == map.coolPosition[1])
                {
                    values[n] = 1;
                }
                else if(character.Equals("Cool") && charX + j == map.hotPosition[0] && charY + i == map.hotPosition[1])
                {
                    values[n] = 1;
                }
                else
                {
                    values[n] = map.data[charY + i, charX + j]; //valuesにデータを代入
                }
            }
        }
        return values;
    }

    private int[] WalkChar(string character, string direction) //walkのときのメソッド
    {
        int[] values = new int[10]{0,0,0,0,0,0,0,0,0,0}; //デフォルトとしてゲームが継続できないときの情報で初期化する
        int charX = -1;
        int charY = -1;
        int charNum = -1; //おかしい数値に設定してるのでエラーを吸収してくれ
        bool score = false;

        //char(X,Y)に位置を代入する
        if(character.Equals("Cool"))
        {
            charX = map.coolPosition[0];
            charY = map.coolPosition[1];
            charNum = 4;
        }
        else if(character.Equals("Hot"))
        {
            charX = map.hotPosition[0];
            charY = map.hotPosition[1];
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

        if(dCharX < 0 || dCharX >= map.size[0] || dCharY < 0 || dCharY >= map.size[1]) //画面外に出たかの判定
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
            map.data[dCharY, dCharX] = 0; //もといたマスを0(通路)に変更
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
            map.coolPosition[0] = dCharX;
            map.coolPosition[1] = dCharY;
            if(score) this.coolScore++;
        }
        else if(character.Equals("Hot"))
        {
            map.hotPosition[0] = dCharX;
            map.hotPosition[1] = dCharY;
            if(score) this.hotScore++;
        }

        --map.turn;
        return values;
    }

    private int[] LookChar(string character, string direction) //lookのときのメソッド
    {
        int[] values; //デフォルトとしてゲームが継続できないときの情報で初期化する
        int charX = -1;
        int charY = -1;

        //char(X,Y)に位置を代入する
        if(character.Equals("Cool"))
        {
            charX = map.coolPosition[0];
            charY = map.coolPosition[1];
        }
        else if(character.Equals("Hot"))
        {
            charX = map.hotPosition[0];
            charY = map.hotPosition[1];
        }

        //座標をずらしてgetReadyと同じことをする
        switch(direction) 
        {
            case "r":
                charX += 2;
                break;
            case "l":
                charX -= 2;
                break;
            case "u":
                charY -= 2;
                break;
            case "d":
                charY += 2;
                break;
        }

        values = AroundChar(charX, charY, character);
        this.difference = null;

        --map.turn;
        return values;
    }

    private int[] SearchChar(string character, string direction) //searchのときのメソッド
    {
        int[] values = new int[10]{1,0,0,0,0,0,0,0,0,0}; //デフォルトとしてゲームが継続できないときの情報で初期化する
        int charX = -1;
        int charY = -1;

        //char(X,Y)に位置を代入する
        if(character.Equals("Cool"))
        {
            charX = map.coolPosition[0];
            charY = map.coolPosition[1];
        }
        else if(character.Equals("Hot"))
        {
            charX = map.hotPosition[0];
            charY = map.hotPosition[1];
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

        for(int i = 1; i < 10; i++)
        {
            if(charX + (i * dCharX) < 0 || charX + (i * dCharX) >= map.size[0] || charY + (i * dCharY) < 0 || charY + (i * dCharY) >= map.size[1]) //mapの範囲外を見たとき
            {
                values[i] = 2; //2(壁)を返す
            }
            else if(character.Equals("Hot") && charX + (i * dCharX) == map.coolPosition[0] && charY + (i * dCharY)== map.coolPosition[1])
            {
                values[i] = 1;
            }
            else if(character.Equals("Cool") && charX + (i * dCharX) == map.hotPosition[0] && charY + (i * dCharY)== map.hotPosition[1])
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
        int charX = -1;
        int charY = -1;
        int charNum = -1;

        //char(X,Y)に位置を代入する
        if(character.Equals("Cool"))
        {
            charX = map.coolPosition[0];
            charY = map.coolPosition[1];
            charNum = 4;
        }
        else if(character.Equals("Hot"))
        {
            charX = map.hotPosition[0];
            charY = map.hotPosition[1];
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
        else if(dCharX == map.coolPosition[0] && dCharY == map.coolPosition[1])
        {
            this.isContinue = false; //継続判定をfalseに
            map.data[dCharY, dCharX] = 2;
            values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
            this.difference = new int[1,3]{{dCharX, dCharY, charNum + 2}}; //差分情報の保存
        }
        else if(dCharX == map.hotPosition[0] && dCharY == map.hotPosition[1])
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
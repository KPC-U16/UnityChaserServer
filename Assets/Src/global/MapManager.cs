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
        bool score;

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

        switch(direction)
        {
            case "r":
                charX++;
                if(charX == map.size[1]) //画面外に出たかの判定
                {
                    this.isContinue = false; //継続判定をfalseに
                    this.difference = new int[1,3]{{charX - 1, charY, 0}}; //差分情報の保存
                }
                else if(map.data[charY, charX] == 2) //もし壁のマスに進んだら
                {
                    this.isContinue = false; //継続判定をfalseに
                    values = AroundChar(charX, charY, character); //一応情報を取得
                    values[0] = 0; //ゲーム終了情報の記録
                    this.difference = new int[2,3]{{charX - 1, charY, 0},{charX, charY, charNum + 2}}; //差分情報の保存
                }
                else if(map.data[charY, charX] == 3) //もしアイテムのマスに進んだら
                {
                    map.data[charY, charX - 1] = 2; //もといたマスに壁を設置
                    values = AroundChar(charX, charY, character); //壁になった周辺情報を取得
                    score = true;
                    this.difference = new int[2,3]{{charX - 1, charY, 2},{charX, charY, charNum}}; //差分情報の保存
                }
                else
                {
                    values = AroundChar(charX, charY, character); //移動後の周辺情報を取得
                    this.difference = new int[2,3]{{charX - 1, charY, 0},{charX, charY, charNum}}; //差分情報の保存
                }
            case "l":
                charX--;
                if(charX == -1) //画面外に出たかの判定
                {
                    this.isContinue = false; //継続判定をfalseに
                    this.difference = new int[1,3]{{charX + 1, charY, 0}}; //差分情報の保存
                }
                else if(map.data[charY, charX] == 2) //もし壁のマスに進んだら
                {
                    this.isContinue = false; //継続判定をfalseに
                    values = AroundChar(charX, charY, character); //一応情報を取得
                    values[0] = 0; //ゲーム終了情報の記録
                    this.difference = new int[2,3]{{charX + 1, charY, 0},{charX, charY, charNum + 2}}; //差分情報の保存
                }
                else if(map.data[charY, charX] == 3) //もしアイテムのマスに進んだら
                {
                    map.data[charY, charX + 1] = 2; //もといたマスに壁を設置
                    values = AroundChar(charX, charY, character); //壁になった周辺情報を取得
                    score = true;
                    this.difference = new int[2,3]{{charX + 1, charY, 2},{charX, charY, charNum}}; //差分情報の保存
                }
                else
                {
                    values = AroundChar(charX, charY, character); //移動後の周辺情報を取得
                    this.difference = new int[2,3]{{charX + 1, charY, 0},{charX, charY, charNum}}; //差分情報の保存
                }
            case "u":
                charY--;
                if(charY == -1) //画面外に出たかの判定
                {
                    this.isContinue = false; //継続判定をfalseに
                    this.difference = new int[1,3]{{charX, charY + 1, 0}}; //差分情報の保存
                }
                else if(map.data[charY, charX] == 2) //もし壁のマスに進んだら
                {
                    this.isContinue = false; //継続判定をfalseに
                    values = AroundChar(charX, charY, character); //一応情報を取得
                    values[0] = 0; //ゲーム終了情報の記録
                    this.difference = new int[2,3]{{charX, charY + 1, 0},{charX, charY, charNum + 2}}; //差分情報の保存
                }
                else if(map.data[charY, charX] == 3) //もしアイテムのマスに進んだら
                {
                    map.data[charY + 1, charX] = 2; //もといたマスに壁を設置
                    values = AroundChar(charX, charY, character); //壁になった周辺情報を取得
                    score = true;
                    this.difference = new int[2,3]{{charX, charY + 1, 2},{charX, charY, charNum}}; //差分情報の保存
                }
                else
                {
                    values = AroundChar(charX, charY, character); //移動後の周辺情報を取得
                    this.difference = new int[2,3]{{charX, charY + 1, 0},{charX, charY, charNum}}; //差分情報の保存
                }
            case "d":
                charY++;
                if(charY == map.size[0]) //画面外に出たかの判定
                {
                    this.isContinue = false; //継続判定をfalseに
                    this.difference = new int[1,3]{{charX, charY - 1, 0}}; //差分情報の保存
                }
                else if(map.data[charY, charX] == 2) //もし壁のマスに進んだら
                {
                    this.isContinue = false; //継続判定をfalseに
                    values = AroundChar(charX, charY, character); //一応情報を取得
                    values[0] = 0; //ゲーム終了情報の記録
                    this.difference = new int[2,3]{{charX, charY - 1, 0},{charX, charY, charNum + 2}}; //差分情報の保存
                }
                else if(map.data[charY, charX] == 3) //もしアイテムのマスに進んだら
                {
                    map.data[charY - 1, charX] = 2; //もといたマスに壁を設置
                    values = AroundChar(charX, charY, character); //壁になった周辺情報を取得
                    score = true;
                    this.difference = new int[2,3]{{charX, charY - 1, 2},{charX, charY, charNum}}; //差分情報の保存
                }
                else
                {
                    values = AroundChar(charX, charY, character); //移動後の周辺情報を取得
                    this.difference = new int[2,3]{{charX, charY - 1, 0},{charX, charY, charNum}}; //差分情報の保存
                }
        }
        //char(X,Y)の位置をもとにchaserMap型に代入する
        if(character.Equals("Cool"))
        {
            map.coolPosition[1] = charX;
            map.coolPosition[0] = charY;
            if(score) this.coolScore++;
        }
        else if(character.Equals("Hot"))
        {
            map.hotPosition[1] = charX;
            map.hotPosition[0] = charY;
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
                values = AroundChar(charX, charY, character);
                this.difference = null;
            case "l":
                charX -= 2;
                values = AroundChar(charX, charY, character);
                this.difference = null;
            case "u":
                charY -= 2;
                values = AroundChar(charX, charY, character);
                this.difference = null;
            case "d":
                charY += 2;
                values = AroundChar(charX, charY, character);
                this.difference = null;
        }

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

        switch(direction)
        {
            case "r":
                for(int i = 1; i <= 9; i++)
                {
                    if(charX + i >= map.size[1])
                    {
                        values[i] = 0;
                    }
                    else if(character.Equals("Hot") && charX + i == coolPosition[0] && charY == coolPosition[1])
                    {
                        values[i] = 1;
                    }
                    else if(character.Equals("Cool") && charX + i == hotPosition[0] && charY == hotPosition[1])
                    {
                        values[i] = 1;
                    }
                    else
                    {
                        values[i] = map.data[charY, charX + i];
                    }
                }
                this.difference = null;
            case "l":
                for(int i = 1; i<= 9; i++)
                {
                    if(charX - i < 0)
                    {
                        values[i] = 0;
                    }
                    else if(character.Equals("Hot") && charX - i == coolPosition[0] && charY == coolPosition[1])
                    {
                        values[i] = 1;
                    }
                    else if(character.Equals("Cool") && charX - i == hotPosition[0] && charY == hotPosition[1])
                    {
                        values[i] = 1;
                    }
                    else
                    {
                        values[i] = map.data[charY, charX - i];
                    }
                }
                this.difference = null;
            case "u":
                for(int i = 1; i<= 9; i++)
                {
                    if(charY - i < 0)
                    {
                        values[i] = 0;
                    }
                    else if(character.Equals("Hot") && charX == coolPosition[0] && charY - i == coolPosition[1])
                    {
                        values[i] = 1;
                    }
                    else if(character.Equals("Cool") && charX == hotPosition[0] && charY - i == hotPosition[1])
                    {
                        values[i] = 1;
                    }
                    else
                    {
                        values[i] = map.data[charY - i, charX];
                    }
                }
                this.difference = null;
            case "d":
                for(int i = 1; i<= 9; i++)
                {
                    if(charY + i >= map.size[0])
                    {
                        values[i] = 0;
                    }
                    else if(character.Equals("Hot") && charX == coolPosition[0] && charY + i == coolPosition[1])
                    {
                        values[i] = 1;
                    }
                    else if(character.Equals("Cool") && charX == hotPosition[0] && charY + i == hotPosition[1])
                    {
                        values[i] = 1;
                    }
                    else
                    {
                        values[i] = map.data[charY + i, charX];
                    }
                }
                this.difference = null;
        }

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

        switch(direction)
        {
            case "r":
                if(charX + 1 >= map.size[1])
                {
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = null;
                }
                else if(charX + 1 == coolPosition[0] && charY == coolPosition[1])
                {
                    this.isContinue = false; //継続判定をfalseに
                    map.data[charY, charX + 1] = 2;
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = new int[1,3]{{charX + 1, charY, charNum + 2}}; //差分情報の保存
                }
                else if(charX + 1 == hotPosition[0] && charY == hotPosition[1])
                {
                    this.isContinue = false; //継続判定をfalseに
                    map.data[charY, charX + 1] = 2;
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = new int[1,3]{{charX + 1, charY, charNum + 2}}; //差分情報の保存
                }
                else
                {
                    map.data[charY, charX + 1] = 2;
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = new int[1,3]{{charX + 1, charY, 2}}; //差分情報の保存
                }
            case "l":
                if(charX - 1 < 0)
                {
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = null;
                }
                else if(charX - 1 == coolPosition[0] && charY == coolPosition[1])
                {
                    this.isContinue = false; //継続判定をfalseに
                    map.data[charY, charX - 1] = 2;
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = new int[1,3]{{charX - 1, charY, charNum + 2}}; //差分情報の保存
                }
                else if(charX - 1 == hotPosition[0] && charY == hotPosition[1])
                {
                    this.isContinue = false; //継続判定をfalseに
                    map.data[charY, charX - 1] = 2;
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = new int[1,3]{{charX - 1, charY, charNum + 2}}; //差分情報の保存
                }
                else
                {
                    map.data[charY, charX - 1] = 2;
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = new int[1,3]{{charX - 1, charY, 2}}; //差分情報の保存
                }
            case "u":
                if(charY - 1 < 0)
                {
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = null;
                }
                else if(charX == coolPosition[0] && charY - 1 == coolPosition[1])
                {
                    this.isContinue = false; //継続判定をfalseに
                    map.data[charY - 1, charX] = 2;
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = new int[1,3]{{charX, charY - 1, charNum + 2}}; //差分情報の保存
                }
                else if(charX == hotPosition[0] && charY - 1 == hotPosition[1])
                {
                    this.isContinue = false; //継続判定をfalseに
                    map.data[charY - 1, charX] = 2;
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = new int[1,3]{{charX, charY - 1, charNum + 2}}; //差分情報の保存
                }
                else
                {
                    map.data[charY - 1, charX] = 2;
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = new int[1,3]{{charX, charY - 1, 2}}; //差分情報の保存
                }
            case "d":
                if(charY + 1 < 0)
                {
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = null;
                }
                else if(charX == coolPosition[0] && charY + 1 == coolPosition[1])
                {
                    this.isContinue = false; //継続判定をfalseに
                    map.data[charY + 1, charX] = 2;
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = new int[1,3]{{charX, charY + 1, charNum + 2}}; //差分情報の保存
                }
                else if(charX == hotPosition[0] && charY + 1 == hotPosition[1])
                {
                    this.isContinue = false; //継続判定をfalseに
                    map.data[charY + 1, charX] = 2;
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = new int[1,3]{{charX, charY + 1, charNum + 2}}; //差分情報の保存
                }
                else
                {
                    map.data[charY + 1, charX] = 2;
                    values = AroundChar(charX, charY, character); //xとyの周辺情報を返す
                    this.difference = new int[1,3]{{charX, charY + 1, 2}}; //差分情報の保存
                }
        }

        --map.turn;
        return values;
    }
}
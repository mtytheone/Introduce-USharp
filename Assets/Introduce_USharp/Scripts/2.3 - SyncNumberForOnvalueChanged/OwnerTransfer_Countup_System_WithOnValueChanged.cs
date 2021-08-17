#region License
/*------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
/* MIT License                                                                                                                                                                                                                                                                                                                                                                                                                                                                  */
/*                                                                                                                                                                                                                                                                                                                                                                                                                                                                              */
/* Copyright (c) 2021 hatuxes                                                                                                                                                                                                                                                                                                                                                                                                                                                   */
/*                                                                                                                                                                                                                                                                                                                                                                                                                                                                              */
/* Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:                             */
/*                                                                                                                                                                                                                                                                                                                                                                                                                                                                              */
/* The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.                                                                                                                                                                                                                                                                                                                                               */
/*                                                                                                                                                                                                                                                                                                                                                                                                                                                                              */
/* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. */
/*------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
#endregion

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class OwnerTransfer_Countup_System_WithOnValueChanged : UdonSharpBehaviour
{
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CountData))] private int _countData;      // データ本体

    public Text DisplayDataText;  // データを表示するText
    public Text OptionText;       // 誰がOwnerかを表示するText

    // OnValueChanged用のプロパティ
    public int CountData
    {
        get => _countData;
        set
        {
            _countData = value;
            DisplayCountData();  // Owner以外のデータ表示処理
        }
    }



    private void Start()
    {
        // Onwerかどうかを表示
        SetOptionalText(Networking.LocalPlayer);
    }



    // Cubeをインタラクトした時に呼ばれる
    public override void Interact()
    {
        // 新にOwnerになる
        var player = Networking.LocalPlayer;
        Networking.SetOwner(player, this.gameObject);

        if (player.IsOwner(this.gameObject))
        {
            // カウントアップ処理
            CountUp();
        }
    }

    // Ownerが移行した際の処理
    // このイベントはインスタンスにいる全員に発行されるので、Network.LocalPlayerを用いる
    // 引数のplayerは新たなオーナーを指す
    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        SetOptionalText(Networking.LocalPlayer);
    }




    // Ownerが値を+1する処理
    public void CountUp()
    {
        _countData++;             // データ更新
        RequestSerialization();   // 同期更新

        DisplayCountData();       // 表示値更新
    }

    // 同期変数の値をUIに表示する処理
    public void DisplayCountData()
    {
        DisplayDataText.text = _countData.ToString();   // データ表示更新
    }

    // プレイヤーがOwnerかどうかをテキストで表示させる処理
    public void SetOptionalText(VRCPlayerApi player)
    {
        if (player.IsOwner(this.gameObject))
        {
            // Owner側の処理
            OptionText.text = $"<color=red>{player.displayName} is Owner!</color>";
        }
        else
        {
            // Owner以外の処理
            OptionText.text = $"{player.displayName} isn't Owner";
        }
    }
}
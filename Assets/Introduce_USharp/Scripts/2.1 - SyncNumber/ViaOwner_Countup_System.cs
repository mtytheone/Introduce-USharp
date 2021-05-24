#region License
/*------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
/* MIT License                                                                                                                                                                                                                                                                                                                                                                                                                                                                  */
/*                                                                                                                                                                                                                                                                                                                                                                                                                                                                              */
/* Copyright (c) 2020 hatuxes                                                                                                                                                                                                                                                                                                                                                                                                                                                   */
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
public class ViaOwner_Countup_System : UdonSharpBehaviour
{
    [UdonSynced(UdonSyncMode.None)] private int _value;        // データ本体

    public Text DisplayDataText;            // データを表示するText
    public Text OptionText;                 // 誰がOwnerかを表示するText

    private UdonBehaviour _thisBehavior;    // UdonBehavior本体



    private void Start()
    {
        // GetComponent<UdonBehavior>(); はU#の仕様上使えない
        _thisBehavior = (UdonBehaviour)GetComponent(typeof(UdonBehaviour));

        // Onwerかどうかを表示
        SetOptionalText(Networking.LocalPlayer);
    }



    // Cubeをインタラクトしたときに呼ばれる
    public override void Interact()
    {
        var player = Networking.LocalPlayer;

        if (player.IsOwner(this.gameObject))
        {
            // Ownerが押したら、純粋にカウントアップする
            CountUp();
        }
        else
        {
            // Owner以外が押したら、Ownerにカウントアップさせるように命令する
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(CountUp));
        }
    }

    // Owner以外のデータ表示処理
    public override void OnDeserialization()
    {
        DisplayDataText.text = _value.ToString();
    }



    // Ownerが値を+1する処理
    public void CountUp()
    {
        _value++;                                   // データ更新
        _thisBehavior.RequestSerialization();       // 同期更新
        DisplayDataText.text = _value.ToString();   // データ表示更新
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
            OptionText.text = $"{player.displayName} don't Owner";
        }
    }
}
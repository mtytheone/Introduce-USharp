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
public class OwnerTransfer_Countup_System : UdonSharpBehaviour
{
    [UdonSynced] private int _value;      // データ本体

    public Text DisplayDataText;          // データを表示するText
    public Text OptionText;               // 誰がOwnerかを表示するText

    private UdonBehaviour _thisBehavior;  // UdonBehavior本体



    private void Start()
    {
        // GetComponent<UdonBehavior>(); はU#の仕様上使えない
        _thisBehavior = (UdonBehaviour)GetComponent(typeof(UdonBehaviour));

        var player = Networking.LocalPlayer;
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



    // Owner以外のデータ表示処理
    public override void OnDeserialization()
    {
        DisplayDataText.text = _value.ToString();
    }

    // SetOwnerのリクエストが飛んだ際に呼ばれる関数
    // 譲渡の可否を返す必要がある
    public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
    {
        return true;  // 譲渡を許可
    }

    // Ownerが移行した際の処理
    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        Debug.Log("OnOwnershipTransferred!");

        // このイベントはインスタンスにいる全員に発行されるので、Network.LocalPlayerを用いる
        // 引数のplayerは新たなオーナーを指す
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
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



    // 新にOwnerになって、データを自身で更新する
    public void RequestCountUp()
    {
        var player = Networking.LocalPlayer;
        Networking.SetOwner(player, this.gameObject);

        if (player.IsOwner(this.gameObject))
        {
            CountUp();
        }
    }

    // Ownerが値を+1する処理
    public void CountUp()
    {
        _value++;                                   // データ更新
        DisplayDataText.text = _value.ToString();   // データ表示

        // Owner変更後に即時更新すると、新しいOwner以外に値がうまく反映されない（次のNetworkTickにならないと反映されないらしい）
        SendCustomEventDelayedSeconds(nameof(SerializeData), 0.4f);
    }

    public void SerializeData()
    {
        _thisBehavior.RequestSerialization();       // 同期更新
    }
}
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
    [UdonSynced(UdonSyncMode.None)] private int _countData;      // データ本体

    public Text DisplayDataText;          // データを表示するText
    public Text OptionText;               // 誰がOwnerかを表示するText



    private void Start()
    {
        // Onwerかどうかを表示
        SetOptionalText(Networking.LocalPlayer);
    }



    // Cubeをインタラクトした時に呼ばれる
    // 新にOwnerになって、データを自身で更新する
    public override void Interact()
    {
        var player = Networking.LocalPlayer;
        Networking.SetOwner(player, this.gameObject);

        if (player.IsOwner(this.gameObject))
        {
            CountUp();
        }
    }

    // Owner以外のデータ表示処理
    public override void OnDeserialization()
    {
        DisplayDataText.text = _countData.ToString();
    }

    // SetOwnerのリクエストが飛んだ際に呼ばれる関数
    // 譲渡の可否を返す必要がある
    // trueを返すだけなので、書かなくても良い
    public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
    {
        return true;  // 譲渡を許可
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
        _countData++; // データ更新

        // Owner変更後に即時更新すると、OnDeserializationに追いつかないことがあるため。少し遅延させている
        SendCustomEventDelayedSeconds(nameof(SerializeData), 0.4f);


        DisplayDataText.text = _countData.ToString();   // データ表示
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

    public void SerializeData()
    {
        RequestSerialization();       // 同期更新
    }
}
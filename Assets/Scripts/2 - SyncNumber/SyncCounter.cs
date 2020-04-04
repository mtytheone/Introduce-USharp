#region License
/*--------------------------------------------*/
/*  A license of this script is MIT License.  */
/*       Writed by "hatuxes"(@kohu_vr)        */
/*                   2020                     */
/*--------------------------------------------*/
#endregion

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class SyncCounter : UdonSharpBehaviour
{
    public Text text;

    [UdonSynced(UdonSyncMode.None)]
    int _num;

    void Start()
    {
        _num = 0;
    }

    void Update()
    {
        text.text = _num.ToString();
    }

    public override void Interact()
    {
        //Cubeに対するオーナーを変更
        if (Networking.GetOwner(this.gameObject) != Networking.LocalPlayer) Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        //処理同期
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CountUp");
    }

    public void CountUp()
    {
        _num++;
    }
}
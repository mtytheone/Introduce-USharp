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

public class Counter : UdonSharpBehaviour
{
    public Text text;

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
        _num++;
    }
}
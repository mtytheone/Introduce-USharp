#region License
/*--------------------------------------------*/
/*  A license of this script is MIT License.  */
/*       Writed by "hatuxes"(@kohu_vr)        */
/*                   2020                     */
/*--------------------------------------------*/
#endregion

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RotateCube : UdonSharpBehaviour
{
    public float RotateSpeed;

    void Update()
    {
        this.gameObject.transform.Rotate(this.gameObject.transform.up * RotateSpeed * Time.deltaTime);
    }
}
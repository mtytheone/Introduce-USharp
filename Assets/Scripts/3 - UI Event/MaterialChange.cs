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

public class MaterialChange : UdonSharpBehaviour
{
    public MeshRenderer Ground;

    public Material BeforeMaterial;
    public Material AfterMaterial;

    void Start()
    {
        Ground.sharedMaterial = BeforeMaterial;
    }

    public void ChangeMaterial()
    {
        //ボタンによるマテリアル更新
        if (Ground.sharedMaterial == BeforeMaterial) Ground.sharedMaterial = AfterMaterial;
        else Ground.sharedMaterial = BeforeMaterial;
    }
}
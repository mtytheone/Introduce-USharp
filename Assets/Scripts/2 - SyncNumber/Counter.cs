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
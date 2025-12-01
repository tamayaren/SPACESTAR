using System;
using UnityEngine;

public class OnWin : MonoBehaviour
{
    private bool hasTouched = false;

    public void EnterMode()
    {
        if (!this.hasTouched)
        {
            this.hasTouched = true;

            StartCoroutine(UIWin.instance.Win(GameObject.FindObjectOfType<PlayerFirstPersonCamera>()));
        }
    }
}

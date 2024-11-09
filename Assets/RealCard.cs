using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealCard : MonoBehaviour
{
    public SpriteRenderer face;
    public Transform shadow;

    //private void OnValidate()
    //{
    //    float eulerx = (transform.localEulerAngles.x + 360) % 360;
    //    Vector3 newShadowEuler = shadow.localEulerAngles;
    //    if (eulerx == 0)
    //    {
    //        newShadowEuler.y = 90;
    //    }
    //    else
    //    {
    //        newShadowEuler.y = 0;
    //    }
    //    shadow.localEulerAngles = newShadowEuler;
    //}
}

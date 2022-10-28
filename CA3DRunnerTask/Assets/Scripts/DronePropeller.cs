using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DronePropeller : MonoBehaviour
{
    [SerializeField] float CicleLength = 2f;
    void Start()
    {
        transform.DORotate(new Vector3(0,360,0),CicleLength * 0.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear).SetLoops(-1,LoopType.Restart);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Obstacle : MonoBehaviour
{
    [SerializeField] float CicleLength = 2;
    void Start()
    {
        // var seq = DOTween.Sequence();
        // seq.Append(transform.DOMoveX(7,CicleLength))
        //     .Join(transform.DORotate(new Vector3(0,360,0),CicleLength, RotateMode.FastBeyond360))
        //     .SetLoops(-1,LoopType.Yoyo);


        // transform.DOMoveX(0.5f,CicleLength).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        transform.DORotate(new Vector3(0,360,0),CicleLength * 0.7f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear).SetLoops(-1,LoopType.Restart);

    }

    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimationGeneric : MonoBehaviour
{

    [SerializeField] private Transform asset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MoveBounce(float duration) {
        asset.DOLocalJump(transform.position, 0.2f, 1, duration, true);
    }

    public void MoveShift(Vector3 direction, float duration) {
        asset.DOPunchPosition(direction, duration, 2, 0.5f, true);
    }

}

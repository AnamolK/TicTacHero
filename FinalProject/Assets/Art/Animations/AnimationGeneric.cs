using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/*******************************************************************************
    Within script that wants to trigger animation on event, add:

        //animation/asset manager
        [SerializeField] private GameObject asset;
        private new AnimationGeneric animation;
            
    In Start(), add:

        animation = asset.GetComponent<AnimationGeneric>();

    In Unity Editor, set asset to the asset object/container that needs to
    be animated.
*******************************************************************************/

public class AnimationGeneric : MonoBehaviour
{
    [SerializeField] private Transform asset;
    
    // Start is called before the first frame update
    void Start()
    {
    }
    private void FixedUpdate()
    {
        
    }

    public void MoveBounce(float duration) {
        asset.localPosition = Vector3.zero;
        asset.DOLocalJump(new Vector3(0,0,0), 0.4f, 1, duration, false).OnComplete(() => {
            resetPosition(0.001f);
        });
    }

    public void MoveShift(Vector3 direction, float duration) {
        asset.localPosition = Vector3.zero;
        asset.DOPunchPosition(direction, duration, 1, 0.5f, false).OnComplete(() => {
            resetPosition(0.3f);
        });
        Debug.Log("Animation Played: MoveShift");
    }

    public void AttackMelee(Vector3 direction, float duration) {
        asset.localPosition = Vector3.zero;
        asset.DOPunchPosition(direction, duration, 3, 0.5f, false).OnComplete(() => {
            resetPosition(1f);
        });
        Debug.Log("Animation Played: AttackMelee");
    }

    public void DamageTaken(float duration) {
        asset.DOShakePosition (duration, 0.3f, 20, 90, false, true, ShakeRandomnessMode.Full).OnComplete(() => {
            resetPosition(1f);
        });
        Debug.Log("Animation Played: DamageTaken");
    }

    public void DamageTakenEnemy(float duration) {
        asset.DOPunchRotation(new Vector3(0,0,0), duration, 20, 0.3f).OnComplete(() => {
            resetPosition(1f);
        });
        Debug.Log("Animation Played: DamageTakenEnemy");
    }

    public void DieEnemy(float duration) {
        asset.DOLocalRotate(new Vector3(0,0,110), duration-0.05f, RotateMode.Fast);
        asset.DOLocalRotate(new Vector3(0,0,-20), 0.05f, RotateMode.Fast);
    }

    private void resetPosition(float duration) {
        if (asset.localPosition != new Vector3(0,0,0)) {
            asset.DOLocalMove(new Vector3(0,0,0), duration, false);
            asset.DOLocalRotate(new Vector3(0,0,0), duration);
            
        }
    }
}

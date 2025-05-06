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
    public GameObject shadowAsset;
    private SpriteRenderer shadow;
    
    // Start is called before the first frame update
    void Start()
    {
    }
    private void FixedUpdate()
    {
        
    }

    public void FadeShadow(float duration, int amount) {
        if (shadow != null) {
            shadow.DOFade(amount, duration);
        }
    }

    public void MoveBounce(float duration) {
        asset.localPosition = Vector3.zero;
        asset.DOLocalJump(new Vector3(0,0,0), 0.4f, 1, duration, false).OnComplete(() => {
            resetPosition(0.001f);
        });
    }

    public void MoveDragon(float duration) {
        asset.localPosition = Vector3.zero;
        asset.DOPunchRotation(new Vector3(0,0,15), duration, 4, 0.8f).OnComplete(() => {
            resetPosition(1f);
        });
    }

    public void MoveSlimeBoss(float duration, float amount) {

        if (shadowAsset == null) {
            return;
        }
        
        shadow = shadowAsset.GetComponent<SpriteRenderer>();
        asset.localPosition = Vector3.zero;
        asset.DOLocalMoveY(0, duration*0.02f).OnComplete(() => {

            shadow.DOFade(0.8f, duration*0.01f);
            shadow.DOFade(0.3f, duration*0.57f);
            asset.DOLocalMoveY(amount, duration*0.58f).SetEase(Ease.OutCubic).OnComplete(() => {
                asset.DOLocalMove(Vector3.zero, duration*0.35f).SetEase(Ease.InCubic).OnComplete(() => {
                    asset.DOShakeScale(duration*0.05f, new Vector3(0, 0.07f, 0), 3, 10, true, ShakeRandomnessMode.Harmonic);
                    resetPosition(0.001f);
                });
                shadow.DOFade(0.8f, duration*0.35f).OnComplete(() => {
                    shadow.DOFade(0f, duration*0.05f);
                });
            });
        });
    }

    public void MoveShift(Vector3 direction, float duration) {
        resetPosition(0.01f);
        asset.localPosition = Vector3.zero;
        asset.DOPunchPosition(direction, duration, 1, 0.5f, false).OnComplete(() => {
            resetPosition(0.3f);
        });
    }

    public void AttackMelee(Vector3 direction, float duration) {
        resetPosition(0.01f);
        asset.localPosition = Vector3.zero;
        asset.DOPunchPosition(direction, duration, 3, 0.35f, false).OnComplete(() => {
            resetPosition(1f);
        });
    }

    public void AttackSlimeSpawn(float duration) {
        asset.localPosition = Vector3.zero;
        asset.DOShakeScale(duration, new Vector3(0.01f, 0.1f, 0), 4, 20, false, ShakeRandomnessMode.Harmonic).OnComplete(() => {
            resetPosition(1f);
        });
    }

    public void AttackBatSpawn(float duration) {
        asset.localPosition = Vector3.zero;
        asset.DOShakePosition (duration, 0.3f, 20, 90, false, true, ShakeRandomnessMode.Full).OnComplete(() => {
            resetPosition(1f);
        });
    }

    public void DamageTaken(float duration) {
        asset.DOShakePosition (duration, 0.5f, 20, 90, false, true, ShakeRandomnessMode.Full).OnComplete(() => {
            resetPosition(1f);
        });
    }

    public void DamageTakenEnemy(float duration) {
        asset.DOPunchRotation(new Vector3(0,0,30), duration, 20, 0.3f).OnComplete(() => {
            resetPosition(1f);
        });
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
    public void fireHitboxSolver(float duration) {
        asset.DOScaleY(0f, 0f);
        asset.DOScaleY(1f, duration);
    }

    public void fireHitboxSolverSmall(float duration) {
        asset.DOScaleX(0f, 0f);
        asset.DOScaleX(1f, duration);
    }

    void OnDestroy() {
        DOTween.KillAll();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_ColorChange : MonoBehaviour
{

    public Color getHitColor;
    public float returnGetHitDelay = 1f;
    public Color anticipationColor;
    public float returnAnticipateDelay = 1f;

    //the character sprite:
    public SpriteRenderer characterSpriteRenderer;
    private Color defaultColor;


    // Start is called before the first frame update
    void Start()
    {
        //set default spriterenderer and oolor
        //characterSpriteRenderer = GetComponentInChildren<SpriteRenderer> ();
        defaultColor = characterSpriteRenderer.color;
        
    }
     // change the sprite to the anticipatin color, then play oroutne to return
    public void GetHit()
    {
        characterSpriteRenderer.color = getHitColor;
        StartCoroutine(ReturnToDefaultColor(returnGetHitDelay));
    }

    // change the sprite to the anticipatin color, then play oroutne to return
    public void AnticipateAttack()
    {
        characterSpriteRenderer.color = anticipationColor;
        StartCoroutine(ReturnToDefaultColor(returnAnticipateDelay));
    }

    IEnumerator ReturnToDefaultColor(float theDelay){
        yield return new WaitForSeconds(theDelay);
        characterSpriteRenderer.color = defaultColor;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour {
    public GameObject flashHolder;
    public float flashTime;
    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderers;

    public void activate()
    {
        int flashSpriteIndex = Random.Range(0, flashSprites.Length);
        for(int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
        }
        flashHolder.SetActive(true);
        Invoke("deactivate", flashTime);
    }

    public void deactivate()
    {
        flashHolder.SetActive(false);
    }

    void Start()
    {
        deactivate();
    }

}

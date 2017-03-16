using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour {

    public LayerMask targetMask;
    Color initColor;
    public Color highlightColor;
    public SpriteRenderer dot;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;
        initColor = dot.color;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward * 40 * Time.deltaTime);
    }

    public void detectTargets(Ray ray)
    {
        if(Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = highlightColor;
        }
        else
        {
            dot.color = initColor;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour {

    public Rigidbody myRigidbody;
    public float minForce;
    public float maxForce;
    float lifetime = 4;
    float fadeTime = 2;

	// Use this for initialization
	void Start () {
        float force = Random.Range(minForce, maxForce);
        myRigidbody.AddForce(transform.right * force);
        myRigidbody.AddTorque(Random.insideUnitSphere * force);
        StartCoroutine(fade());
	}

    IEnumerator fade()
    {
        yield return new WaitForSeconds(lifetime);
        float fadePercent = 0;
        float fadeSpeed = 1 / fadeTime;
        Material mat = GetComponent<Renderer>().material;
        Color initColor = mat.color;
        while(fadePercent < 1)
        {
            fadePercent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initColor, Color.clear, fadePercent);
            yield return null;
        }
        Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

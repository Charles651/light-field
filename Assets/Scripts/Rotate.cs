using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    public float m_Speed = 50.0f;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.Rotate(Vector3.up, Time.deltaTime * m_Speed);
	}
}

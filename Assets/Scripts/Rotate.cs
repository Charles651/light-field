using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    public float m_Speed;
	// Use this for initialization
	void Start () {
        m_Speed = 50.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.Rotate(Vector3.up, Time.deltaTime * m_Speed);
	}
}

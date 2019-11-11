using UnityEngine;
using System.Collections;

public class MoveController : MonoBehaviour {
	public float Speed = 5.0f;
    public float RotateSpeed = 125.0f;

    //public void aScript otherscript;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float translation = Input.GetAxis("Vertical") * -Speed;
        float rotation = Input.GetAxis("Horizontal") * RotateSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(new Vector3(0, 0, translation), Space.Self);
        transform.Rotate(new Vector3(0, rotation, 0), Space.Self);
    }
}

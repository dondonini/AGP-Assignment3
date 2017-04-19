using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour {

    public Vector3 m_rotationSpeed = new Vector3(0.0f, 1.0f, 0.0f);

	// Update is called once per frame
	void Update () {
        transform.Rotate(m_rotationSpeed * Time.deltaTime);
	}
}

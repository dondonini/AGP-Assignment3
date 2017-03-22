using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCursorFollow : MonoBehaviour {

    public float m_dampening = 0.5f;
    public float m_maxTurnFromCenter = 10.0f;

    [Range(0.0f, 1.0f)]
    public float m_turnPercentage = 0.5f;
    public Vector2 cursorOffset = new Vector2(0.0f, 10.0f);

    private Camera m_mainCamera;

    private Vector3 m_velocity = Vector3.zero;
    private Transform m_lookAtTransform;

	// Use this for initialization
	void Start () {

        // Creating LookAt Transform (AKA GameObject)
        GameObject lookAtPos = Instantiate(new GameObject()) as GameObject;

        lookAtPos.name = "LookAtPos";
        lookAtPos.transform.SetParent(transform.parent);
        m_lookAtTransform = lookAtPos.transform;

        m_mainCamera = transform.GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {

        // Get max distance
        Vector3 lookAtFinalPos = GetMouseVectorFromCenter() * GetClampedMouseDistanceFromCenter(0.0f, m_maxTurnFromCenter);

        Debug.DrawLine(Vector3.zero, lookAtFinalPos, Color.red);

        // Apply percentage
        lookAtFinalPos = Vector3.Lerp(Vector3.zero, lookAtFinalPos, m_turnPercentage);

        Debug.DrawLine(Vector3.zero, lookAtFinalPos, Color.green);

        // SmoothDamp LookAt motion
        m_lookAtTransform.position = Vector3.SmoothDamp(m_lookAtTransform.position, lookAtFinalPos, ref m_velocity, m_dampening);

        // Look at transform
        transform.LookAt(m_lookAtTransform);

    }

    /// <summary>
    /// Get mouse position in 3D space
    /// </summary>
    /// <returns>Mouse position vector</returns>
    private Vector3 Get3DMousePos()
    {
        Vector3 result = Vector3.zero;

        if (Input.mousePresent)
        {
            Ray ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Checks if the ray hits the "CursorPos" plane
            if (Physics.Raycast(ray, out hit, 1 << LayerMask.NameToLayer("CursorPos")))
            {
                result = hit.point;
                result.z = 0.0f;
            }
        }

        return result;
    }

    /// <summary>
    /// Get the distance of the mouse from the center. The value is clamped using min and max.
    /// </summary>
    /// <param name="min">Clamp minimum</param>
    /// <param name="max">Clamp maximum</param>
    /// <returns>Clamped distance</returns>
    private float GetClampedMouseDistanceFromCenter(float min, float max)
    {
        Vector3 mousePos = Get3DMousePos();

        float distance = Vector3.Distance(Vector3.zero, mousePos);

        return Mathf.Clamp(distance, min, max);
    }

    private Vector3 GetMouseVectorFromCenter()
    {
        return (Get3DMousePos() - Vector3.zero).normalized;
    }
}

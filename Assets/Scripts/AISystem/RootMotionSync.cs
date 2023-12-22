using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RootMotionSync : MonoBehaviour
{
    [SerializeField] Transform m_transform;
    [SerializeField] bool m_applyRotationWarp;
    Animator m_animator;

    float m_angle;
    bool m_warpEnabled = true;

    public AIMovement m_movement;

    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void SetTurnWarp(float angle)
    {
        m_angle = angle;
    }

    public void SetWarp(bool warpOn)
    {
        m_warpEnabled = warpOn;
    }

    public void SetYPos(float y)
    {
        Vector3 pos = transform.position;
        pos.y = y;
        m_transform.position = pos;
    }

    void OnAnimatorMove()
    {
        Vector3 deltaMove = m_animator.deltaPosition;
        deltaMove.y = 0f;

        Quaternion deltaRotation = m_animator.deltaRotation;

        if (m_applyRotationWarp && m_warpEnabled)
        {
            deltaRotation = Quaternion.Euler(0f, m_angle * 0.05f, 0f);
        }
        if (Vector3.Distance(deltaMove, Vector3.zero) > 1)
        {
            Debug.Log(Vector3.Distance(deltaMove, Vector3.zero) + " Was the distance");
        }

        m_transform.position += deltaMove;
        m_transform.rotation *= deltaRotation;
    }
}
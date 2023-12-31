using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

public class AIMovement
{
    const float m_arrivalThreshold = 0.9f;
    MovementSettings m_settings;
    IManager m_aiManager;
    IBeing m_attachedBeing;
    RootMotionSync m_rootMotionSync;
    Animator m_animator;

    public Path m_currentPath;

    bool m_movementEnabled;
    float m_speed = 0;

    public bool m_atDestination { get; private set; } = true;

    public AIMovement(MovementSettings settings, [CanBeNull] Animator animator, IBeing attachedBeing, IManager manager, RootMotionSync rootMotionSync)
    {
        m_settings = settings;
        m_animator = animator;
        m_attachedBeing = attachedBeing;
        m_aiManager = manager;
        m_rootMotionSync = rootMotionSync;
        m_rootMotionSync.m_movement = this;
    }

    public void EnableMovement()
    {
        m_movementEnabled = true;
        MovementLoop().Forget();
    }

    public void DisableMovement() => m_movementEnabled = false;

    public bool CanMove()
    {
        return m_movementEnabled;
    }

    public void Stop()
    {
        m_speed = 0;
        m_currentPath = Path.Empty;
        m_atDestination = true;
    }

    public void SetWarp(bool rootMotionOn) => m_rootMotionSync.SetWarp(rootMotionOn);

    public void SetDestination(Vector3 dest)
    {
        if (Vector3.SqrMagnitude(m_attachedBeing.m_position - dest) <= m_arrivalThreshold * m_arrivalThreshold)
        {
            m_atDestination = true;
            return;
        }

        Path path = m_aiManager.GeneratePath(new PathRequest()
        {
            m_destination = dest,
            m_destinationDirection = Vector3.Normalize(dest - m_attachedBeing.m_position),
            m_origin = m_attachedBeing.m_position,
            m_originDirection = m_attachedBeing.m_forward,
        });

        if (path.m_isEmpty)
        {
            return;
        }

        m_currentPath = path;
        m_atDestination = false;
    }

    async UniTask MovementLoop()
    {
        while (m_movementEnabled)
        {
            if (m_currentPath.m_isEmpty == false && m_animator != null)
            {
                if (!m_atDestination)
                {
                    UpdateForwardBackwards();
                    UpdateSideWays();
                }
                else
                {
                    m_speed = 0;
                    m_animator.SetFloat("ForwardsBackwards", m_speed);
                    m_rootMotionSync.SetTurnWarp(0);
                    m_animator.SetFloat("Sideways", 0);
                }

                float sqrmag = Vector3.SqrMagnitude(m_attachedBeing.m_position - (Vector3)m_currentPath.m_destination);
                m_atDestination = sqrmag <= m_arrivalThreshold * m_arrivalThreshold;
            }

            await UniTask.Yield();
        }
    }

    void UpdateSideWays()
    {
        m_currentPath.GetRelativePoint(m_attachedBeing.m_position, m_settings.m_distance, out float3 predictPos, out float3 predictTan);
        m_rootMotionSync.SetYPos(predictPos.y);

        predictTan.y = 0;

        float angle = Vector3.SignedAngle(m_attachedBeing.m_forward, predictTan, Vector3.up);
        m_rootMotionSync.SetTurnWarp(angle);
        m_animator.SetFloat("Sideways", angle * Mathf.Deg2Rad);
    }

    void UpdateForwardBackwards()
    {
        float distToDest = Vector3.Distance(m_currentPath.m_destination, m_attachedBeing.m_position);

        if (distToDest >= m_arrivalThreshold * m_arrivalThreshold)
        {
            m_speed = Mathf.Lerp(m_speed, m_settings.m_run, Time.deltaTime * m_settings.m_acceleration);
            float speed = math.remap(0, m_settings.m_run, 0, 1, m_speed);
            m_animator.SetFloat("ForwardsBackwards", speed);
        }
        else
        {
            m_speed = 0;
            m_animator.SetFloat("ForwardsBackwards", m_speed);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;

public class AIBeing : MonoBehaviour, IBeing
{
    public Vector3 m_position => transform.position;
    public Vector3 m_forward => transform.forward;
    public Vector3 m_headPosition => m_head.position;

    [field: SerializeField] public AISettings AISettings { get; private set; } = new();
    [SerializeField] Animator m_animator;
    [SerializeField] RootMotionSync m_rootMotionSync;
    [SerializeField] Transform m_head;

    Intelligience m_intelligience;
    AIManager m_aiManager;
    public bool m_showDebugInfo;
    public bool m_showPath;
    public float m_maxDebugDistance;

    void Start()
    {
        m_aiManager = FindObjectOfType<ExampleAIManager>().m_aiManager;
        m_aiManager.RegisterBeing(this);

        m_animator ??= GetComponentInChildren<Animator>();

        List<IOptic> optics = CreateOptics();
        AIKnowledge aIKnowledge = new AIKnowledge();
        AIMovement aIMovement = new AIMovement(AISettings.MovementSettings, m_animator, this, m_aiManager, m_rootMotionSync);
        BehaviourManager behaviourManager = new BehaviourManager(UnpackBehaviourTree(AISettings.BehaviourTree, new BehaviourInput()
        {
            m_aIKnowledge = aIKnowledge,
            m_aIMovement = aIMovement,
            m_go = gameObject,
        }));

        m_intelligience = new Intelligience(optics, aIKnowledge, aIMovement, behaviourManager);

        //For Tutorial only
        m_intelligience.EnableIntelligience();
    }

    void OnDestroy()
    {
        m_intelligience?.DisableIntelligience();
        m_aiManager?.DeregisterBeing(this);
    }

    List<IOptic> CreateOptics()
    {
        List<IOptic> createdOptics = new List<IOptic>();
        createdOptics.Add(new OpticSensor(AISettings.ObservationSettings, m_aiManager, this));

        return createdOptics;
    }

    BehaviourTree UnpackBehaviourTree(BTAsset asset, BehaviourInput input)
    {
        BTAsset BTInstance = ScriptableObject.Instantiate(asset);
        var bt = BTInstance.m_behaviourTree;
        bt.SetBehaviourInput(input);
        return bt;
    }

    public void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (m_showDebugInfo)
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                float distanceToCamera = Vector3.Distance(transform.position, sceneView.camera.transform.position);
                if (distanceToCamera <= m_maxDebugDistance)
                {
                    GUIStyle style_AIInfo = new GUIStyle();
                    style_AIInfo.normal.textColor = Color.white;
                    string AIInfo = "\nIs At Destination: " + m_intelligience.IsAtDestination() +
                        "\nMovement Enabled: " + m_intelligience.CanMove() + "\nCan See Player: " + m_intelligience.CanSeePlayer();
                    Handles.Label(transform.position + Vector3.up * 3f, AIInfo, style_AIInfo);
                }
            }
        }

        if (!m_intelligience.IsAtDestination() && m_showPath)
        {
            BezierKnot[] positions = m_intelligience.GetPath().ToArray();
            for (int i = 0; i < positions.Length; i++)
            {
                if (i == 0)
                {
                    Debug.DrawLine(transform.position, positions[0].Position, Color.red);
                }
                else
                {
                    Debug.DrawLine(positions[i - 1].Position, positions[i].Position, Color.red);
                }
            }
            Debug.DrawLine(transform.position, m_animator.deltaPosition + transform.position, Color.cyan);
            Debug.DrawLine(transform.position, (transform.rotation * Vector3.forward) + transform.position, Color.magenta);
        }
    }
}
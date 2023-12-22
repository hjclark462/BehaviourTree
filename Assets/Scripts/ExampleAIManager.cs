using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleAIManager : MonoBehaviour
{
    public AIManager m_aiManager;
    // Start is called before the first frame update
    void Awake()
    {
        m_aiManager = new AIManager();
    }
}

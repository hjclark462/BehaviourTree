using System.Collections.Generic;
using UnityEngine;

namespace AISystem.BehaviourTrees
{
    [CreateAssetMenu(menuName = "BehaviourTree/BehaviourTree", fileName = "New Behaviour Tree")]
    public class BTAsset : ScriptableObject
    {
        public BehaviourTree m_behaviourTree;
        public List<NodeData> m_nodeData;
    }
}

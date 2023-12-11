using UnityEngine;
using AISystem.BehaviourTrees;

namespace AISystem.Data
{
    [System.Serializable]
    public class AISettings
    {
        [field: SerializeField] public ObservationSettings ObservationSettings { get; set; } = new();
        [field: SerializeField] public MovementSettings MovementSettings { get; private set; }
        [field: SerializeField] public BTAsset BehaviourTree { get; private set; }
    }
}
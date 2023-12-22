using System;
using UnityEngine;
using AISystem;

    public abstract class CompositeNode : Node
    {
        [field: SerializeReference, HideInInspector] public Node[] m_children { get; set; } = Array.Empty<Node>();
        public void SetChildren(Node[] children) => m_children = children;

    }

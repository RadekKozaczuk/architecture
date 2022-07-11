using System.Collections.Generic;

namespace UnityEngine.AI
{
    [ExecuteInEditMode]
    [AddComponentMenu("Navigation/NavMeshModifierVolume", 31)]
    [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
    public class NavMeshModifierVolume : MonoBehaviour
    {
        public static List<NavMeshModifierVolume> ActiveModifiers { get; } = new();

        public Vector3 Size { get => _size; set => _size = value; }
        [SerializeField]
        Vector3 _size = new(4.0f, 3.0f, 4.0f);

        public Vector3 Center { get => _center; set => _center = value; }
        [SerializeField]
        Vector3 _center = new(0, 1.0f, 0);

        public int Area { get => _area; set => _area = value; }
        [SerializeField]
        int _area;

        // List of agent types the modifier is applied for.
        // Special values: empty == None, m_AffectedAgents[0] =-1 == All.
        [SerializeField]
        List<int> _affectedAgents = new(new[] {-1}); // Default value is All

        void OnEnable()
        {
            if (!ActiveModifiers.Contains(this))
                ActiveModifiers.Add(this);
        }

        void OnDisable() => ActiveModifiers.Remove(this);

        public bool AffectsAgentType(int agentTypeID)
        {
            if (_affectedAgents.Count == 0)
                return false;
            if (_affectedAgents[0] == -1)
                return true;

            return _affectedAgents.IndexOf(agentTypeID) != -1;
        }
    }
}
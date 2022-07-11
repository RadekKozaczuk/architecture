using System.Collections.Generic;

namespace UnityEngine.AI
{
    [ExecuteInEditMode]
    [AddComponentMenu("Navigation/NavMeshModifier", 32)]
    [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
    public class NavMeshModifier : MonoBehaviour
    {
        public static List<NavMeshModifier> ActiveModifiers { get; } = new();

        public bool OverrideArea { get => _overrideArea; set => _overrideArea = value; }
        [SerializeField]
        bool _overrideArea;

        public int Area { get => _area; set => _area = value; }
        [SerializeField]
        int _area;

        public bool IgnoreFromBuild { get => _ignoreFromBuild; set => _ignoreFromBuild = value; }
        [SerializeField]
        bool _ignoreFromBuild;

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
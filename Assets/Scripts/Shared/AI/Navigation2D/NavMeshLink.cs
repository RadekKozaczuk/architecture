using System.Collections.Generic;

namespace UnityEngine.AI
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(-101)]
    [AddComponentMenu("Navigation/NavMeshLink", 33)]
    [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
    public class NavMeshLink : MonoBehaviour
    {
        public int AgentTypeID
        {
            get => _agentTypeID;
            set
            {
                _agentTypeID = value;
                UpdateLink();
            }
        }
        [SerializeField]
        int _agentTypeID;

        public Vector3 StartPoint
        {
            get => _startPoint;
            set
            {
                _startPoint = value;
                UpdateLink();
            }
        }
        [SerializeField]
        Vector3 _startPoint = new(0.0f, 0.0f, -2.5f);

        public Vector3 EndPoint
        {
            get => _endPoint;
            set
            {
                _endPoint = value;
                UpdateLink();
            }
        }
        [SerializeField]
        Vector3 _endPoint = new(0.0f, 0.0f, 2.5f);

        public float Width
        {
            get => _width;
            set
            {
                _width = value;
                UpdateLink();
            }
        }
        [SerializeField]
        float _width;

        public int CostModifier
        {
            get => _costModifier;
            set
            {
                _costModifier = value;
                UpdateLink();
            }
        }
        [SerializeField]
        int _costModifier = -1;

        public bool Bidirectional
        {
            get => _bidirectional;
            set
            {
                _bidirectional = value;
                UpdateLink();
            }
        }
        [SerializeField]
        bool _bidirectional = true;

        public bool AutoUpdate { get => _autoUpdatePosition; set => SetAutoUpdate(value); }
        [SerializeField]
        bool _autoUpdatePosition;

        public int Area
        {
            get => _area;
            set
            {
                _area = value;
                UpdateLink();
            }
        }
        [SerializeField]
        int _area;

        NavMeshLinkInstance _mLinkInstance;

        Vector3 _mLastPosition = Vector3.zero;
        Quaternion _mLastRotation = Quaternion.identity;

        static readonly List<NavMeshLink> _sTracked = new();

        void OnEnable()
        {
            AddLink();
            if (_autoUpdatePosition && _mLinkInstance.valid)
                AddTracking(this);
        }

        void OnDisable()
        {
            RemoveTracking(this);
            _mLinkInstance.Remove();
        }

        public void UpdateLink()
        {
            _mLinkInstance.Remove();
            AddLink();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            _width = Mathf.Max(0.0f, _width);

            if (!_mLinkInstance.valid)
                return;

            UpdateLink();

            if (!_autoUpdatePosition)
                RemoveTracking(this);
            else if (!_sTracked.Contains(this))
                AddTracking(this);
        }
#endif

        void OnDidApplyAnimationProperties() => UpdateLink();

        static void AddTracking(NavMeshLink link)
        {
#if UNITY_EDITOR
            if (_sTracked.Contains(link))
            {
                Debug.LogError("Link is already tracked: " + link);
                return;
            }
#endif

            if (_sTracked.Count == 0)
                NavMesh.onPreUpdate += UpdateTrackedInstances;

            _sTracked.Add(link);
        }

        static void RemoveTracking(NavMeshLink link)
        {
            _sTracked.Remove(link);

            if (_sTracked.Count == 0)
                NavMesh.onPreUpdate -= UpdateTrackedInstances;
        }

        void SetAutoUpdate(bool value)
        {
            if (_autoUpdatePosition == value)
                return;

            _autoUpdatePosition = value;
            if (value)
                AddTracking(this);
            else
                RemoveTracking(this);
        }

        void AddLink()
        {
#if UNITY_EDITOR
            if (_mLinkInstance.valid)
            {
                Debug.LogError("Link is already added: " + this);
                return;
            }
#endif

            var link = new NavMeshLinkData
            {
                startPosition = _startPoint,
                endPosition = _endPoint,
                width = _width,
                costModifier = _costModifier,
                bidirectional = _bidirectional,
                area = _area,
                agentTypeID = _agentTypeID
            };

            Transform tran = transform;
            _mLinkInstance = NavMesh.AddLink(link, tran.position, tran.rotation);
            if (_mLinkInstance.valid)
                _mLinkInstance.owner = this;

            _mLastPosition = tran.position;
            _mLastRotation = tran.rotation;
        }

        bool HasTransformChanged()
        {
            if (_mLastPosition != transform.position)
                return true;

            return _mLastRotation != transform.rotation;
        }

        static void UpdateTrackedInstances()
        {
            foreach (NavMeshLink instance in _sTracked)
                if (instance.HasTransformChanged())
                    instance.UpdateLink();
        }
    }
}
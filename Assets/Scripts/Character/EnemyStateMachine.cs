using UnityEngine;

namespace AppleGrapple
{
    [RequireComponent(typeof(CharacterMovementController), typeof(Health), typeof(CharacterIdentity))]
    [RequireComponent(typeof(SwordOrigin))]
    public class EnemyStateMachine : MonoBehaviour, IInputProvider
    {
        [SerializeField] private float _attackDistance = 1.5f;
        [SerializeField] private float _enemySearchRadius = 8f;
        [SerializeField, Range(0f, 1f)] private float _lowHealthThreshold = 0.3f;
        [SerializeField] private float _decisionInterval = 0.25f;
        [SerializeField] private float _roamArrivalDistance = 0.25f;
        [SerializeField] private float _roamMinimumDistance = 3f;
        [SerializeField] private MapConfig _mapConfig;
        [SerializeField] private CharacterConfig _characterConfig;
        private const float RoamBoundaryMargin = 5f;

        private CharacterMovementController _movementController;
        private Health _health;
        private CharacterIdentity _identity;
        private SwordOrigin _swordOrigin;
        private EnemyStateBase _currentState;
        private Vector2 _movementInput;
        private float _decisionTimer;

    #if UNITY_EDITOR
        [SerializeField] private string _debugCurrentState;
        [SerializeField] private string _debugLastDecision;
        [SerializeField] private string _debugTarget;
        [SerializeField] private int _debugSwordCount;
        [SerializeField] private int _debugTargetSwordCount;
        [SerializeField] private float _debugHealthPercent;
        [SerializeField] private Vector2 _debugMovementInput;
    #endif

        internal CharacterMovementController MovementController => _movementController;
        internal Health Health => _health;
        internal CharacterIdentity Identity => _identity;
        internal float AttackDistance => _attackDistance;
        internal float EnemySearchRadius => _enemySearchRadius;
        internal float LowHealthThreshold => _lowHealthThreshold;
        internal float AiSmartValue => _characterConfig.aiSmartValue;
        internal SwordOrigin SwordOrigin => _swordOrigin;
        internal float DecisionInterval => _decisionInterval;
        internal float RoamArrivalDistance => _roamArrivalDistance;
        internal float RoamStoppingDistance => Mathf.Max(_roamArrivalDistance, _characterConfig.movementSpeed * _decisionInterval);
        internal MapConfig MapConfig => _mapConfig;
        internal Transform Target { get; private set; }
        internal EnemyStateBase InitialState { get; private set; }
        internal EnemyStateBase SearchForEnemyState { get; private set; }
        internal EnemyStateBase SearchForSwordState { get; private set; }
        internal EnemyStateBase AttackState { get; private set; }
        internal EnemyStateBase IdleState { get; private set; }
        internal EnemyStateBase DeadState { get; private set; }

        public bool IsDragging => false;
        public EnemyStateBase CurrentState => _currentState;

        private void Awake()
        {
            _movementController = GetComponent<CharacterMovementController>();
            _health = GetComponent<Health>();
            _identity = GetComponent<CharacterIdentity>();
            _swordOrigin = GetComponent<SwordOrigin>();

            InitialState = new EnemyInitialState(this);
            SearchForEnemyState = new EnemySearchForEnemyState(this);
            SearchForSwordState = new EnemySearchForSwordState(this);
            AttackState = new EnemyAttackState(this);
            IdleState = new EnemyIdleState(this);
            DeadState = new EnemyDeadState(this);

            _health.Died += HandleDied;
            _health.Damaged += HandleDamaged;
        }

        private void Start()
        {
            _movementController.SetInputProvider(this);
            ChangeState(InitialState);
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.Died -= HandleDied;
                _health.Damaged -= HandleDamaged;
            }

            _currentState?.Exit();
        }

        public void UpdateInput()
        {
            _decisionTimer -= Time.deltaTime;
            if (_decisionTimer <= 0f)
            {
                _decisionTimer = _decisionInterval;
                _currentState?.Update();
            }

#if UNITY_EDITOR
            RefreshDebugMonitor();
#endif
        }

        public Vector2 GetMovementInput()
        {
            return _movementInput;
        }

        public void ChangeState(EnemyStateBase nextState)
        {
            if (nextState == null || nextState == _currentState)
                return;

            var previousStateName = _currentState?.GetType().Name ?? "None";
            _currentState?.Exit();
            _currentState = nextState;
            _movementInput = Vector2.zero;
            _currentState.Enter();
            Debug.Log($"[AI:{name}] State: {previousStateName} -> {_currentState.GetType().Name}", this);
        }

        internal void SetMovementInput(Vector2 movementInput)
        {
            _movementInput = Vector2.ClampMagnitude(movementInput, 1f);
        }

        internal void SetDebugDecision(string decision)
        {
#if UNITY_EDITOR
            _debugLastDecision = decision;
#endif
            Debug.Log($"[AI:{name}] Decision: {decision}", this);
        }

#if UNITY_EDITOR
        private void RefreshDebugMonitor()
        {
            _debugCurrentState = _currentState?.GetType().Name ?? "None";
            _debugTarget = Target != null ? Target.name : "None";
            _debugSwordCount = _swordOrigin != null ? _swordOrigin.SwordCount : 0;
            var targetSwordOrigin = Target != null ? Target.GetComponent<SwordOrigin>() : null;
            _debugTargetSwordCount = targetSwordOrigin != null ? targetSwordOrigin.SwordCount : 0;
            _debugHealthPercent = _health != null ? _health.HealthPercent : 0f;
            _debugMovementInput = _movementInput;
        }
#endif

        internal void SetTarget(Transform target)
        {
            Target = target;
        }

        internal bool HasValidTarget()
        {
            if (Target == null || !Target.gameObject.activeInHierarchy)
                return false;

            var targetHealth = Target.GetComponent<Health>();
            return targetHealth != null && !targetHealth.IsDead;
        }

        internal Transform FindClosestEnemy()
        {
            var identities = Object.FindObjectsByType<CharacterIdentity>(FindObjectsSortMode.None);
            Transform closest = null;
            var closestDistance = float.MaxValue;

            foreach (var identity in identities)
            {
                if (identity == _identity)
                    continue;

                var targetHealth = identity.GetComponent<Health>();
                if (targetHealth == null || targetHealth.IsDead)
                    continue;

                var distance = (identity.transform.position - transform.position).sqrMagnitude;
                if (distance > _enemySearchRadius * _enemySearchRadius)
                    continue;

                if (distance < closestDistance)
                {
                    closest = identity.transform;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        internal SwordPickup FindClosestSwordPickup()
        {
            var pickups = Object.FindObjectsByType<SwordPickup>(FindObjectsSortMode.None);
            SwordPickup closest = null;
            var closestDistance = float.MaxValue;

            foreach (var pickup in pickups)
            {
                if (pickup.IsReservedByOther(this))
                    continue;

                var distance = (pickup.transform.position - transform.position).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closest = pickup;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        private void HandleDied()
        {
            ChangeState(DeadState);
        }

        private void HandleDamaged(HitInfo hitInfo)
        {
            if (_currentState != DeadState && _currentState != SearchForSwordState)
            {
                SetDebugDecision("Damaged: searching for sword");
                ChangeState(SearchForSwordState);
            }
        }

        internal Vector2 GetRandomRoamPosition()
        {
            if (_mapConfig == null)
                return transform.position;

            var boundaryOffset = _mapConfig.boundarySettings.offset;
            var halfWidth = Mathf.Max(0f,
                (_mapConfig.GridX * 0.5f + 0.5f) * _mapConfig.tileWorldSize
                - boundaryOffset.x
                - RoamBoundaryMargin);
            var halfHeight = Mathf.Max(0f,
                (_mapConfig.GridY * 0.5f + 0.5f) * _mapConfig.tileWorldSize
                - boundaryOffset.y
                - RoamBoundaryMargin);
            var currentPosition = (Vector2)transform.position;

            for (var attempt = 0; attempt < 12; attempt++)
            {
                var candidate = new Vector2(
                    Random.Range(-halfWidth, halfWidth),
                    Random.Range(-halfHeight, halfHeight));

                if ((candidate - currentPosition).sqrMagnitude >= _roamMinimumDistance * _roamMinimumDistance)
                    return candidate;
            }

            return new Vector2(
                Mathf.Clamp(currentPosition.x, -halfWidth, halfWidth),
                Mathf.Clamp(currentPosition.y, -halfHeight, halfHeight));
        }
    }
}

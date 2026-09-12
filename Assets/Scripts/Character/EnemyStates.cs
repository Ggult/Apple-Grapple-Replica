using UnityEngine;

namespace AppleGrapple
{
    public sealed class EnemyInitialState : EnemyStateBase
    {
        public EnemyInitialState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter() { }

        public override void Update()
        {
            if (StateMachine.SwordOrigin.SwordCount > 0)
            {
                StateMachine.SetDebugDecision("Has sword: searching for an enemy");
                StateMachine.ChangeState(StateMachine.SearchForEnemyState);
                return;
            }

            StateMachine.SetDebugDecision("No sword: searching for a sword pickup");
            StateMachine.ChangeState(StateMachine.SearchForSwordState);
        }

        public override void Exit() { }
    }

    public sealed class EnemySearchForEnemyState : EnemyStateBase
    {
        public EnemySearchForEnemyState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            StateMachine.SetMovementInput(Vector2.zero);
        }

        public override void Update()
        {
            var target = StateMachine.FindClosestEnemy();
            if (target == null)
            {
                StateMachine.SetDebugDecision("No enemy in search radius: searching for sword");
                StateMachine.ChangeState(StateMachine.SearchForSwordState);
                return;
            }

            StateMachine.SetTarget(target);
            StateMachine.SetDebugDecision($"Target selected: {target.name}");
            StateMachine.ChangeState(StateMachine.AttackState);
        }

        public override void Exit() { }
    }

    public sealed class EnemySearchForSwordState : EnemyStateBase
    {
        private SwordPickup _targetPickup;

        public EnemySearchForSwordState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            StateMachine.SetMovementInput(Vector2.zero);
            _targetPickup = StateMachine.FindClosestSwordPickup();
            _targetPickup?.TryReserve(StateMachine);
        }

        public override void Update()
        {
            if (_targetPickup == null)
            {
                StateMachine.SetDebugDecision("No sword pickup available: roaming");
                StateMachine.ChangeState(StateMachine.IdleState);
                return;
            }

            if (!_targetPickup.gameObject.activeInHierarchy)
            {
                StateMachine.SetDebugDecision(StateMachine.SwordOrigin.SwordCount > 0
                    ? "Pickup collected: searching for an enemy"
                    : "No sword pickup available: idling");
                StateMachine.ChangeState(StateMachine.SwordOrigin.SwordCount > 0
                    ? StateMachine.SearchForEnemyState
                    : StateMachine.IdleState);
                return;
            }

            if (_targetPickup.IsReservedByOther(StateMachine))
            {
                StateMachine.SetDebugDecision("Pickup reserved by another AI: searching again");
                StateMachine.ChangeState(StateMachine.SearchForSwordState);
                return;
            }

            var direction = (_targetPickup.transform.position - StateMachine.transform.position).normalized;
            StateMachine.SetMovementInput(direction);

            if ((_targetPickup.transform.position - StateMachine.transform.position).sqrMagnitude <= StateMachine.AttackDistance * StateMachine.AttackDistance)
            {
                if (!StateMachine.SwordOrigin || StateMachine.SwordOrigin.SwordCount > 0)
                    StateMachine.ChangeState(StateMachine.AttackState);
            }
        }

        public override void Exit()
        {
            _targetPickup?.ReleaseReservation(StateMachine);
            _targetPickup = null;
            StateMachine.SetMovementInput(Vector2.zero);
        }
    }

    public sealed class EnemyAttackState : EnemyStateBase
    {
        public EnemyAttackState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter() { }

        public override void Update()
        {
            if (StateMachine.Health.HealthPercent <= StateMachine.LowHealthThreshold
                || StateMachine.SwordOrigin.SwordCount == 0)
            {
                StateMachine.SetDebugDecision("Low health or no sword: searching for sword");
                StateMachine.ChangeState(StateMachine.SearchForSwordState);
                return;
            }

            if (!StateMachine.HasValidTarget())
            {
                StateMachine.SetTarget(null);
                StateMachine.SetDebugDecision("Target lost: returning to initial decision");
                StateMachine.ChangeState(StateMachine.InitialState);
                return;
            }

            if (ShouldAvoidTarget())
            {
                StateMachine.SetDebugDecision("Target has more swords: searching for sword");
                StateMachine.ChangeState(StateMachine.SearchForSwordState);
                return;
            }

            var offset = StateMachine.Target.position - StateMachine.transform.position;
            if (offset.sqrMagnitude <= StateMachine.AttackDistance * StateMachine.AttackDistance)
            {
                StateMachine.SetMovementInput(Vector2.zero);
                return;
            }

            StateMachine.SetMovementInput(offset.normalized);
        }

        private bool ShouldAvoidTarget()
        {
            if (!StateMachine.HasValidTarget() || StateMachine.SwordOrigin.SwordCount == 0)
                return false;

            var targetSwordOrigin = StateMachine.Target.GetComponent<SwordOrigin>();
            if (targetSwordOrigin == null || targetSwordOrigin.SwordCount <= StateMachine.SwordOrigin.SwordCount)
                return false;

            return Random.value <= StateMachine.AiSmartValue;
        }

        public override void Exit()
        {
            StateMachine.SetMovementInput(Vector2.zero);
        }
    }

    public sealed class EnemyIdleState : EnemyStateBase
    {
        private Vector2 _roamTarget;

        public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            _roamTarget = StateMachine.GetRandomRoamPosition();
            StateMachine.SetDebugDecision($"No immediate objective: roaming to {_roamTarget}");
        }

        public override void Update()
        {
            if (StateMachine.SwordOrigin.SwordCount == 0)
            {
                StateMachine.SetDebugDecision("No sword while roaming: searching for sword");
                StateMachine.ChangeState(StateMachine.SearchForSwordState);
                return;
            }

            var offset = _roamTarget - (Vector2)StateMachine.transform.position;
            var stoppingDistance = StateMachine.RoamStoppingDistance;
            if (offset.sqrMagnitude <= stoppingDistance * stoppingDistance)
            {
                StateMachine.SetDebugDecision("Reached roam point: searching for sword");
                StateMachine.ChangeState(StateMachine.SearchForSwordState);
                return;
            }

            StateMachine.SetMovementInput(offset.normalized);
        }

        public override void Exit()
        {
            StateMachine.SetMovementInput(Vector2.zero);
        }
    }

    public sealed class EnemyDeadState : EnemyStateBase
    {
        public EnemyDeadState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            StateMachine.SetMovementInput(Vector2.zero);
        }

        public override void Update() { }
        public override void Exit() { }
    }
}

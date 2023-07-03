using UnityEngine;

public class Cop : Spaceship
{
    [SerializeField] private float _searchTime;
    private float _searchTimeLeft;

    CopAttackSystem _attackSystem => GetComponent<CopAttackSystem>();

    [Header("Icon")]

    [SerializeField] private GameObject _searchOn;
    [SerializeField] private GameObject _attackOn;
    [SerializeField] private GameObject _retreatOn;

    private new void Start()
    {
        base.Start();
    }

    public void Spawn(Vector3 location)
    {
        _agent.SetDestination(location);
        _currentState = SearchState;
        _searchTimeLeft = _searchTime;
        _attackOn.SetActive(false);
        _searchOn.SetActive(true);
        _retreatOn.SetActive(false);
    }


    private void SearchState()
    {
        if (_searchTimeLeft > 0) 
        {
            _searchTimeLeft -= Time.deltaTime; 
        }
        else
        {
            _agent.SetDestination(_gm.PoliceStation.position);
            _currentState = RetreatState; return;
        }

        FlyAroundRandomly();
        Scan();
        FixStuck();
    }

    private void AttackState()
    {
        if (_target == null)
        {
            LoseTarget();
            return;
        }

        if (!_target.isActiveAndEnabled)
        {
            LoseTarget();
            return;
        }

        if ((_target as Pirate).IsTargetable())
        {
            _agent.SetDestination(_target.transform.position);
            _attackSystem.ShootingTarget();
        }
        else
        {
            LoseTarget();
            return;
        }
        
    }

    private void RetreatState()
    {
        if (_agent.remainingDistance <= 1)
        { 
            gameObject.SetActive(false);
        }
        Scan();
    }

    private void LoseTarget()
    {
        _target = null;
        _agent.SetDestination(_gm.PoliceStation.position);
        _currentState = RetreatState;
        _attackOn.SetActive(false);
        _searchOn.SetActive(false);
        _retreatOn.SetActive(true);
    }

    protected override void Scan()
    {
        // Timer
        base.Scan();
        if (_scanCD > 0) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _scanRadius, _scanableLayers);
        foreach (Collider2D collider in colliders)
        {
            if (collider.transform.TryGetComponent<Pirate>(out Pirate p))
            {
                if (p.IsAttacking())
                {
                    if (CheckIfInFront(new Vector2(p.transform.position.x, p.transform.position.y)))
                    {
                        _target = p;
                        _currentState = AttackState;
                        _attackOn.SetActive(true);
                        _searchOn.SetActive(false);
                        _retreatOn.SetActive(false);
                        return;
                    }
                }
            }
        }
    }

    public void Agro(Transform attacker)
    {
        if (attacker.TryGetComponent<Spaceship>(out _target))
        {
            _currentState = AttackState;
            _attackOn.SetActive(true);
            _searchOn.SetActive(false);
            _retreatOn.SetActive(false);
        }
    }
}

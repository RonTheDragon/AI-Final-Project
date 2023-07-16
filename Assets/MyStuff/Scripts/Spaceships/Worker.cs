using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Worker : Spaceship
{
    private int _minerals;
    

    [Header("Panic")]
    [SerializeField] private float _panicDuration = 10;
    private float _panicTimeLeft;
    [SerializeField] private float _callThePoliceCooldown = 5;
    private float _callThePoliceCD;
    [SerializeField] private float _panicSpeedMult = 2f;

    [Header("Icon")]
    [SerializeField] private GameObject _mineralOff;
    [SerializeField] private GameObject _mineralOn;
    [SerializeField] private GameObject _underAttack;

    private SpriteRenderer _noteResource => _mineralOn.GetComponent<SpriteRenderer>();
    private Sprite _noteSprite => _noteResource.sprite;

    private int _miningLVL;
    [SerializeField] private int _miningLvlEffect=2;

    private new void Start()
    {
        base.Start();
        _startingSpeed = _agent.speed;
        _startingAccel = _agent.acceleration;
        _startingRot = _agent.angularSpeed;
        Spawn();
    }

    #region States
    private void WorkState()
    {
        if (Vector2.Distance(transform.position, _agent.destination) < 1)
        {
            _agent.enabled = false;
            _agent.enabled = true;
            if (_minerals > 0)
            {
                SellMinerals();
            }
            else
            {
                MineMinerals();
            }
        }
    }

    private void PanicState()
    {
        if (_attacker == null)
        {
            StopPanic();
            return;
        }

        if (_callThePoliceCD > 0)
        {
            _callThePoliceCD -= Time.deltaTime;
        }

        if (_panicTimeLeft > 0)
        {
            _panicTimeLeft -= Time.deltaTime;
        }
        else
        {
            StopPanic();
            return;
        }
        RunAwayFromAttacker();
    }
    #endregion

    private void MineMinerals()
    {
        _minerals = (int)UnityEngine.Random.Range(_gm.MineralsExtraction.x+(_miningLVL-1* _miningLvlEffect),
            _gm.MineralsExtraction.y + (_miningLVL - 1 * _miningLvlEffect));
        _agent.SetDestination(_gm.Factory.position);
        _agent.speed = _currentSpeed * 0.5f;
        _mineralOff.SetActive(false);
        _mineralOn.SetActive(true);
        StartCoroutine(DisplayNote(0.0f, $"+{_minerals} Mined", Color.cyan, _gm.MineralIcon));
    }

    private void SellMinerals()
    {
        int currencyAdded = _minerals * 40;
        _currency += currencyAdded;
        StartCoroutine(DisplayNote(0.0f, $"-{_minerals} Sold", Color.cyan, _gm.MineralIcon));
        StartCoroutine(DisplayNote(0.2f, $"{_currency-currencyAdded}+{currencyAdded}={_currency}$",Color.yellow,_gm.CurrencyIcon));
        _minerals = 0;
        WorkerShop();
        _agent.speed = _currentSpeed;
        _agent.SetDestination(_gm.Mine.position);
        _mineralOff.SetActive(true);
        _mineralOn.SetActive(false);
    }

    private void WorkerShop()
    {
        _health.Heal();
        if (_currency < _upgradePrice) 
            return;

        _currency-= _upgradePrice;
        StartCoroutine(DisplayNote(0.6f, $"{_currency + _upgradePrice}-{_upgradePrice}={_currency}$", Color.yellow, _gm.CurrencyIcon));
        int chosenUpgrade = UnityEngine.Random.Range(0, 3);
        switch (chosenUpgrade)
        {
            case 0: BuySpeed(); break; 
            case 1: BuyMining(); break;
            case 2: BuyArmor(); break;
        }
    }

    private void BuyMining()
    {
        _miningLVL++;
        StartCoroutine(DisplayNote(0.4f, $"MineUp! {_miningLVL - 1}>>{_miningLVL}", Color.blue, _gm.MineIcon));
        
    }



    public void Panic(Transform attacker)
    {
        _attacker = attacker;
        _panicTimeLeft = _panicDuration;
        _currentState = PanicState;
        _underAttack.SetActive(true);
        _agent.speed = _currentSpeed  * (_minerals > 0 ? 0.5f : 1) * _panicSpeedMult;
        if (_callThePoliceCD <= 0) { CallThePolice(); _callThePoliceCD = _callThePoliceCooldown; }
    }

    private void CallThePolice()
    {
        if (_attacker == null)
        {
            return; 
        }
        _gm.CallThePoliceToLocation(_attacker.transform.position);
    }


    private void StopPanic()
    {
        _attacker = null;
        _underAttack.SetActive(false);
        _currentState = WorkState;

        if (_gm == null) return;

        if (_minerals > 0)
        {
            _agent.SetDestination(_gm.Factory.position);
        }
        else
        {
            _agent.SetDestination(_gm.Mine.position);
        }
    }

    public override void Spawn()
    {
        base.Spawn();
        StopPanic();
        _miningLVL = 1;
        _gm = GameManager.Instance;
        _minerals = 0;
        _agent.SetDestination(_gm.Mine.position);
    }

    public int GetMinerals()
    {
        return _minerals;
    }

}

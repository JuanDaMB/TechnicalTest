using System;
using Model;
using UnityEngine;
using Values;
using View;
using Random = UnityEngine.Random;

namespace Controller
{
    public interface IEnemyController
    { 
        event EventHandler<ActionEventArgs> EnemyAttack;
        event EventHandler EnemyTurnEnd;
        event EventHandler EnemyFainted;
        public void Initialize(EnemyValues values);
        public void TakeAction(ActionEventArgs e);
        public void StartTurn();
    }
 
    public class EnemyController : IEnemyController
    {
        private readonly IEnemyModel _model;
        private readonly IEnemyView _view;
        public event EventHandler<ActionEventArgs> EnemyAttack;
        public event EventHandler EnemyTurnEnd;
        public event EventHandler EnemyFainted;

        private float _attackChance;
        private float _defenseChance;
        
        public EnemyController(IEnemyModel model, IEnemyView view)
        {
            _model = model;
            _view = view;
 
            model.OnHealthChanged += ChangeHealth;
            model.OnDefend += OnDefend;
            model.OnWeaken += OnWeaken;
            model.OnVulnerable += OnVulnerable;
            model.OnDead += EnemyDead;
        }

        private void EnemyDead(object sender, EventArgs e)
        {
            _view.IsAlive = _model.Health != 0;
            EnemyFainted?.Invoke(this, EventArgs.Empty);
        }

        public void Initialize(EnemyValues values)
        {
            _model.MaxHealt = values.maxHealth;
            _model.Health = values.health;
            _model.AttackPotency = values.attackPotency;
            _model.ShieldPotency = values.shieldPotency;
            _model.SpellPotency = values.spellPotency;
            _model.StatusPotency = values.statusPotency;

            _attackChance = values.attackChance; 
            _defenseChance = values.defenseChance;

            _model.VulnerableTurns = 0;
            _model.WeakenTurns = 0;

            _view.IsAlive = _model.Health != 0;
            _view.Sprite = values.sprite;
            DecideTurn();
        }

        public void TakeAction(ActionEventArgs e)
        {
            switch (e.Type)
            {
                case ActionType.Attack:
                    if (_model.VulnerableTurns > 0)
                    {
                        e.Potency += e.Potency / 2;
                    }
                    if (e.Potency > _model.Shield)
                    {
                        _model.Health -= e.Potency - _model.Shield;
                        _model.Shield = 0;
                    }
                    else
                    {
                        _model.Shield -= e.Potency;
                    }
                    break;
                case ActionType.Defense:
                    _model.Shield += e.Potency+_model.ShieldPotency;
                    break;
                case ActionType.Spell:
                    switch (e.SpellType)
                    {
                        case SpellType.Vulnerable:
                            _model.VulnerableTurns += e.Potency;
                            break;
                        case SpellType.Weak:
                            _model.WeakenTurns += e.Potency;
                            break;
                        case SpellType.Heal:
                            _model.Health += e.Potency;
                            break;
                    }
                    break;
            }
        }

        private void OnDefend(object sender, EventArgs e)
        {
            _view.Shield = _model.Shield;
        }

        public void StartTurn()
        {
            _model.Shield = 0;
            ActionEventArgs args = new ActionEventArgs {Type = _model.Type, SpellType = _model.SpellType};
            switch (_model.Type)
            {
                case ActionType.Attack:
                    args.Self = false;
                    args.Potency = _model.AttackPotency;
                    if (_model.WeakenTurns > 0)
                    {
                        args.Potency -= args.Potency / 2;
                    }
                    CallAction(args);
                    break;
                case ActionType.Defense:
                    _model.Shield += _model.ShieldPotency;
                    break;
                case ActionType.Spell:
                    switch (_model.SpellType)
                    {
                        case SpellType.Vulnerable:
                            args.Self = false;
                            args.Potency = _model.StatusPotency;
                            CallAction(args);
                            break;
                        case SpellType.Weak:
                            args.Self = false;
                            args.Potency = _model.StatusPotency;
                            CallAction(args);
                            break;
                        case SpellType.Heal:
                            _model.Health += _model.SpellPotency;
                            break;
                    }
                    break;
            }
            EndTurn();
        }

        private void EndTurn()
        {
            if (_model.VulnerableTurns >0)
            {
                _model.VulnerableTurns--;   
            }

            if (_model.WeakenTurns > 0)
            {
                _model.WeakenTurns--;   
            }
            DecideTurn();
            EnemyTurnEnd?.Invoke(this,EventArgs.Empty);
        }

        private void OnVulnerable(object sender, EventArgs e)
        {
            _view.Vulnerable = _model.VulnerableTurns;
        }

        private void OnWeaken(object sender, EventArgs e)
        {
            _view.Weaken = _model.WeakenTurns;
            UpdateDecision();
        }

        private void DecideTurn()
        {
            bool chosen = false;
            float c1 = Random.Range(0f, 100f);
            if (c1 > 0 && c1 <= _attackChance)
            {
                _model.Type = ActionType.Attack;
            }
            else if (c1> _attackChance && c1 <= _defenseChance)
            {
                _model.Type = ActionType.Defense;
            }
            else
            {
                _model.Type = ActionType.Spell;
                _model.SpellType = (SpellType) Random.Range(0,3);
            }
            while (!chosen)
            {
                if (_model.Type == ActionType.Spell && _model.SpellType == SpellType.Heal && _model.Health == _model.MaxHealt)
                {
                    c1 = Random.Range(0f, 100f);
                    if (c1 > 0 && c1 <= _attackChance)
                    {
                        _model.Type = ActionType.Attack;
                    }
                    else if (c1> _attackChance && c1 <= _defenseChance)
                    {
                        _model.Type = ActionType.Defense;
                    }
                    else
                    {
                        _model.Type = ActionType.Spell;
                        _model.SpellType = (SpellType) Random.Range(0,3);
                    }
                }
                else
                {
                    chosen = true;
                }
            }
            UpdateDecision();
        }

        private void UpdateDecision()
        {
            switch (_model.Type)
            {
                case ActionType.Attack:
                    int e = _model.AttackPotency;
                    if (_model.WeakenTurns > 0)
                    {
                        e -= e / 2;
                    }
                    _view.TurnIntention = "Attack: " + e;
                    break;
                case ActionType.Defense:
                    _view.TurnIntention = "Defend: " + _model.ShieldPotency;
                    break;
                case ActionType.Spell:
                    switch (_model.SpellType)
                    {
                        case SpellType.Vulnerable:
                            _view.TurnIntention = "Vulnerable " + _model.StatusPotency;
                            break;
                        case SpellType.Weak:
                            _view.TurnIntention = "Weak " + _model.StatusPotency;
                            break;
                        case SpellType.Heal:
                            _view.TurnIntention = "Heal " + _model.SpellPotency;
                            break;
                    }
                    break;
            }
        }
 
        private void CallAction(ActionEventArgs e)
        {
            OnEnemyAttack(e);
        }

        private void ChangeHealth(object sender, EnemyHealthChangedEventArgs e)
        {
            _view.Health = _model.Health+"/"+_model.MaxHealt;
            _view.HealthPercent = (float)_model.Health / _model.MaxHealt;
        }
 
        protected virtual void OnEnemyAttack(ActionEventArgs e)
        {
            EnemyAttack?.Invoke(this, e);
        }
    }
}
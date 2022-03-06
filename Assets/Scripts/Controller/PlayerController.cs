using System;
using Model;
using Values;
using View;
using Random = System.Random;

namespace Controller
{
    public interface IPlayerController
    { 
        event EventHandler<int> PlayerAttack;
        public event EventHandler PlayerManaChanged;
        public void Initialize(PlayerValues values);
        public void TakeAction(ActionEventArgs e);
        public int Mana(int e = 0);
        public void ResetShield();
        public bool IsWeaken();
    }
 
    public class PlayerController : IPlayerController
    {
        private readonly IPlayerModel _model;
        private readonly IPlayerView _view;

        public event EventHandler<int> PlayerAttack;
        public event EventHandler PlayerManaChanged;
        
        public PlayerController(IPlayerModel model, IPlayerView view)
        {
            _model = model;
            _view = view;
 
            model.OnHealthChanged += ChangeHealth;
            model.OnManaChanged += OnManaChanged;
            model.OnWeaken += OnWeaken;
            model.OnVulnerable += OnVulnerable;
            model.OnDefend += OnDefend;
        }

        public void Initialize(PlayerValues values)
        {
            _model.MaxHealt = values.maxHealth;
            _model.Health = values.health;
            _model.Mana = values.mana;
            _model.AttackPotency = values.attackPotency;
            _model.ShieldPotency = values.shieldPotency;
            _model.SpellPotency = values.spellPotency;
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

        public int Mana(int e)
        {
            _model.Mana += e;
            return _model.Mana;
        }

        public void ResetShield()
        {
            _model.Shield = 0;
        }

        public bool IsWeaken()
        {
            return _model.WeakenTurns > 0;
        }

        private void OnManaChanged(object sender, EventArgs e)
        {
            _view.Mana = _model.Mana.ToString();
            PlayerManaChanged?.Invoke(this,EventArgs.Empty);
        }

        private void OnDefend(object sender, int e)
        {
            _view.Shield = _model.Shield;
        }

        private void OnVulnerable(object sender, EventArgs e)
        {
            _view.Vulnerable = _model.VulnerableTurns;
        }

        private void OnWeaken(object sender, EventArgs e)
        {
            _view.Weaken = _model.WeakenTurns;
        }
        private void DoDamage(object sender, int e)
        {
            OnEnemyAttack(e);
        }

        private void ChangeHealth(object sender, PlayerHealthChangedEventArgs e)
        {
            _view.Health = _model.Health+"/"+_model.MaxHealt;
            _view.HealthPercent = (float)_model.Health / _model.MaxHealt;
        }
 
        protected virtual void OnEnemyAttack(int e)
        {
            PlayerAttack?.Invoke(this, e);
        }
    }
}

using System;
using Model;
using Values;
using View;
using Random = System.Random;

namespace Controller
{
    public interface IPlayerController
    { 
        public event EventHandler PlayerManaChanged;
        public event EventHandler PlayerFainted;
        public event EventHandler PlayerHurt;
        public event EventHandler<bool> PlayerWeakened;
        public event EventHandler<int> PlayerDamageChanged;
        public void Initialize(PlayerValues values);
        public void TakeAction(ActionEventArgs e);
        public void ReceiveItemEffect(ItemEffect effect, int effectPotency);
        public int Mana(int e = 0);
        public void TurnStart();
        public void StartBattle();
        public bool IsWeaken();
    }
 
    public class PlayerController : IPlayerController
    {
        private readonly IPlayerModel _model;
        private readonly IPlayerView _view;
        public event EventHandler PlayerManaChanged;
        public event EventHandler PlayerFainted;
        public event EventHandler PlayerHurt;
        public event EventHandler<bool> PlayerWeakened;
        public event EventHandler<int> PlayerDamageChanged;


        public PlayerController(IPlayerModel model, IPlayerView view)
        {
            _model = model;
            _view = view;
 
            model.OnHealthChanged += ChangeHealth;
            model.OnManaChanged += OnManaChanged;
            model.OnWeaken += OnWeaken;
            model.OnVulnerable += OnVulnerable;
            model.OnInvulnerable += OnInvulnerable;
            model.OnDefend += OnDefend;
            model.OnDead += PlayerDead;
            model.OnTakeDamage += DamageTaken;
            model.DamageChanged+= DamageChanged;
        }

        private void DamageChanged(object sender, EventArgs e)
        {
            PlayerDamageChanged?.Invoke(this, _model.AttackPotency);
        }

        private void DamageTaken(object sender, EventArgs e)
        {
            PlayerHurt?.Invoke(this,EventArgs.Empty);
        }

        private void PlayerDead(object sender, EventArgs e)
        {
            _view.IsAlive = _model.Health > 0;
            PlayerFainted?.Invoke(this,EventArgs.Empty);
        }

        public void Initialize(PlayerValues values)
        {
            _model.MaxHealt = values.maxHealth;
            _model.Health = values.health;
            _model.Mana = values.mana;
            _model.AttackPotency = values.attackPotency;
            _model.ShieldPotency = values.shieldPotency;
            _model.SpellPotency = values.spellPotency;

            _model.InvulnerableTurns = 0;
            _model.VulnerableTurns = 0;
            _model.WeakenTurns = 0;
            
            _view.IsAlive = _model.Health > 0;
        }
        private void OnInvulnerable(object sender, EventArgs e)
        {
            _view.Invulnerable = _model.InvulnerableTurns;
        }

        public void TakeAction(ActionEventArgs e)
        {
            switch (e.Type)
            {
                case ActionType.Attack:
                    if (_model.InvulnerableTurns > 0)
                    {
                        _model.InvulnerableTurns--;
                    }
                    else
                    {
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
                        case SpellType.Invulnerable:
                            _model.InvulnerableTurns += e.Potency;
                            break;
                    }
                    break;
            }
        }

        public void ReceiveItemEffect(ItemEffect effect, int effectPotency)
        {
            switch (effect)
            {
                case ItemEffect.Attack:
                    _model.AttackPotency += effectPotency;
                    break;
                case ItemEffect.MaxHealth:
                    _model.MaxHealt += effectPotency;
                    break;
                case ItemEffect.Health:
                    _model.Health += effectPotency;
                    break;
                case ItemEffect.Shield:
                    _model.Shield += effectPotency;
                    break;
                case ItemEffect.Invulnerabilty:
                    _model.InvulnerableTurns += effectPotency;
                    break;
            }
        }

        public int Mana(int e)
        {
            _model.Mana += e;
            return _model.Mana;
        }

        public void TurnStart()
        {
            _model.Shield = 0;
            if (_model.VulnerableTurns >0)
            {
                _model.VulnerableTurns--;
            }
            if (_model.WeakenTurns >0)
            {
                _model.WeakenTurns--;
            }
        }

        public void StartBattle()
        {
            _model.InvulnerableTurns = 0;
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
            PlayerWeakened?.Invoke(this,_model.WeakenTurns> 0);
        }

        private void ChangeHealth(object sender, PlayerHealthChangedEventArgs e)
        {
            _view.Health = _model.Health+"/"+_model.MaxHealt;
            _view.HealthPercent = (float)_model.Health / _model.MaxHealt;
        }
    }
}

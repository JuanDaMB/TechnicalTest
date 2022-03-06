using System;
using UnityEngine;

namespace Model
{
    public class PlayerHealthChangedEventArgs : EventArgs
    {
        public int Health;
        public int MaxHealth;
    }
    
    public interface IPlayerModel
    {
        event EventHandler<int> OnDefend;
        event EventHandler<PlayerHealthChangedEventArgs> OnHealthChanged;
        event EventHandler OnManaChanged;
        event EventHandler OnWeaken;
        event EventHandler OnVulnerable;
        event EventHandler OnInvulnerable;
        event EventHandler OnDead;
 
        int Health { get; set; }
        int MaxHealt { get; set; }
        int Shield { get; set; }
        int AttackPotency { get; set; }
        int ShieldPotency { get; set; }
        int SpellPotency { get; set; }
        int WeakenTurns { get; set; }
        int VulnerableTurns { get; set; }
        int InvulnerableTurns { get; set; }
        int Mana { get; set; }
    }
    
    public class PlayerModel : IPlayerModel
    {
        public event EventHandler<int> OnDefend;
        public event EventHandler<PlayerHealthChangedEventArgs> OnHealthChanged;
        public event EventHandler OnManaChanged;
        public event EventHandler OnWeaken;
        public event EventHandler OnVulnerable;
        public event EventHandler OnInvulnerable;
        public event EventHandler OnDead;
        private int _health;
        private int _maxHealth;
        private int _shield;
        private int _spellPotency;
        private int _weakenTurns;
        private int _vulnerableTurns;
        private int _attackPotency;
        private int _shieldPotency;
        private int _mana;
        private int _invulnerableTurns;

        public int Health
        {
            get => _health;
            set
            {
                if (_health == value) return;
                _health = value;
                if (_health > _maxHealth)
                {
                    _health = _maxHealth;
                }

                if (_health < 0)
                {
                    _health = 0;
                }
                PlayerHealthChangedEventArgs e = new PlayerHealthChangedEventArgs
                {
                    Health = _health, MaxHealth = _maxHealth
                };
                OnHealthChanged?.Invoke(this,e);
                if (_health <= 0)
                {
                    OnDead?.Invoke(this,EventArgs.Empty);
                }
            }
        }

        public int MaxHealt
        {
            get => _maxHealth;
            set
            {
                if (_maxHealth == value) return;
                _maxHealth = value;
                PlayerHealthChangedEventArgs e = new PlayerHealthChangedEventArgs
                {
                    Health = _health, MaxHealth = _maxHealth
                };
                OnHealthChanged?.Invoke(this,e);
            }
        }

        public int Shield
        {
            get => _shield;
            set
            {
                if (_shield == value) return;
                _shield = value;
                OnDefend?.Invoke(this,_shield);
            }
        }

        public int SpellPotency
        {
            get => _spellPotency;
            set => _spellPotency = value;
        }

        public int WeakenTurns
        {
            get => _weakenTurns;
            set
            {
                _weakenTurns = value;
                OnWeaken?.Invoke(this,EventArgs.Empty);
            }
        }

        public int VulnerableTurns
        {
            get => _vulnerableTurns;
            set
            {
                _vulnerableTurns = value;
                OnVulnerable?.Invoke(this,EventArgs.Empty);
            }
        }

        public int InvulnerableTurns
        {
            get => _invulnerableTurns;
            set
            {
                _invulnerableTurns = value;
                OnInvulnerable?.Invoke(this,EventArgs.Empty);
            }
        }

        public int Mana
        {
            get => _mana;
            set
            {
                if (_mana == value) return;
                _mana = value;
                OnManaChanged?.Invoke(this,EventArgs.Empty);
            }
        }

        public int AttackPotency
        {
            get => _attackPotency;
            set => _attackPotency = value;
        }


        public int ShieldPotency
        {
            get => _shieldPotency;
            set => _shieldPotency = value;
        }
    }
}

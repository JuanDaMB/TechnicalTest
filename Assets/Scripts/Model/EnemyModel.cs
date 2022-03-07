using System;

namespace Model
{
    public class EnemyHealthChangedEventArgs : EventArgs
    {
        public int Health;
        public int MaxHealth;
    }
    
    public interface IEnemyModel
    {
        event EventHandler OnDefend;
        event EventHandler<EnemyHealthChangedEventArgs> OnHealthChanged;
        event EventHandler OnWeaken;
        event EventHandler OnVulnerable;
        event EventHandler OnDead;
 
        // EnemyStats
        int Health { get; set; }
        int MaxHealt { get; set; }
        int Shield { get; set; }
        int AttackPotency { get; set; }
        int ShieldPotency { get; set; }
        int SpellPotency { get; set; }
        int StatusPotency { get; set; }
        int WeakenTurns { get; set; }
        int VulnerableTurns { get; set; }
        ActionType Type { get; set; }
        SpellType SpellType { get; set; }
    }

    public class EnemyModel : IEnemyModel
    {
        public event EventHandler OnDefend;
        public event EventHandler<EnemyHealthChangedEventArgs> OnHealthChanged;
        public event EventHandler OnWeaken;
        public event EventHandler OnVulnerable;
        public event EventHandler OnDead;
        private int _health;
        private int _maxHealth;
        private int _shield;
        private int _spellPotency;
        private int _weakenTurns;
        private int _vulnerableTurns;
        private int _attackPotency;
        private int _shieldPotency;
        private ActionType _turn;
        private SpellType _spellType;
        private int _statusPotency;

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
                EnemyHealthChangedEventArgs e = new EnemyHealthChangedEventArgs
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
                if (_maxHealth < _health)
                {
                    Health = _maxHealth;
                }
                _maxHealth = value;
                EnemyHealthChangedEventArgs e = new EnemyHealthChangedEventArgs
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
                _shield = value;
                OnDefend?.Invoke(this,EventArgs.Empty);
            }
        }

        public int SpellPotency
        {
            get => _spellPotency;
            set => _spellPotency = value;
        }

        public int StatusPotency
        {
            get => _statusPotency;
            set => _statusPotency = value;
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


        public SpellType SpellType
        {
            get => _spellType;
            set => _spellType = value;
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

        public ActionType Type
        {
            get => _turn;
            set => _turn = value;
        }
    }
}
using System;
using UnityEngine;

public enum ActionType
{
    Attack,
    Defense,
    Spell
}

public enum SpellType
{
    Vulnerable,
    Weak,
    Heal, 
    Invulnerable
}
public class ActionEventArgs : EventArgs
{
    public ActionType Type;
    public SpellType SpellType;
    public int Cost;
    public int Potency;
    public bool Self;
}

namespace Model
{
    public interface ICardModel
    {
        event EventHandler<ActionEventArgs> OnCardUsed; 
        event EventHandler OnChangedValues;
 
        int Cost { get; set; }
        public ActionType Type { get; set; }
        public SpellType SpellType { get; set; }
        int Potency { get; set; }
        int ExtraAttack { get; set; }
        bool CardUsed { get; set; }
        bool CanBeUsed { get; set; }
    }
    public class CardModel : ICardModel
    {
        public event EventHandler<ActionEventArgs> OnCardUsed;
        public event EventHandler OnChangedValues;
        private bool _cardUsed;
        private int _cost;
        private ActionType _type;
        private SpellType _spellType;
        private string _effect;
        private int _potency;
        private bool _targetSelf;
        private bool _canBeUsed;
        private int _extraAttack;

        public int Cost
        {
            get => _cost;
            set
            {
                _cost = value;
                OnChangedValues?.Invoke(this,EventArgs.Empty);
            }
        }

        public ActionType Type
        {
            get => _type;
            set
            {
                _type = value;
                OnChangedValues?.Invoke(this,EventArgs.Empty);
            }
        }

        public SpellType SpellType
        {
            get => _spellType;
            set
            {
                _spellType = value;
                OnChangedValues?.Invoke(this,EventArgs.Empty);
            }
        }

        public string Effect
        {
            get => _effect;
            set
            {
                _effect = value;
                OnChangedValues?.Invoke(this,EventArgs.Empty);
            }
        }

        public int Potency
        {
            get => _potency;
            set
            {
                _potency = value;
                OnChangedValues?.Invoke(this,EventArgs.Empty);
            }
        }

        public int ExtraAttack
        {
            get => _extraAttack;
            set
            {
                _extraAttack = value;
                OnChangedValues?.Invoke(this,EventArgs.Empty);
            }
        }

        public bool TargetSelf
        {
            get => _targetSelf;
            set
            {
                _targetSelf = value;
                OnChangedValues?.Invoke(this,EventArgs.Empty);
            }
        }


        public bool CardUsed
        {
            get => _cardUsed;
            set
            {
                _cardUsed = value;
                if (_cardUsed)
                {
                    ActionEventArgs e = new ActionEventArgs
                    {
                        Cost = _cost,
                        Type = _type,
                        SpellType = _spellType,
                        Self = ((_spellType == SpellType.Heal || _spellType == SpellType.Invulnerable) &&
                                _type == ActionType.Spell) || _type == ActionType.Defense,
                        Potency = _type == ActionType.Attack ? _potency + _extraAttack : _potency
                    };

                    OnCardUsed?.Invoke(this, e);
                }
                else
                {
                    OnChangedValues?.Invoke(this,EventArgs.Empty);
                }
            }
        }

        public bool CanBeUsed
        {
            get => _canBeUsed;
            set
            {
                _canBeUsed = value;
                OnChangedValues?.Invoke(this,EventArgs.Empty);
            }
        }
    }
}

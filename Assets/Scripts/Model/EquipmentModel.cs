using System;
using UnityEngine;

public enum ItemActivationCondition
{
    StartBattle,
    EndBattle,
    StartTurn,
    EndTurn,
    OnSpell,
    OnDamageTaken,
    OnEquip
}

public enum ItemEffect
{
    Health,
    Shield,
    Attack,
    Damage,
    Gold,
    MaxHealth,
    Invulnerabilty,
    CardAmount
}

public enum Ubication
{
    Head,
    Chest,
    Weapon
}

namespace Model
{
    public interface IEquipmentModel
    {
        event EventHandler OnEquip;
        event EventHandler OnUnequip;
        event EventHandler PriceChanged;
        event EventHandler OnBought;
        event EventHandler OnSell;
        event EventHandler OnRender;
 
        int EffectPotency { get; set; }
        int Condition { get; set; }
        int BasePrice { get; set; }
        float Discount { get; set; }
        int CurrenPrice { get; }
        ItemActivationCondition ActivationCondition { get; set; }
        Ubication ItemUbication { get; set; }
        ItemEffect ItemEffect { get; set; }
        bool Equiped { get; set; }
        bool Bought { get; set; }
        bool Render { get; set; }
        void Reset(bool bought, bool equiped);
    }
    public class EquipmentModel : IEquipmentModel
    {
        private bool _equiped;
        private int _basePrice;
        private float _discount;
        private int _currenPrice;
        private bool _bought;
        private int _condition;
        private bool _render;
        public event EventHandler OnEquip;
        public event EventHandler OnUnequip;
        public event EventHandler PriceChanged;
        public event EventHandler OnBought;
        public event EventHandler OnSell;
        public event EventHandler OnRender;

        public int EffectPotency { get; set; }

        public int Condition
        {
            get => _condition;
            set => _condition = value;
        }

        public int BasePrice
        {
            get => _basePrice;
            set
            {
                if(_basePrice == value) return;
                _basePrice = value;
                PriceChanged?.Invoke(this,EventArgs.Empty);
            }
        }

        public float Discount
        {
            get => _discount;
            set
            {
                if(_discount == value) return;
                _discount = value;
                PriceChanged?.Invoke(this,EventArgs.Empty);
            }
        }

        public int CurrenPrice
        {
            get
            {
                _currenPrice = (int)(_basePrice * _discount);
                return _currenPrice;
            }
        }

        public ItemActivationCondition ActivationCondition { get; set; }

        public Ubication ItemUbication { get; set; }

        public ItemEffect ItemEffect { get; set; }

        public bool Equiped
        {
            get => _equiped;
            set
            {
                if (_equiped == value)
                {
                    return;
                }
                _equiped = value;
                if (_equiped)
                {
                    OnEquip?.Invoke(this,EventArgs.Empty);
                }
                else
                {
                    OnUnequip?.Invoke(this,EventArgs.Empty);
                }
            }
        }

        public bool Bought
        {
            get => _bought;
            set
            {
                if (_bought == value) return;
                _bought = value;
                if (_bought)
                {
                    OnBought?.Invoke(this,EventArgs.Empty);
                }
                else
                {
                    OnSell?.Invoke(this,EventArgs.Empty);
                }
            }
        }

        public bool Render
        {
            get => _render;
            set
            {
                if (_render == value)
                {
                    return;
                }
                _render = value;
                OnRender?.Invoke(this,EventArgs.Empty);
            }
        }

        public void Reset(bool bought, bool equiped)
        {
            _bought = bought;
            _equiped = equiped;
        }
    }
}

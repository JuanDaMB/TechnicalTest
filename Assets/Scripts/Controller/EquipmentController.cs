using System;
using Model;
using UnityEngine;
using Values;
using View;

namespace Controller
{
    public interface IEquipmentController
    {
        event EventHandler<EquipmentValues> ItemBought;
        event EventHandler<EquipmentValues> ItemSold;
        event EventHandler<EquipmentValues> ItemEquiped;
        event EventHandler<EquipmentValues> ItemUnequiped;
        public void Initialize(EquipmentValues values, int currentGold,bool bought, bool equiped, bool isInShop);
        public void ShoudlShow(bool value);
        public void SlotUsed(Ubication ubication);
        public void SlotFree();
    }
    public class EquipmentController : IEquipmentController
    {
        private readonly IEquipmentModel _model;
        private readonly IEquipmentView _view;
        private EquipmentValues _values;
        
        public event EventHandler<EquipmentValues> ItemBought;
        public event EventHandler<EquipmentValues> ItemSold;
        public event EventHandler<EquipmentValues> ItemEquiped;
        public event EventHandler<EquipmentValues> ItemUnequiped;

        public EquipmentController(IEquipmentModel model, IEquipmentView view)
        {
            _model = model;
            _view = view;

            view.OnBuy += ViewOnBuy;
            view.OnSell += ViewOnSell;
            view.OnEquip += ViewOnEquip;
            view.OnUnequip += ViewOnUnequip;
            model.PriceChanged += PriceChanged;
            model.OnBought += ModelOnBought;
            model.OnUnequip += ModelOnUnequip;
            model.OnEquip += ModelOnEquip;
            model.OnSell += ModelOnSell;
        }

        private void ModelOnSell(object sender, EventArgs e)
        {
            if (_model.Bought) return;
            ItemSold?.Invoke(this,_values);
        }

        private void ModelOnUnequip(object sender, EventArgs e)
        {
            if (_model.Equiped) return;
            ItemUnequiped?.Invoke(this,_values);
        }
        private void ModelOnEquip(object sender, EventArgs e)
        {
            ItemEquiped?.Invoke(this,_values);
        }

        private void ModelOnBought(object sender, EventArgs e)
        {
            if (!_model.Bought) return;
            ItemBought?.Invoke(this,_values);
        }


        private void ViewOnUnequip(object sender, EventArgs e)
        {
            _model.Equiped = false;
        }

        private void ViewOnEquip(object sender, EventArgs e)
        {
            _model.Equiped = true;
        }

        private void ViewOnSell(object sender, EventArgs e)
        {
            _model.Bought = false;
        }
        private void ViewOnBuy(object sender, EventArgs e)
        {
            _model.Bought = true;
        }

        private void PriceChanged(object sender, EventArgs e)
        {
            _view.Price = "$ "+_model.CurrenPrice;
            _view.Discounted = _model.Discount < 1f;
        }


        public void Initialize(EquipmentValues values, int currentGold,bool bought, bool equiped, bool isInShop)
        {
            _values = values;
            _model.ActivationCondition = values.activationCondition;
            _model.Condition = values.condition;
            _model.ItemUbication = values.itemUbication;
            _model.ItemEffect = values.itemEffect;
            _model.BasePrice = values.basePrice;
            _model.Discount = values.discount;
            _model.EffectPotency = values.effectPotency;
            _model.Reset(bought,equiped);
            _view.Sprite = values.sprite;
            _view.Effect = values.effect;
            _view.OnShop = isInShop;
            _view.IsEquiped = _model.Equiped;
            _view.UpdateButtons();
            _view.Purchasable = currentGold >= _model.CurrenPrice || _model.Bought;
            _view.Show = true;
        }

        public void ShoudlShow(bool value)
        {
            Render(value);
        }

        public void SlotUsed(Ubication ubication)
        {
            if (ubication == _model.ItemUbication && _model.Bought)
            {
                _view.CanEquip = false;
                _view.UpdateButtons();
            }
        }

        public void SlotFree()
        {
            _view.CanEquip = true;
            _view.UpdateButtons();
        }

        private void Render(bool value)
        {
            _view.Show = value;
        }
    }
}

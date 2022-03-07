using System;
using Model;
using UnityEngine;
using Values;
using View;
using Random = Unity.Mathematics.Random;

namespace Controller
{
    public interface ICardController
    { 
        event EventHandler<ActionEventArgs> CardUsed;
        public void Initialize(CardValues values, int currentPlayerMana);
        public void UpdateUsage(int currentPlayerMana);
        public void UpdateDamage(int damage, bool weakened);
        public void DestroyAll();
    }
    public class CardController : ICardController
    {
        public event EventHandler<ActionEventArgs> CardUsed;
        private readonly ICardModel _model;
        private readonly ICardView _view;
        private bool _weakened;
        public void Initialize(CardValues values, int currentPlayerMana)
        {
            _model.Cost = values.cost;
            _model.Potency = values.potency;
            _model.Type = values.type;
            _model.SpellType = values.spellType;
            _model.CardUsed = false;
            _model.CanBeUsed = currentPlayerMana >= _model.Cost;
        }

        public void UpdateUsage(int currentPlayerMana)
        {
            _model.CanBeUsed = currentPlayerMana >= _model.Cost;
        }

        public void UpdateDamage(int damage, bool weakened)
        {
            _weakened = weakened;
            _model.ExtraAttack = damage;
        }

        public void DestroyAll()
        {
            _view.DestroyObject();
        }

        public CardController(ICardModel model, ICardView view)
        {
            _model = model;
            _view = view;

            view.OnClick += CardSelected;
            model.OnCardUsed += UseCard;
            model.OnChangedValues += UpdateCardValues;
        }

        private void UpdateCardValues(object sender, EventArgs e)
        {
            _view.Cost = _model.Cost;
            _view.Type = _model.Type;
            _view.IsUsed = _model.CardUsed;
            _view.CanBeUsed = _model.CanBeUsed;
            switch (_model.Type)
            {
                case ActionType.Attack:
                    int damage = _model.Potency+_model.ExtraAttack;
                    if (_weakened)
                    {
                        damage -= damage / 2;
                    }
                    _view.Effect = "Deal " + damage + " damage";
                    break;
                case ActionType.Defense:
                    _view.Effect = "Gain " + _model.Potency + " shield";
                    break;
                case ActionType.Spell:
                    switch (_model.SpellType)
                    {
                        case SpellType.Vulnerable:
                            _view.Effect = "Apply " + _model.Potency +" turns of vulnerability to the enemy (50% more Damage received)";
                            break;
                        case SpellType.Weak:
                            _view.Effect = "Apply " + _model.Potency +" turns of weak to the enemy (50% more Damage dealt)";
                            break;
                        case SpellType.Heal:
                            _view.Effect = "Heal yourself " + _model.Potency;
                            break;
                        case SpellType.Invulnerable:
                            _view.Effect = "Gain " + _model.Potency+" turns of Invulnerability (receive no damage)";
                            break;
                        default:
                            _view.Effect = "Spell Error";
                            break;
                    }
                    break;
                default:
                    _view.Effect = "Card Error";
                    break;
            }
        }

        private void UseCard(object sender, ActionEventArgs e)
        {
            CardUsed?.Invoke(this, e);
        }

        private void CardSelected(object sender, EventArgs e)
        {
            if (_model.CardUsed)
            {
                return;
            }
            _model.CardUsed = true;
            _view.IsUsed = _model.CardUsed;
        }
 
    }
}

using System;
using System.Collections.Generic;
using Controller;
using Factory;
using Model;
using UnityEngine;
using Values;
using Random = UnityEngine.Random;

namespace Main
{
    public class GameManager : MonoBehaviour
    {
        #region Controllers
        
        private IPlayerController _playerController;
        private IEnemyController _enemyController;
        private List<ICardController> _cardControllers;
        private List<IEquipmentController> _shopEquipmentControllers, _inventoryEquipmentControllers, _equipedEquipmentControllers;
        private IInventoryController _inventoryController;

        #endregion
        
        public Transform cardsParent, shopParent, inventoryParent, equipmentParent, equipedParent;
        public GameObject battleView, shopView, endGameView;
        
        [Header("Values")]
        public PlayerValues playerValues;
        public List<EnemyValues> enemyValues;
        public List<CardValues> cardValues;
        public List<EquipmentValues> equipmentValues;
        [SerializeField]private List<EquipmentValues> _currentShop, _currentInventory, _currentEquiped;
        private int _turnCount;
        
        void Awake()
        {
            _cardControllers = new List<ICardController>();
            _shopEquipmentControllers = new List<IEquipmentController>();
            _inventoryEquipmentControllers = new List<IEquipmentController>();
            _equipedEquipmentControllers = new List<IEquipmentController>();
            _currentInventory = new List<EquipmentValues>();
            _currentShop = new List<EquipmentValues>();
            _currentEquiped = new List<EquipmentValues>();
            CreateEnemy();
            CreatePlayer();
            _inventoryController = CreateInventory();
            _inventoryController.SetCurrentGold(playerValues.gold);
            
            for (int i = 0; i < 3; i++)
            {
                
                _cardControllers.Add(CreateCard());
            }

            for (int i = 0; i < 3; i++)
            {
                _shopEquipmentControllers.Add(CreateEquipment(shopParent));
                _equipedEquipmentControllers.Add(CreateEquipment(equipedParent));
                _currentEquiped.Add(null);
            }
            
            StartBattle();
        }

        private void CreateEnemy()
        {
            var enemyModelFactory = new EnemyModelFactory();
            var enemyModel = enemyModelFactory.Model;
            var enemyViewFactory = new EnemyViewFactory();
            var enemyView = enemyViewFactory.View;
            var enemyControllerFactory = new EnemyControllerFactory(enemyModel, enemyView);
            var enemyController = enemyControllerFactory.Controller;
            _enemyController = enemyController;
            _enemyController.EnemyAttack += TakeEnemyAttack;
            _enemyController.EnemyTurnEnd += EnemyTurnEnd;
            _enemyController.EnemyFainted += EnemyFainted;
        }
        
        private void CreatePlayer()
        {
            var playerModelFactory = new PlayerModelFactory();
            var playerModel = playerModelFactory.Model;
            var playerViewFactory = new PlayerViewFactory();
            var playerView = playerViewFactory.View;
            var playerControllerFactory = new PlayerControllerFactory(playerModel, playerView);
            var playerController = playerControllerFactory.Controller;
            _playerController = playerController;
            _playerController.PlayerManaChanged += OnPlayerManaChanged;
            _playerController.PlayerFainted+= PlayerFainted;
            _playerController.PlayerHurt += PlayerHurt;
            _playerController.PlayerWeakened += PlayerWeakened;
            _playerController.PlayerDamageChanged += PlayerDamageChanged;
            _playerController.Initialize(playerValues);
        }

        private void PlayerDamageChanged(object sender, int e)
        {
            foreach (ICardController controller in _cardControllers)
            {
                controller.UpdateDamage(e, _playerController.IsWeaken());
            }
        }

        private void PlayerWeakened(object sender, bool e)
        {
            foreach (ICardController controller in _cardControllers)
            {
                controller.UpdateDamage(0, e);
            }
        }

        private void PlayerHurt(object sender, EventArgs e)
        {
            CheckItemEffects(ItemActivationCondition.OnDamageTaken);
        }

        private ICardController CreateCard()
        {
            var cardModelFactoy = new CardModelFactory();
            var cardModel = cardModelFactoy.Model;
            var cardViewFactory = new CardViewFactory(cardsParent);
            var cardView = cardViewFactory.View;
            var cardControllerFactory = new CardControllerFactory(cardModel, cardView);
            var cardController = cardControllerFactory.Controller;
            cardController.CardUsed += OnCardUsed;
            return cardController;
        }
        private IInventoryController CreateInventory()
        {
            var inventoryModelFactoy = new InventoryModelFactory();
            var inventoryModel = inventoryModelFactoy.Model;
            var inventoryViewFactory = new InventoryViewFactory(inventoryParent);
            var inventoryView = inventoryViewFactory.View;
            var inventoryControllerFactory = new InventoryControllerFactory(inventoryModel, inventoryView);
            var inventoryController = inventoryControllerFactory.Controller;
            return inventoryController;
        }
        private IEquipmentController CreateEquipment(Transform parent)
        {
            var equipmentModelFactoy = new EquipmentModelFactory();
            var equipmentModel = equipmentModelFactoy.Model;
            var equipmentViewFactory = new EquipmentViewFactory(parent);
            var equipmentView = equipmentViewFactory.View;
            var equipmentControllerFactory = new EquipmentControllerFactory(equipmentModel, equipmentView);
            var equipmentController = equipmentControllerFactory.Controller;
            equipmentController.ItemEquiped += ItemEquiped;
            equipmentController.ItemUnequiped += ItemUnequiped;
            equipmentController.ItemSold += ItemSold;
            equipmentController.ItemBought += ItemBought;
            equipmentController.ShoudlShow(false);
            return equipmentController;
        }

        private void ItemUnequiped(object sender, EquipmentValues e)
        {
            e.effectUsed = false;
            CheckItemEffects(ItemActivationCondition.OnEquip, true);
            e.effectUsed = false;
            
            switch (e.itemUbication)
            {
                case Ubication.Head:
                    _currentEquiped[0] = null;
                    break;
                case Ubication.Chest:
                    _currentEquiped[1] = null;
                    break;
                case Ubication.Weapon:
                    _currentEquiped[2] = null;
                    break;
            }
            
            _currentInventory.Add(e);
            
            UpdateInventory();
            UpdateEquipment();
        }

        private void UpdateEquipment()
        {
            foreach (IEquipmentController controller in _equipedEquipmentControllers)
            {
                controller.ShoudlShow(false);
            }
            for (int i = 0; i < _currentEquiped.Count; i++)
            {
                if (_currentEquiped[i] != null)
                {
                    foreach (IEquipmentController controller in _inventoryEquipmentControllers)
                    {
                        controller.SlotUsed(_currentEquiped[i].itemUbication);
                    }
                    _equipedEquipmentControllers[i].Initialize(_currentEquiped[i], _inventoryController.GetCurrentGold(), true, true, false);
                }
            }
        }

        private void ItemSold(object sender, EquipmentValues e)
        {
            _inventoryController.SetCurrentGold(_inventoryController.GetCurrentGold()+e.basePrice);
            e.effectUsed = false;
            _currentInventory.Remove(e);
            _currentShop.Add(e);

            UpdateShop();
            UpdateInventory();
        }


        private void ItemBought(object sender, EquipmentValues e)
        {
            _inventoryController.SetCurrentGold(_inventoryController.GetCurrentGold()-e.basePrice);
            e.effectUsed = false;
            _currentInventory.Add(e);
            _currentShop.Remove(e);

            UpdateShop();
            UpdateInventory();
        }
        private void ItemEquiped(object sender, EquipmentValues e)
        {
            switch (e.itemUbication)
            {
                case Ubication.Head:
                    _currentEquiped[0] = e;
                    break;
                case Ubication.Chest:
                    _currentEquiped[1] = e;
                    break;
                case Ubication.Weapon:
                    _currentEquiped[2] = e;
                    break;
            }
            _currentInventory.Remove(e);
            CheckItemEffects(ItemActivationCondition.OnEquip);
            UpdateInventory();
            UpdateEquipment();
        }

        private void CheckItemEffects(ItemActivationCondition condition, bool invert = false, int count = 0)
        {
            foreach (EquipmentValues values in _currentEquiped)
            {
                if (values == null) continue;
                if (values.effectUsed) continue;
                if (values.activationCondition != condition) continue;
                if (condition == ItemActivationCondition.OnEquip)
                {
                    values.effectUsed = true;
                }
                int potency = values.effectPotency;
                if (invert)
                {
                    potency *= -1;
                }

                switch (values.itemEffect)
                {
                    case ItemEffect.Health:
                        _playerController.ReceiveItemEffect(values.itemEffect, potency);
                        break;
                    case ItemEffect.Shield:
                        _playerController.ReceiveItemEffect(values.itemEffect, potency);
                        break;
                    case ItemEffect.Attack:
                        _playerController.ReceiveItemEffect(values.itemEffect, potency);
                        break;
                    case ItemEffect.Gold:
                        _inventoryController.SetCurrentGold(_inventoryController.GetCurrentGold() + potency);
                        break;
                    case ItemEffect.MaxHealth:
                        _playerController.ReceiveItemEffect(values.itemEffect, potency);
                        break;
                    case ItemEffect.Invulnerabilty:
                        _playerController.ReceiveItemEffect(values.itemEffect, potency);
                        break;
                    case ItemEffect.CardAmount:
                        int n = _cardControllers.Count + potency;
                        foreach (ICardController controller in _cardControllers)
                        {
                            controller.DestroyAll();
                        }
                        _cardControllers.Clear();
                        for (int i = 0; i < n; i++)
                        {
                            _cardControllers.Add(CreateCard());
                        }

                        break;
                    case ItemEffect.Damage:
                        ActionEventArgs args = new ActionEventArgs();
                        args.Potency = potency;
                        args.Type = ActionType.Attack;
                        if (values.condition > 0)
                        {
                            if (values.condition == count)
                            {
                                _enemyController.TakeAction(args);
                            }
                        }
                        else
                        {
                                
                            _enemyController.TakeAction(args);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
        }

        private void UpdateInventory()
        {
            foreach (IEquipmentController controller in _inventoryEquipmentControllers)
            {
                controller.ShoudlShow(false);
            }
            
            foreach (IEquipmentController controller in _inventoryEquipmentControllers)
            {
                controller.SlotFree();
            }
            foreach (var t in _currentEquiped)
            {
                if (t == null) continue;
                foreach (IEquipmentController controller in _inventoryEquipmentControllers)
                {
                    controller.SlotUsed(t.itemUbication);
                }
            }

            if (_currentInventory.Count <= 0) return;
            for (int i = 0; i < _currentInventory.Count; i++)
            {
                if (i >= _inventoryEquipmentControllers.Count)
                {
                    _inventoryEquipmentControllers.Add(CreateEquipment(equipmentParent));
                }
                _inventoryEquipmentControllers[i].Initialize(_currentInventory[i], _inventoryController.GetCurrentGold(),
                    true, false, false);
            }
            foreach (IEquipmentController controller in _inventoryEquipmentControllers)
            {
                controller.SlotFree();
            }
            foreach (var t in _currentEquiped)
            {
                if (t == null) continue;
                foreach (IEquipmentController controller in _inventoryEquipmentControllers)
                {
                    controller.SlotUsed(t.itemUbication);
                }
            }
        }

        private void UpdateShop()
        {
            foreach (IEquipmentController controller in _shopEquipmentControllers)
            {
                controller.ShoudlShow(false);
            }

            if (_currentShop.Count <= 0) return;
            for (int i = 0; i < _currentShop.Count; i++)
            {
                if (i >= _shopEquipmentControllers.Count)
                {
                    _shopEquipmentControllers.Add(CreateEquipment(shopParent));
                }
                _shopEquipmentControllers[i].Initialize(_currentShop[i],_inventoryController.GetCurrentGold(), false,false,true);
            }
        }

        private void EnemyFainted(object sender, EventArgs e)
        {
            EndBattle();
        }

        private void EndBattle()
        {
            CheckItemEffects(ItemActivationCondition.EndBattle);
            battleView.SetActive(false);
            shopView.SetActive(true);
            _inventoryController.SetCurrentGold(_inventoryController.GetCurrentGold()+Random.Range(10,21));
            foreach (EquipmentValues values in _currentShop)
            {
                equipmentValues.Add(values);
            }
            _currentShop.Clear();

            for (int i = 0; i < _shopEquipmentControllers.Count; i++)
            {
                if (equipmentValues.Count <= 0)
                {
                    return;
                }
                int index = Random.Range(0, equipmentValues.Count);
                _currentShop.Add(equipmentValues[index]);
                equipmentValues.RemoveAt(index);
            }
            
            UpdateShop();
        }


        private void EnemyTurnEnd(object sender, EventArgs e)
        {
            _turnCount++;
            OnStartTurn();
        }

        private void OnPlayerManaChanged(object sender, EventArgs e)
        {
            foreach (var t in _cardControllers)
            {
                t.UpdateUsage(_playerController.Mana());
            }
        }
        
        private void PlayerFainted(object sender, EventArgs e)
        {
            battleView.SetActive(false);
            shopView.SetActive(false);
            endGameView.SetActive(true);
        }
        
        public void StartBattle()
        {
            _turnCount = 0;
            _playerController.StartBattle();
            CheckItemEffects(ItemActivationCondition.StartBattle);
            battleView.SetActive(true);
            shopView.SetActive(false);
            _enemyController.Initialize(enemyValues[Random.Range(0,enemyValues.Count)]);
            OnStartTurn();
        }
        
        private void OnStartTurn()
        {
            _playerController.TurnStart();
            CheckItemEffects(ItemActivationCondition.StartTurn, false,_turnCount);
            int currentMana = _playerController.Mana();
            _playerController.Mana(-currentMana);
            _playerController.Mana(playerValues.mana);
            foreach (var t in _cardControllers)
            {
                int n = Random.Range(0, cardValues.Count);
                t.Initialize(cardValues[n], _playerController.Mana());
            }
        }

        public void EndTurn()
        {
            CheckItemEffects(ItemActivationCondition.EndTurn);
            _enemyController.StartTurn();
        }

        private void OnCardUsed(object sender, ActionEventArgs e)
        {
            if (e.Type == ActionType.Attack && _playerController.IsWeaken())
            {
                e.Potency -= e.Potency / 2;
            }
            if (e.Self)
            {
                _playerController.TakeAction(e);
            }
            else
            {
                _enemyController.TakeAction(e);
            }

            if (e.Type == ActionType.Spell)
            {
                CheckItemEffects(ItemActivationCondition.OnSpell);
            }
            _playerController.Mana(-e.Cost);
        }

        void TakeEnemyAttack(object sender, ActionEventArgs e)
        {
            _playerController.TakeAction(e);
        }
    }
}

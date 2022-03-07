using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Factory;
using Model;
using SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Values;
using Random = UnityEngine.Random;

namespace Main
{
    public class GameManager : MonoBehaviour
    {
        #region Controllers
        
        private IPlayerController _playerController;
        private IEnemyController _enemyController, _finalBossController;
        private List<ICardController> _cardControllers;
        private List<IEquipmentController> _shopEquipmentControllers, _inventoryEquipmentControllers, _equipedEquipmentControllers;
        private IInventoryController _inventoryController;

        #endregion
        
        public Transform cardsParent, shopParent, inventoryParent, equipmentParent, equipedParent;
        public GameObject battleView, shopView;
        public SaveScriptableData saveData;
        
        [Header("Values")]
        public PlayerValues playerValues, originalPLayerValues;
        public List<EnemyValues> enemyValues;
        public List<CardValues> cardValues;
        public List<EquipmentValues> equipmentValues;
        private List<EquipmentValues> _currentShop, _currentInventory, _currentEquiped;
        public EnemyValues finalBoss;
        [Header("GameFlowCounters")] 
        public TextMeshProUGUI turns;
        public TextMeshProUGUI battles;
        [Header("EndGame")] 
        public GameObject endGameView;
        public TextMeshProUGUI endGameMessage;
        private int _turnCount;
        private int _battleCount;

        private int TurnCount
        {
            set
            {
                _turnCount = value;
                turns.text = "Turn: " + _turnCount;
            }
        }

        private int BattleCount
        {
            set
            {
                _battleCount = value;
                battles.text = "Battle: " + _battleCount +"/10";
            }
        }

        void Awake()
        {
            _cardControllers = new List<ICardController>();
            _shopEquipmentControllers = new List<IEquipmentController>();
            _inventoryEquipmentControllers = new List<IEquipmentController>();
            _equipedEquipmentControllers = new List<IEquipmentController>();
            _currentInventory = new List<EquipmentValues>();
            _currentShop = new List<EquipmentValues>();
            _currentEquiped = new List<EquipmentValues>();
            _inventoryController = CreateInventory();
            _inventoryController.SetCurrentGold(playerValues.gold);

            for (int i = 0; i < 3; i++)
            {
                _shopEquipmentControllers.Add(CreateEquipment(shopParent));
                _equipedEquipmentControllers.Add(CreateEquipment(equipedParent));
                _currentEquiped.Add(null);
            }
            
            for (int i = 0; i < 3; i++)
            {
                _cardControllers.Add(CreateCard());
            }
            _playerController = CreatePlayer();
            _playerController.PlayerManaChanged += OnPlayerManaChanged;
            _playerController.PlayerFainted+= PlayerFainted;
            _playerController.PlayerHurt += PlayerHurt;
            _playerController.PlayerWeakened += PlayerWeakened;
            _playerController.PlayerDamageChanged += PlayerDamageChanged;

            if (saveData.load)
            {
                LoadData();
            }
            else
            {
                BattleCount = 0;
                playerValues = originalPLayerValues;
                _playerController.Initialize(playerValues);
            }
            
            _enemyController = CreateEnemy("Enemy");
            _finalBossController = CreateEnemy("FinalBoss");
            
            _enemyController.EnemyAttack += TakeEnemyAttack;
            _enemyController.EnemyTurnEnd += EnemyTurnEnd;
            _enemyController.EnemyFainted += EnemyFainted;
            _finalBossController.EnemyAttack += TakeEnemyAttack;
            _finalBossController.EnemyTurnEnd += EnemyTurnEnd;
            _finalBossController.EnemyFainted += EnemyFainted;
            
            StartBattle();
        }

        #region LoadSave

        private void LoadData()
        {
            SaveData data = SaveSystem.SaveSystem.ReadState();
            if (data != null)
            {
                saveData.equipmentValues = new List<EquipmentValues>();
                saveData.inventoryValues = new List<EquipmentValues>();

                saveData.player = data.player;
                saveData.battle = data.battle;
                playerValues = saveData.player;
                _battleCount = saveData.battle;
                _inventoryController.SetCurrentGold(playerValues.gold);

                _playerController.Initialize(playerValues);
                
                if (data.inventoryValues != null && data.inventoryValues.Count > 0)
                {
                    foreach (EquipmentValues value in data.inventoryValues)
                    {
                        saveData.inventoryValues.Add(value);
                    }

                    foreach (EquipmentValues value in saveData.inventoryValues)
                    {
                        _currentInventory.Add(value);
                        equipmentValues.Remove(value);
                    }
                    UpdateInventory();
                }
                if (data.equipmentValues != null && data.equipmentValues.Count > 0)
                {
                    foreach (EquipmentValues value in data.equipmentValues)
                    {
                        saveData.equipmentValues.Add(value);
                    }
                    foreach (EquipmentValues value in saveData.equipmentValues)
                    {
                        if (value != null)
                        {
                            value.effectUsed = false;
                            switch (value.itemUbication)
                            {
                                case Ubication.Head:
                                    _currentEquiped[0] = value;
                                    break;
                                case Ubication.Chest:
                                    _currentEquiped[1] = value;
                                    break;
                                case Ubication.Weapon:
                                    _currentEquiped[2] = value;
                                    break;
                            }
                            equipmentValues.Remove(value);
                            CheckItemEffects(ItemActivationCondition.OnEquip);
                            UpdateEquipment();
                        }
                    }
                }
            }
        }

        public void Save()
        {
            saveData.equipmentValues = new List<EquipmentValues>();
            saveData.inventoryValues = new List<EquipmentValues>();

            foreach (EquipmentValues values in _currentInventory)
            {
                saveData.inventoryValues.Add(values);
            }

            foreach (EquipmentValues values in _currentEquiped)
            {
                saveData.equipmentValues.Add(values);
            }

            playerValues = _playerController.Current();
            playerValues.gold = _inventoryController.GetCurrentGold();
            saveData.player = playerValues;
            saveData.battle = _battleCount;
            SaveSystem.SaveSystem.SaveState(saveData);
            
            SceneManager.LoadScene(0);
        }
        
        #endregion

        #region Factory
        private IEnemyController CreateEnemy(string resource)
        {
            var enemyModelFactory = new EnemyModelFactory();
            var enemyModel = enemyModelFactory.Model;
            var enemyViewFactory = new EnemyViewFactory(resource);
            var enemyView = enemyViewFactory.View;
            var enemyControllerFactory = new EnemyControllerFactory(enemyModel, enemyView);
            var enemyController = enemyControllerFactory.Controller;
            return enemyController;
        }
        private IPlayerController CreatePlayer()
        {
            var playerModelFactory = new PlayerModelFactory();
            var playerModel = playerModelFactory.Model;
            var playerViewFactory = new PlayerViewFactory();
            var playerView = playerViewFactory.View;
            var playerControllerFactory = new PlayerControllerFactory(playerModel, playerView);
            var playerController = playerControllerFactory.Controller;
            return playerController;
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
        #endregion

        #region PlayerEvents
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
            endGameMessage.text = "You Died";
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
        #endregion

        #region ItemEvents
        private void ItemSold(object sender, EquipmentValues e)
        {
            _inventoryController.SetCurrentGold(_inventoryController.GetCurrentGold()+ (int)(e.basePrice*e.discount));
            e.effectUsed = false;
            _currentInventory.Remove(e);
            _currentShop.Add(e);

            UpdateShop();
            UpdateInventory();
        }
        private void ItemBought(object sender, EquipmentValues e)
        {
            _inventoryController.SetCurrentGold(_inventoryController.GetCurrentGold()- (int)(e.basePrice*e.discount));
            e.effectUsed = false;
            _currentInventory.Add(e);
            _currentShop.Remove(e);

            UpdateShop();
            UpdateInventory();
        }
        private void ItemEquiped(object sender, EquipmentValues e)
        {
            _currentEquiped[(int)e.itemUbication] = e;
            _currentInventory.Remove(e);
            CheckItemEffects(ItemActivationCondition.OnEquip);
            UpdateInventory();
            UpdateEquipment();
        }
        private void ItemUnequiped(object sender, EquipmentValues e)
        {
            e.effectUsed = false;
            CheckItemEffects(ItemActivationCondition.OnEquip, true);
            e.effectUsed = false;
            _currentEquiped[(int)e.itemUbication] = null;
            _currentInventory.Add(e);
            UpdateInventory();
            UpdateEquipment();
        }
        private void CheckItemEffects(ItemActivationCondition condition, bool invert = false, int count = 0)
        {
            foreach (var values in from values in _currentEquiped where values != null where !values.effectUsed where values.activationCondition == condition select values)
            {
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
                    case ItemEffect.Shield:
                    case ItemEffect.MaxHealth:
                    case ItemEffect.Attack:
                    case ItemEffect.Invulnerabilty:
                        _playerController.ReceiveItemEffect(values.itemEffect, potency);
                        break;
                    case ItemEffect.Gold:
                        _inventoryController.SetCurrentGold(_inventoryController.GetCurrentGold() + potency);
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
                }
            }
        }
        #endregion

        #region UpdateItemViews
        private void HideItemList(List<IEquipmentController> controllers)
        {
            foreach (IEquipmentController controller in controllers)
            {
                controller.ShoudlShow(false);
            }
        }
        private void UpdateEquipment()
        {
            HideItemList(_equipedEquipmentControllers);
            for (int i = 0; i < _currentEquiped.Count; i++)
            {
                if (_currentEquiped[i] == null) continue;
                foreach (IEquipmentController controller in _inventoryEquipmentControllers)
                {
                    controller.SlotUsed(_currentEquiped[i].itemUbication);
                }
                _equipedEquipmentControllers[i].Initialize(_currentEquiped[i], _inventoryController.GetCurrentGold(), true, true, false);
            }
        }
        private void UpdateInventory()
        {
            HideItemList(_inventoryEquipmentControllers);
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
            HideItemList(_shopEquipmentControllers);
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
        #endregion

        #region BattleFlow
        public void StartBattle()
        {
            TurnCount = 0;
            BattleCount = _battleCount+1;
            _playerController.StartBattle();
            CheckItemEffects(ItemActivationCondition.StartBattle);
            battleView.SetActive(true);
            shopView.SetActive(false);
            if (_battleCount >= 10)
            {
                _enemyController = _finalBossController;
                _enemyController.Initialize(finalBoss);
            }
            else
            {
                _enemyController.Initialize(enemyValues[Random.Range(0,enemyValues.Count)]);
            }
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
        private void EnemyTurnEnd(object sender, EventArgs e)
        {
            TurnCount = _turnCount + 1;
            OnStartTurn();
        }
        private void EndBattle()
        {
            CheckItemEffects(ItemActivationCondition.EndBattle);
            if (_battleCount>= 10)
            {
                endGameView.SetActive(true);
                endGameMessage.text = "You defeated the final boss";
                return;
            }
            battleView.SetActive(false);
            shopView.SetActive(true);
            _inventoryController.SetCurrentGold(_inventoryController.GetCurrentGold()+Random.Range(10,21));
            
        }

        private void CreateShop()
        {
            foreach (EquipmentValues values in _currentShop)
            {
                values.discount = 1f;
                equipmentValues.Add(values);
            }
            _currentShop.Clear();

            float random = Random.Range(0f, 100f);
            for (int i = 0; i < _shopEquipmentControllers.Count && i < 3; i++)
            {
                if (equipmentValues.Count <= 0)
                {
                    return;
                }
                int index = Random.Range(0, equipmentValues.Count);
                if (random > 50f)
                {
                    equipmentValues[index].discount = Random.Range(0.1f, 0.9f);
                    random = 0f;
                }
                _currentShop.Add(equipmentValues[index]);
                equipmentValues.RemoveAt(index);
            }
            
            UpdateShop();
        }
        #endregion

        #region EnemyEvents
        private void EnemyFainted(object sender, EventArgs e)
        {
            EndBattle();
        }
        void TakeEnemyAttack(object sender, ActionEventArgs e)
        {
            _playerController.TakeAction(e);
        }
        #endregion
    }
}

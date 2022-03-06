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
        private IPlayerController PlayerController;
        private IEnemyController EnemyController;
        public Transform cardsParent;
        private List<ICardModel> CardModels;
        public PlayerValues PlayerValues;
        public List<EnemyValues> EnemyValues;
        public List<CardValues> CardValues;
        public GameObject BattleView, ShopView;
        void Awake()
        {
            CardModels = new List<ICardModel>();
            CreateEnemy();
            EnemyController.Initialize(EnemyValues[Random.Range(0,EnemyValues.Count)]);
            EnemyController.EnemyTurnEnd += EnemyTurnEnd;
            EnemyController.EnemyFainted += EnemyFainted;
            
            CreatePlayer();
            PlayerController.Initialize(PlayerValues);
            
            for (int i = 0; i < 4; i++)
            {
                CreateCard(i);
            }
            OnStartTurn();
        }

        private void EnemyFainted(object sender, EventArgs e)
        {
            BattleView.SetActive(false);
            ShopView.SetActive(true);
        }

        public void StartBattle()
        {
            BattleView.SetActive(true);
            ShopView.SetActive(false);
            EnemyController.Initialize(EnemyValues[Random.Range(0,EnemyValues.Count)]);
            OnStartTurn();
        }

        private void EnemyTurnEnd(object sender, EventArgs e)
        {
            OnStartTurn();
        }

        private void CreateCard(int i)
        {
            var cardModelFactoy = new CardModelFactory();
            var cardModel = cardModelFactoy.Model;
            var cardViewFactory = new CardViewFactory(cardsParent);
            var cardView = cardViewFactory.View;
            var cardControllerFactory = new CardControllerFactory(cardModel, cardView);
            var cardController = cardControllerFactory.Controller;
            CardModels.Add(cardModel);
            cardController.CardUsed += OnCardUsed;
        }

        private void CreatePlayer()
        {
            var playerModelFactory = new PlayerModelFactory();
            var playerModel = playerModelFactory.Model;
            var playerViewFactory = new PlayerViewFactory();
            var playerView = playerViewFactory.View;
            var playerControllerFactory = new PlayerControllerFactory(playerModel, playerView);
            var playerController = playerControllerFactory.Controller;
            playerController.PlayerManaChanged += OnPlayerManaChanged;
            PlayerController = playerController;
        }

        private void CreateEnemy()
        {
            var enemyModelFactory = new EnemyModelFactory();
            var enemyModel = enemyModelFactory.Model;
            var enemyViewFactory = new EnemyViewFactory();
            var enemyView = enemyViewFactory.View;
            var enemyControllerFactory = new EnemyControllerFactory(enemyModel, enemyView);
            var enemyController = enemyControllerFactory.Controller;
            enemyController.EnemyAttack += TakeEnemyAttack;
            EnemyController = enemyController;
        }

        private void OnPlayerManaChanged(object sender, EventArgs e)
        {
            foreach (var t in CardModels)
            {
                t.CanBeUsed = PlayerController.Mana() >= t.Cost;
            }
        }

        private void OnStartTurn()
        {
            PlayerController.ResetShield();
            int currentMana = PlayerController.Mana();
            PlayerController.Mana(-currentMana);
            PlayerController.Mana(PlayerValues.mana);
            foreach (var t in CardModels)
            {
                int n = Random.Range(0, CardValues.Count);
                t.Cost = CardValues[n].cost;
                t.Potency = CardValues[n].potency;
                t.Type = CardValues[n].type;
                t.SpellType = CardValues[n].spellType;
                t.CardUsed = false;
                t.CanBeUsed = PlayerController.Mana() >= t.Cost;
            }
        }

        public void EndTurn()
        {
            EnemyController.StartTurn();
        }

        private void OnCardUsed(object sender, ActionEventArgs e)
        {
            if (e.Type == ActionType.Attack && PlayerController.IsWeaken())
            {
                e.Potency -= e.Potency / 2;
            }
            if (e.Self)
            {
                PlayerController.TakeAction(e);
            }
            else
            {
                EnemyController.TakeAction(e);
            }
            PlayerController.Mana(-e.Cost);
        }

        void TakeEnemyAttack(object sender, ActionEventArgs e)
        {
            PlayerController.TakeAction(e);
        }
    }
}

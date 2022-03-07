using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace View
{
    public interface IEquipmentView
    {
        event EventHandler OnBuy;
        event EventHandler OnSell;
        event EventHandler OnEquip;
        event EventHandler OnUnequip;
        string Price { set; }
        string Effect { set; }
        Sprite Sprite { set; }
        bool Show { set; }
        bool Purchasable { set; }
        bool Discounted { set; }
        bool OnShop { set; }
        bool IsEquiped { set; }
        bool CanEquip { set; }
        void UpdateButtons();
    }
    public class EquipmentView : MonoBehaviour, IEquipmentView
    {
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI effectText;
        public Color baseColor, discountColor;
        public Image itemSprite;
        public GameObject description, mask, canBuy, canSell, canEquip, canUnequip;
        public Button buttonBuy, buttonSell, buttonEquip, buttonUnequip;
        private bool _isEquiped;
        private bool _onShop;
        private bool _canEquip;
        public event EventHandler OnBuy;
        public event EventHandler OnSell;
        public event EventHandler OnEquip;
        public event EventHandler OnUnequip;

        public string Price
        {
            set => priceText.text = value;
        }

        public string Effect
        {
            set => effectText.text = value;
        }

        public Sprite Sprite
        {
            set => itemSprite.sprite = value;
        }

        public bool Show
        {
            set => gameObject.SetActive(value);
        }

        public bool Purchasable
        {
            set
            {
                buttonBuy.interactable = value;
                mask.SetActive(!value);
            }
        }

        public bool Discounted
        {
            set => priceText.color = value ? discountColor : baseColor;
        }

        public bool OnShop
        {
            set => _onShop = value;
        }

        public bool IsEquiped
        {
            set => _isEquiped = value;
        }

        public bool CanEquip
        {
            set => _canEquip = value;
        }


        public void UpdateButtons()
        {
            canBuy.SetActive(_onShop);
            canSell.SetActive(!_onShop && !_isEquiped);
            canEquip.SetActive(!_onShop && !_isEquiped && _canEquip);
            canUnequip.SetActive(_isEquiped);
        }

        private void Start()
        {
            buttonBuy.onClick.AddListener(BuyClicked);
            buttonSell.onClick.AddListener(SellClicked);
            buttonEquip.onClick.AddListener(EquipClicked);
            buttonUnequip.onClick.AddListener(UnequipClicked);
        }

        private void BuyClicked()
        {
            OnBuy?.Invoke(this,EventArgs.Empty);
        }
        private void SellClicked()
        {
            OnSell?.Invoke(this,EventArgs.Empty);
        }
        private void EquipClicked()
        {
            OnEquip?.Invoke(this,EventArgs.Empty);
        }
        private void UnequipClicked()
        {
            OnUnequip?.Invoke(this,EventArgs.Empty);
        }
        

        public void MouseEnter()
        {
            description.SetActive(true);
        }

        public void MouseExit()
        {
            description.SetActive(false);
        }
    }
}

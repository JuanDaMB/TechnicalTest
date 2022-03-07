using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class CardClickedEventArgs : EventArgs
    {
    }
    
    public interface ICardView
    {
        event EventHandler<CardClickedEventArgs> OnClick;
 
        int Cost { set; }
        ActionType Type { set; }
        string Effect { set; }
        bool IsUsed { set; }
        bool CanBeUsed { set; }
        void DestroyObject();
    }
    public class CardView : MonoBehaviour, ICardView
    {
        public TextMeshProUGUI costText;
        public TextMeshProUGUI typeText;
        public TextMeshProUGUI effectText;
        public GameObject mask;
        public int Cost
        {
            set => costText.text = value.ToString();
        }

        public ActionType Type
        {
            set => typeText.text = value.ToString();
        }

        public string Effect
        {
            set => effectText.text = value;
        }

        public bool IsUsed
        {
            set => gameObject.SetActive(!value);
        }

        public bool CanBeUsed
        {
            set
            {
                button.interactable = value;
                mask.SetActive(!value);
            }
        }

        public void DestroyObject()
        {
            Destroy(gameObject);
        }

        public event EventHandler<CardClickedEventArgs> OnClick;
        public Button button;

        private void Start()
        {
            button.onClick.AddListener(Clicked);
        }

        private void Clicked()
        {
            var eventArgs = new CardClickedEventArgs();
            OnClick(this, eventArgs);
        }

        // void Update()
        // {
        //     if (!Input.GetMouseButtonDown(0)) return;
        //     var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     
        //     if (!Physics.Raycast(ray, out var hit) || hit.transform != transform) return;
        //     var eventArgs = new CardClickedEventArgs();
        //     OnClick(this, eventArgs);
        // }
    }
}

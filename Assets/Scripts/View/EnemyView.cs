using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public interface IEnemyView
    {
        string Health { set; }
        float HealthPercent { set; }
        int Shield { set; }
        int Weaken { set; }
        int Vulnerable { set; }
        int Invulnerable { set; }
        string TurnIntention { set; }
        bool IsAlive { set; }
    }
    
    public class EnemyView : MonoBehaviour, IEnemyView
    {
        public TextMeshProUGUI healthText;
        public Image healthBar;
        public TextMeshProUGUI shieldText;
        public GameObject shieldSymbol;
        public TextMeshProUGUI weakenText;
        public GameObject weakenSymbol;
        public TextMeshProUGUI vulnerableText;
        public GameObject vulneranbleSymbol;
        public TextMeshProUGUI invulnerableText;
        public GameObject invulneranbleSymbol;
        public TextMeshProUGUI turnIntention;

        public string Health
        {
            set => healthText.text = value;
        }

        public float HealthPercent
        {
            set => healthBar.fillAmount = value;
        }

        public int Shield
        {
            set
            {
                shieldSymbol.SetActive(value > 0);
                shieldText.text = value.ToString();
            }
        }


        public int Weaken
        {
            set
            {
                weakenSymbol.SetActive(value > 0);
                weakenText.text = value.ToString();
            }
        }

        public int Vulnerable
        {
            set
            {
                vulneranbleSymbol.SetActive(value > 0);
                vulnerableText.text = value.ToString();
            }
        }

        public int Invulnerable
        {
            set
            {
                invulneranbleSymbol.SetActive(value > 0);
                invulnerableText.text = value.ToString();
            }
        }

        public string TurnIntention
        {
            set => turnIntention.text = value;
        }

        public bool IsAlive
        {
            set => gameObject.SetActive(value);
        }
    }
}
using System.Collections;
using TMPro;
using UnityEngine;

namespace View
{
    public interface IInventoryView
    {
        int Gold { set; }
    }
    public class InventoryView : MonoBehaviour, IInventoryView
    {
        public Animator animator;
        private int _currentGold, _oldGold;
        private int _difference;
        public TextMeshProUGUI  goldText;
        public TextMeshProUGUI differenceGoldText;

        public int Gold
        {
            set
            {
                if (value != _currentGold)
                {
                    _difference = value - _currentGold;
                    differenceGoldText.text = Mathf.Sign(_difference) + "$ " + _difference;
                }

                _oldGold = _currentGold;
                _currentGold = value;
                animator.Play("MoneyChanged");
                StopCoroutine(ChangeGold());
                StartCoroutine(ChangeGold());
            }
        }

        private IEnumerator ChangeGold()
        {
            float t = 0f;
            while (t <= 1f)
            {
                goldText.text = "$ "+(int)Mathf.Lerp(_oldGold, _currentGold, t);
                yield return null;
                t += Time.deltaTime;
            }
            goldText.text = "$ "+_currentGold;
        }
    }
}

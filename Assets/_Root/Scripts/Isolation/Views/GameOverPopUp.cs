namespace Ai4Gamedev.MiniMax.Isolation.Views
{
    using TMPro;
    using UnityEngine;

    public class GameOverPopUp : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;
        
        public void Show(IPlayer winner)
        {
            text.text = string.Format(text.text, winner.Id);
            gameObject.SetActive(true);
        }
    }
}
using UnityEngine;

namespace Eliminate.Scripts
{
    public class GameSweet : MonoBehaviour
    {
        private int x;

        public int X
        {
            get => x;
            set => x = value;
        }

        private int y;

        public int Y
        {
            get => y;
            set => y = value;
        }

        private GameManager.SweetsType type;

        public GameManager.SweetsType Tyep
        {
            get => type;
        }

        public GameManager gameManager;

        public void Init(int x,int y,GameManager gameManager,GameManager.SweetsType type)
        {
            this.x = x;
            this.y = y;
            this.gameManager = gameManager;
            this.type = type;
        }
        
    }
}
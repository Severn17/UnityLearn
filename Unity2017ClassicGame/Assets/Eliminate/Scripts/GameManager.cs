using System;
using UnityEngine;

namespace Eliminate.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get => _instance;
            set => _instance = value;
        }

        public int xColumn;
        public int yRow;
        public GameObject gridPrefab;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            for (int x = 0; x < xColumn; x++)
            {
                for (int y = 0; y < yRow; y++)
                {
                    GameObject chocolate = Instantiate(gridPrefab, CorrectPosition(x, y), Quaternion.identity);
                    chocolate.transform.SetParent(transform);
                }
            }
        }

        public Vector3 CorrectPosition(int x,int y)
        {
            return new Vector3(transform.position.x - xColumn / 2f + x, transform.position.y + yRow / 2f - y);
        }
    }
}
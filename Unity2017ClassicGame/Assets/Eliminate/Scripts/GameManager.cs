using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Eliminate.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public enum SweetsType
        {
            EMPTY,
            NOMAL,
            BARRIER,
            ROW_CLEAR,
            COLUMN_CLEAR,
            RAINBOWCANDY,
            COUNT,//标记类型
        }

        public Dictionary<SweetsType, GameObject> sweetPrefabDic;
        [Serializable]
        public struct SweetsPrefab
        {
            public SweetsType type;
            public GameObject prefab;
        }

        public SweetsPrefab[] sweetsPrefabs;
        
        private static GameManager _instance;
        public static GameManager Instance
        {
            get => _instance;
            set => _instance = value;
        }

        public int xColumn;
        public int yRow;
        public GameObject gridPrefab;

        private GameObject[,] sweets;

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            sweetPrefabDic = new Dictionary<SweetsType, GameObject>();
            for (int i = 0; i < sweetsPrefabs.Length; i++)
            {
                if (!sweetPrefabDic.ContainsKey(sweetsPrefabs[i].type))
                {
                    sweetPrefabDic.Add(sweetsPrefabs[i].type,sweetsPrefabs[i].prefab);
                }
            }
            
            for (int x = 0; x < xColumn; x++)
            {
                for (int y = 0; y < yRow; y++)
                {
                    GameObject chocolate = Instantiate(gridPrefab, CorrectPosition(x, y), Quaternion.identity);
                    chocolate.transform.SetParent(transform);
                }
            }
            
            sweets = new GameObject[xColumn,yRow];
            for (int x = 0; x < xColumn; x++)
            {
                for (int y = 0; y < yRow; y++)
                {
                    sweets[x,y] = Instantiate(sweetPrefabDic[SweetsType.NOMAL], CorrectPosition(x, y), Quaternion.identity);
                    sweets[x,y].transform.SetParent(transform);
                }
            }
        }

        public Vector3 CorrectPosition(int x,int y)
        {
            return new Vector3(transform.position.x - xColumn / 2f + x, transform.position.y + yRow / 2f - y);
        }
    }
}
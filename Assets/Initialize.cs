using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class Initialize : MonoBehaviour
    {
        public GameObject prefab;
        public Transform LeadPoint;
        public int count;
        // Start is called before the first frame update
        void Start()
        {
            UnityEngine.Random.InitState(0);
            //下面两行为了同时生成不一样的随机种子，因为Agent要同时创建多个
            byte[] buffer = Guid.NewGuid().ToByteArray();//生成字节数组
            int iRoot = BitConverter.ToInt32(buffer, 0);//利用BitConvert方法把字节数组转换为整数
            UnityEngine.Random.InitState(iRoot);
            // var people = GameObject.Instantiate(prefab,pos);
            
        }

        // Update is called once per frame
        void Update()
        {
            if(count!=0){
                float x = UnityEngine.Random.Range(-10, 10);
                float z = UnityEngine.Random.Range(-5, 5);
                Vector3 pos = new Vector3(x,0.2f,z);
                //实例化
                var people = Instantiate(prefab,pos,Quaternion.identity);
                people.GetComponent<AICharacterControl>().SetTarget(LeadPoint);
                --count;
            }
            
        }
    }
}

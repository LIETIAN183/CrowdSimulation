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
        public Transform SafePlace;
        private NormalDistribution normal;
        public int count;
        public GameObject fire;
        // Start is called before the first frame update
        void Start()
        {
            UnityEngine.Random.InitState(0);
            //下面两行为了同时生成不一样的随机种子，因为Agent要同时创建多个
            byte[] buffer = Guid.NewGuid().ToByteArray();//生成字节数组
            int iRoot = BitConverter.ToInt32(buffer, 0);//利用BitConvert方法把字节数组转换为整数
            UnityEngine.Random.InitState(iRoot);
            normal = GetComponent<NormalDistribution>();
            Invoke("invokeTest",5.0f);
        }

        private void invokeTest(){
            fire.SetActive(true);
        }
        // Update is called once per frame
        void Update()
        {
            if(count!=0){
                //随机生成Agent的初始化位置
                float x = UnityEngine.Random.Range(-10, 10);
                float z = UnityEngine.Random.Range(-5, 5);
                Vector3 pos = new Vector3(x,0.2f,z);
                //实例化
                // string name = "Agent" + count;
                var people = Instantiate(prefab,pos,Quaternion.identity);
                people.name = "Agent" + count;//每个Agent命名
                // people
                people.GetComponent<AICharacterControl>().SetTarget(LeadPoint);//设置Agent闲逛时的目标物体集合
                people.GetComponent<AICharacterControl>().SetSafePlace(SafePlace);//设置Agent闲逛时的目标物体集合
                people.GetComponent<AICharacterControl>().SetNeuroticism(normal.NextDouble(0,1));//初始化目标物体的Neuroticism属性
                --count;//控制Agent个数
            }
            
        }
    }
}

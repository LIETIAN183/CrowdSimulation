using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

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
        public List<AICharacterControl> Agents;

        [Header("UIControl")]
        public Slider sd;
        public Text timeStepText;
        public InputField setSij;
        public InputField setRji;
        public InputField setK1;
        public InputField setK2;

        // Start is called before the first frame update
        void Start()
        {
            UnityEngine.Random.InitState(0);
            //下面两行为了同时生成不一样的随机种子，因为Agent要同时创建多个
            byte[] buffer = Guid.NewGuid().ToByteArray();//生成字节数组
            int iRoot = BitConverter.ToInt32(buffer, 0);//利用BitConvert方法把字节数组转换为整数
            UnityEngine.Random.InitState(iRoot);
            normal = GetComponent<NormalDistribution>();
            generate();
            // Invoke("invokeTest",5.0f);
        }

        public void activeFire(){
            fire.SetActive(true);
        }

        public void stopFire(){
            fire.SetActive(false);
        }

        private void generate(){
            for(int i = 1;i<=count;++i){
                //随机生成Agent的初始化位置
                float x = UnityEngine.Random.Range(-10, 10);
                float z = UnityEngine.Random.Range(-5, 5);
                Vector3 pos = new Vector3(x,0.2f,z);
                //实例化
                // string name = "Agent" + count;
                var people = Instantiate(prefab,pos,Quaternion.identity);
                
                people.name = "Agent" + i;//每个Agent命名
                var aicontrol = people.GetComponent<AICharacterControl>();
                aicontrol.SetTarget(LeadPoint);//设置Agent闲逛时的目标物体集合
                aicontrol.SetSafePlace(SafePlace);//设置Agent闲逛时的目标物体集合
                aicontrol.SetNeuroticism(normal.NextDouble(0,1));//初始化目标物体的Neuroticism属性
                Agents.Add(aicontrol);
            }
        }

        public void timeStepControl(){
            var value = sd.GetComponent<Slider>().value;
            foreach (var item in Agents)
            {
                item.SetTimeStep(value);
            }
            timeStepText.text = "TimeStep:"+value;
        }

        public void k1control(){
            int value;
            if(setK1.text==""){
                value=300;
            }else{
                value = int.Parse(setK1.text);
            }
            
            foreach (var item in Agents)
            {
                item.setK1(value);
            }
        }

        public void k2control(){
            int value;
            if(setK2.text==""){
                value=200;
            }else{
                value = int.Parse(setK2.text);
            }
            
            foreach (var item in Agents)
            {
                item.setK2(value);
            }
        }
        
        public void Sijcontrol(){
            double value;
            if(setSij.text==""){
                value=0.7;
            }else{
                value = double.Parse(setSij.text);
            }
            
            foreach (var item in Agents)
            {
                item.setSij(value);
            }
        }

        public void Rjicontrol(){
            double value;
            if(setRji.text==""){
                value=0.7;
            }else{
                value = double.Parse(setRji.text);
            }
            
            foreach (var item in Agents)
            {
                item.setRji(value);
            }
        }


        // Update is called once per frame
        void Update()
        {
            
        }
    }
}

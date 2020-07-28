using System;
using UnityEngine;
using System.Collections;


namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Transform target;                                    // target to aim for
        private Transform[] LeadPoints;
        //符合N(1,0.25)
        public double neuroticism;                      //正态分布初始化neuroticism，Personality module中的一个元素，其他的没有看到算法，没有实现
        private double fear,threshold,Sij=0.9;              //设置情绪激活的阈值为1-neuroticism，neuroticism越高，情绪阈值越低，越容易发生恐慌行为
        private double Rji = 0.9;
        private int k1,k2;

        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();
            

	        agent.updateRotation = false;
	        agent.updatePosition = true;

            //获得所有导航点
            if(target!=null){
                LeadPoints = target.GetComponentsInChildren<Transform>();
            }

            //设置一个随机种子
            UnityEngine.Random.InitState(0);
            //下面两行为了同时生成不一样的随机种子，因为Agent要同时创建多个
            byte[] buffer = Guid.NewGuid().ToByteArray();//生成字节数组
            int iRoot = BitConverter.ToInt32(buffer, 0);//利用BitConvert方法把字节数组转换为整数
            UnityEngine.Random.InitState(iRoot);

            if (LeadPoints.Length>1)
                ResetDestination();//第一次设置初始目标点
        }


        private void Update()
        {
            if(fear>threshold){
                character.SetWalk(false);
            }
            if (agent.remainingDistance > agent.stoppingDistance)
                character.Move(agent.desiredVelocity, false, false);
            else
                ResetDestination();
                // character.Move(Vector3.zero, false, false);
            
        }

        //设置目标物体的位置为目的地
        private void ResetDestination(){
            agent.SetDestination(LeadPoints[UnityEngine.Random.Range(1,LeadPoints.Length)].position);
        }


        //设置Taget，目标物体
        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        //修改不同类别的Agent的颜色
        public void SetNeuroticism(double neuroticism){
            this.neuroticism=neuroticism;
            fear = neuroticism*0.1;     //初始化fear为neuroticism的一半
            threshold = 1-neuroticism;
            Color color = Color.grey;
            if(neuroticism<=0){
                
            }else if(neuroticism<=0.3){
                color=Color.blue;
            }else if(neuroticism<=0.7){
                color=Color.green;
            }else if(neuroticism<=1){
                color=Color.yellow;
            }
            // GameObject.Find("EthanBody").GetComponent<SkinnedMeshRenderer>().material;//使用GameObject.Find查找的是全局对象
            foreach (Transform t in this.GetComponentsInChildren<Transform>())//更改Agent颜色
            {
                if(t.name=="EthanBody")
                {
                    t.GetComponent<Renderer>().material.color = color; //使用Renderer和SkinnedMeshRenderer均可
                    break;
                }
            }
        }

        public double GetEmotion(){
            if(fear>threshold){
                return fear;
            }
            return 0;
        }
    }
}

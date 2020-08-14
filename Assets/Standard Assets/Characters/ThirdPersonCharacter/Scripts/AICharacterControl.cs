using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Transform target;                                    // target to aim for
        public Transform safePlace;
        private Transform[] LeadPoints;
        //符合N(1,0.25)
        public double neuroticism,fear,threshold;                      //正态分布初始化neuroticism，Personality module中的一个元素，其他的没有看到算法，没有实现
        private double deltaFear=0;              //设置情绪激活的阈值为1-neuroticism，neuroticism越高，情绪阈值越低，越容易发生恐慌行为
        public double Sij,Rji;
        public int k1,k2;//k1=300,k2=200;k=k1+k2
        private bool activeFear=false;
        // private bool activeRecover = false;

        public float timeStep=0.1f;
        public float timeSteptmp=0f;
        private List<Collider> colliders = new List<Collider>();
        public Collider[] show;
        private Vector3 offset = new Vector3(0.0f,0.1f,0.0f);

        private Renderer renderer;
        private Color colorBack;

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

            
            Sij=0.7;
            Rji=0.7;
        }

        private void Update()//更新动画
        {
            if(!activeFear){
                Collider[] cs = Physics.OverlapSphere(this.transform.position, 1,1 << LayerMask.NameToLayer("Dangerous"));
                if(cs.Length>0){
                    fear = 0.81;
                }
            }

            HashSet<Collider> set = new HashSet<Collider>(colliders);
            colliders = new List<Collider>(set);
            show = colliders.ToArray();
            
            //colliders = Physics.OverlapSphere(this.transform.position+offset, 1f);
            timeSteptmp+=Time.deltaTime;
            if(timeSteptmp>timeStep){
                detectEmotion();
                timeSteptmp=0f;
            }
            
            if (agent.remainingDistance > agent.stoppingDistance){
                if(fear<0.35)
                    character.setm_MoveSpeedMultiplier(0.015f);
                else if(fear<0.8)
                    character.setm_MoveSpeedMultiplier((10.5f*(float)fear-2.4f)*0.01f);
                else if(fear<0.9)
                    character.setm_MoveSpeedMultiplier(0.06f);
                else
                    character.setm_MoveSpeedMultiplier(0f);

                character.Move(agent.desiredVelocity, false, false);
            }
            else{
                //不恐慌状态时才随机切换目的地
                if(!activeFear)
                    ResetDestination();
                else
                    character.Move(Vector3.zero, false, false);
            }   

            
        }

        private void detectEmotion(){
            if(!activeFear){
                foreach (Collider c in colliders){
                    var ts = c.gameObject.transform;
                    double dis = Vector3.Distance(this.transform.position,ts.position);//该方法的判断的是1m内是否有物体相交，但可能实际上两物体坐标距离超过1f
                    deltaFear+=(1-(1/(1+Math.Exp(-dis))))*ts.GetComponent<AICharacterControl>().GetEmotion()*Rji*fear;
                }
            }
        }

        private void LateUpdate() {
            
            colliders.Clear();
            
            fear+=deltaFear;
            deltaFear=0;
            //情绪计算完毕后为恐慌情绪，行为改为奔跑，Agent颜色改为红色
            if(fear>threshold&&!activeFear){
                activeFear=true;
                character.SetWalk(false);//Agent动作改为跑步
                renderer.material.color = Color.red;
                //Agent跑向安全位置
                SetTarget(safePlace);
                LeadPoints = target.GetComponentsInChildren<Transform>();
                // ResetDestination();
                if(Vector3.Distance(this.transform.position,LeadPoints[1].position)<Vector3.Distance(this.transform.position,LeadPoints[2].position)){
                    agent.SetDestination(LeadPoints[1].position);
                }else{
                    agent.SetDestination(LeadPoints[2].position);
                }
                //k时间后恢复为可情绪感染状态
                Invoke("recoverStep1",k1);
                Invoke("recoverStep2",k1+k2);
            }
        }

        private void OnTriggerStay(Collider other) {
            if(other is SphereCollider && other.tag == "Agent"){
                colliders.Add(other);
            }
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

        public void SetSafePlace(Transform safePlace){
            this.safePlace=safePlace;
        }

        

        //初始化参数，并修改不同类别的Agent的颜色
        public void SetNeuroticism(double neuroticism){
            this.neuroticism=neuroticism;
            fear = neuroticism*0.1;     //初始化fear为neuroticism的一半
            threshold = 1-neuroticism;
            if(threshold>0.8)
                threshold = 0.8;
            if(threshold<0.4){
                threshold = 0.4;
            }

            this.k1 = 300 + (int)((neuroticism-0.5)*200);
            k2=200;
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
                    renderer =  t.GetComponent<Renderer>(); //使用Renderer和SkinnedMeshRenderer均可
                    break;
                }
            }
            renderer.material.color = color;
            colorBack =color;
        }

        //提供当前情绪供其他Agent获取
        public double GetEmotion(){
            if(activeFear){
                return Sij;
            }
            return 0;
        }

        //情绪恢复
        private void recoverStep1(){
            fear= neuroticism*0.1;
            character.SetWalk(true);
            renderer.material.color = colorBack;
            deltaFear=0;
        }
        
        private void recoverStep2(){
            activeFear = false;
        }

        //UI设置界面
        public void SetTimeStep(float timeStep){
            this.timeStep=timeStep;
        }

        public void setK1(int k1){
            this.k1=k1;
        }

        public void setK2(int k2){
            this.k2=k2;
        }

        public void setRji(double Rji){
            this.Rji=Rji;
        }

        public void setSij(double Sij){
            this.Sij=Sij;
        }
    }
}
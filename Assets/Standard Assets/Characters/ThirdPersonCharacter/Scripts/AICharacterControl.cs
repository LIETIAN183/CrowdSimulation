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
        public Transform safePlace;
        private Transform[] LeadPoints;
        //符合N(1,0.25)
        public double neuroticism,fear,threshold;                      //正态分布初始化neuroticism，Personality module中的一个元素，其他的没有看到算法，没有实现
        private double deltaFear;              //设置情绪激活的阈值为1-neuroticism，neuroticism越高，情绪阈值越低，越容易发生恐慌行为
        public double Sij,Rji;
        public int k;//k1=300,k2=200;k=k1+k2
        private bool activeFear=false;
        // private bool activeRecover = false;

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

            deltaFear=0;
            Sij=0.7;
            Rji=0.7;
            k=500;
        }


        private void Update()//更新动画
        {
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, 1f,1 << LayerMask.NameToLayer("Dangerous"));
            
            if(colliders.Length>0&&!activeFear){
                fear = 0.89;
                Debug.Log(this.transform.name+":"+colliders[0].gameObject.name);
            }

            if (agent.remainingDistance > agent.stoppingDistance){
                character.Move(agent.desiredVelocity, false, false);
                if(fear<0.35)
                    character.setm_MoveSpeedMultiplier(0.015f);
                else if(fear<0.8){
                    character.setm_MoveSpeedMultiplier((10.5f*(float)fear-2.4f)*0.01f);
                }else if(fear<0.9){
                    character.setm_MoveSpeedMultiplier(0.06f);
                }else
                    character.setm_MoveSpeedMultiplier(0f);
            }

            else{
                //不恐慌状态时才随机切换目的地
                if(!activeFear){
                    ResetDestination();
                }else{
                    character.Move(Vector3.zero, false, false);
                }
                
            }
                
                
        }

        //设置0.5s为一个时间步长
        private void FixedUpdate() {
            
            if(!activeFear){
                //获取感知范围内的所有Agent的collider
                Collider[] colliders = Physics.OverlapSphere(this.transform.position, 1f,1 << LayerMask.NameToLayer("Agent"));
                 for (int i = 0; i < colliders.Length; i++){
                    var gameObject = colliders[i].gameObject;
                    if(this.name == gameObject.name){//删除自身
                        continue;
                    }
                    double dis = Vector3.Distance(this.transform.position,gameObject.transform.position);//该方法的判断的是1m内是否有物体相交，但可能实际上两物体坐标距离超过1f
                    deltaFear+=(1-(1/(1+Math.Exp(-dis))))*gameObject.transform.GetComponent<AICharacterControl>().GetEmotion()*Rji;
                    // Debug.Log(this.name+"|"+gameObject.transform.name+"|"+gameObject.transform.GetComponent<AICharacterControl>().GetEmotion()+"|"+(1-(1/(1+Math.Exp(-dis))))+"|"+(1-(1/(1+Math.Exp(-dis))))*gameObject.transform.GetComponent<AICharacterControl>().GetEmotion()*Rji);
                 }
            }
            // Debug.Log ("FixedUpdate:"+this.name);  
            StartCoroutine (AfterFixedUpdate());
        }

        IEnumerator AfterFixedUpdate()
        {
            // Debug.Log ("AfterFixedUpdate:"+this.name);
            yield return new WaitForFixedUpdate();//加这一行后下面的代码会在所有FixedUpdate()完成后再执行
            // Debug.Log ("AfterYield:"+this.name);
            fear+=deltaFear;
            
            deltaFear=0;
            //情绪计算完毕后为恐慌情绪，行为改为奔跑，Agent颜色改为红色
            if(fear>threshold&&!activeFear){
                activeFear=true;
                character.SetWalk(false);//Agent动作改为跑步
                foreach (Transform t in this.GetComponentsInChildren<Transform>())//情绪激活，变更颜色为红色
                {
                    if(t.name=="EthanBody")
                    {
                        t.GetComponent<Renderer>().material.color = Color.red; //使用Renderer和SkinnedMeshRenderer均可
                        break;
                    }
                }
                //Agent跑向安全位置
                SetTarget(safePlace);
                LeadPoints = target.GetComponentsInChildren<Transform>();
                ResetDestination();
                //k时间后恢复为可情绪感染状态

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
            if(threshold<0.4)
                threshold = 0.4;
            k += (int)((neuroticism-0.5)*100);
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

        //提供当前情绪供其他Agent获取
        public double GetEmotion(){
            if(activeFear){
                return fear*Sij;
            }
            return 0;
        }
    }
}

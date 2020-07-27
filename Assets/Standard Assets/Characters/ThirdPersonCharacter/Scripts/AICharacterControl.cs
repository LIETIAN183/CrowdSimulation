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
            if (agent.remainingDistance > agent.stoppingDistance)
                character.Move(agent.desiredVelocity, false, false);
            else
                ResetDestination();
                // character.Move(Vector3.zero, false, false);
            
        }

        private void ResetDestination(){
            agent.SetDestination(LeadPoints[UnityEngine.Random.Range(1,LeadPoints.Length)].position);
        }


        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}

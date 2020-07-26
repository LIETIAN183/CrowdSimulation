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
            LeadPoints = target.GetComponentsInChildren<Transform>();

            //设置一个随机种子
            UnityEngine.Random.InitState(0);
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
            // Random.Range(4,10);

            if (LeadPoints.Length>1)
                agent.SetDestination(LeadPoints[UnityEngine.Random.Range(1,LeadPoints.Length)].position);
        }


        private void Update()
        {
            if (agent.remainingDistance > agent.stoppingDistance)
                character.Move(agent.desiredVelocity, false, false);
            else
                agent.SetDestination(LeadPoints[UnityEngine.Random.Range(1,LeadPoints.Length)].position);
                // character.Move(Vector3.zero, false, false);
        }


        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}

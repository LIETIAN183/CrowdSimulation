using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fire : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    private void FixedUpdate() {
        if(GetComponent<SphereCollider>().radius<10){
            GetComponent<SphereCollider>().radius+=0.003f;
        }

        if(GetComponent<NavMeshObstacle>().radius<10){
            GetComponent<NavMeshObstacle>().radius+=0.003f;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class UIcontrol : MonoBehaviour
    {
        public Slider sd;
        public  int count;
        // Start is called before the first frame update
        void Start()
        {
            count = GameObject.Find("People").GetComponent<Initialize>().count;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void timeStepControl(){
            for(int i =1;i<=count;++i){
                GameObject.Find("Agent"+i.ToString()).GetComponent<AICharacterControl>().SetTimeStep(sd.GetComponent<Slider>().value);
            }
            
        }
    }
}
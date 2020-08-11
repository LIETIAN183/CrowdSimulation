using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NormalDistribution : MonoBehaviour
{
    private double Mu=0.5,Sigma=0.25;
    private System.Random rand;

    public void setParameter(double Mu, double Sigma){
        this.Mu = Mu;
        this.Sigma = Sigma;
    }

    public double NextDouble()
    {
        double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles 
        double u2 = rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1) 
        double randNormal = Mu + Sigma * randStdNormal; //random normal(mean,stdDev^2)

        return randNormal;
    }

    public double NextDouble(double min,double max)
    {
        double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles 
        double u2 = rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1) 
        double randNormal = Mu + Sigma * randStdNormal; //random normal(mean,stdDev^2)

        if(randNormal<min||randNormal>max){
            return NextDouble(min,max);
        }
        return randNormal;
    }
    
    private void Awake() {
        rand = new System.Random((int)DateTime.UtcNow.Ticks);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

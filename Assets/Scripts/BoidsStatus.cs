using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoidsStatus : MonoBehaviour
{

	private int amount = 100;
	public int Amount { get {return amount;} set{amount = value;} }

	private float speed;
	public float Speed { get{return speed;} set{speed = value;} }
	public Text speedtext;
	public Slider speedslider;

	private float max_vel = 0.1f;
	public float Max_vel { get{return max_vel;} set{max_vel = value;}}

	private float min_vel = 0.1f;
	public float Min_vel { get{return min_vel;} set{min_vel = value;} }

	private float sight = 5;
	public float Sight { get{return sight;} set{sight = value;} }

	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update()
	{
		Speed = speedslider.value * 1.5f;
		speedtext.text = "SPEED:" + Speed.ToString("f1");
	}

	public void SpeedChange(float value)
	{
		Speed = value * 10;
	}
}

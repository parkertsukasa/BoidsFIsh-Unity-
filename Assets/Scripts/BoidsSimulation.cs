using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsSimulation : MonoBehaviour
{

	private BoidsStatus status;
	public GameObject fishpref;//魚のプレハブ

	private GameObject[] fish;
	private Vector3[] fishmove;

	// Use this for initialization
	void Start()
	{
		status = GetComponent<BoidsStatus>();
		Init();
	}
	
	// Update is called once per frame
	void Update()
	{
		for (int i = 0; i < status.Amount; i++)
		{
			Cruise(i);
		}
	}

	/* ----------- Init () ------------
   * 初期化 魚の群れを作り出す
   */
	private void Init()
	{
		System.Array.Resize(ref fish, status.Amount);
		System.Array.Resize(ref fishmove, status.Amount);


		for (int i = 0; i < status.Amount; i++)
		{
			Vector3 pos = new Vector3(Random.Range(-4.5f, 4.5f), 0, Random.Range(-4.5f, 4.5f));
			Quaternion rot = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
			fish[i] = Instantiate(fishpref, pos, rot) as GameObject;
		}
	}

	/* ---------- Cruise ----------
   * 巡回
   */
	void Cruise(int i)
	{

		//それぞれの要素のバランス
		float factor_cohe = 1.0f;
		float factor_sepa = 3.0f;
		float factor_alig = 3.0f;

		float factor = factor_cohe + factor_sepa + factor_alig;
		fishmove[i] = ((factor_cohe * Cohesion(i)) + (factor_sepa * Separate(i)) + (factor_alig * Alignment(i))) / factor;

		fishmove[i] += fish[i].transform.forward * status.Speed * Time.deltaTime;//前進分を追加

		float velocity = fishmove[i].magnitude;

		//一定より早すぎる場合は正規化して切り詰める
		if (velocity > status.Max_vel)
			fishmove[i] = fishmove[i].normalized * status.Max_vel;

		//移動
		fish[i].transform.position += fishmove[i];

		//向きを揃える
		fish[i].transform.rotation = Quaternion.LookRotation(fishmove[i]);

	}
		 

	/* ---------- Cohesipn ----------
   * 集結 : 付近の個体の中心地を求めて向かう
   */
	Vector3 Cohesion(int i)
	{
		Vector3 center = Vector3.zero;

		int near = 0;

		for (int j = 0; j < status.Amount; j++)
		{
			if (i != j)//自分は除外
			{
				float dist = Vector3.Distance(fish[i].transform.position, fish[j].transform.position);
				if (dist < status.Sight)//視界の内側にいたら
				{
					near += 1;
					center += fish[j].transform.position;
				}
			}
		}
			
		Vector3 move = Vector3.zero;

		if(near > 0)
		{
			center /= near;//全個体の平均位置
			move = (center - fish[i].transform.position);//平均位置と自分の位置の差を移動量とする
		}

		return move;
	}

	/* ---------- Separate ----------
   * 分離 : 最も近い個体の反対方向へと向かう
   */
	Vector3 Separate(int i)
	{
		Vector3 move = Vector3.zero;

		int near = 0;//近くにいる魚の数

		for (int j = 0; j < status.Amount; j++)
		{
			if (j != i)//自分は除外
			{
				float dist = Vector3.Distance(fish[i].transform.position, fish[j].transform.position);
				if (dist < status.Sight)//視界の内側にいたら
				{
					near += 1;
					Vector3 diff = fish[j].transform.position - fish[i].transform.position;
					move += diff.normalized * 1.0f / (diff.magnitude * diff.magnitude) * -1;
				}
			}
		}

		AvoidWall(i,ref near, ref move);
			
		if(near > 0)
			move /= near;

		return move;
	}

	/* ---------- AvoidWall ----------
	 * 壁を避ける、自分の位置と壁の直行する位置に他の個体がいるとみなして、そいつを避けるべく振る舞う
	 */
	void AvoidWall (int i, ref int near, ref Vector3 move)
	{
		Vector3[] Wallfish = new Vector3[4];
		Wallfish[0] = new Vector3 (-5.0f, fish[i].transform.position.y, fish[i].transform.position.z);
		Wallfish[1] = new Vector3 (5.0f, fish[i].transform.position.y, fish[i].transform.position.z);
		Wallfish[2] = new Vector3 (fish[i].transform.position.x, fish[i].transform.position.y, -5.0f);
		Wallfish[3] = new Vector3 (fish[i].transform.position.x, fish[i].transform.position.y, 5.0f);

		for(int j = 0; j < Wallfish.Length; j++)
		{
			float dist = Vector3.Distance(fish[i].transform.position, Wallfish[j]);
			if (dist < status.Sight)//視界の内側にいたら
			{
				near += 1;
				Vector3 diff = Wallfish[j] - fish[i].transform.position;
				move += diff.normalized * 1.0f / (diff.magnitude * diff.magnitude) * -1;
			}
		}
	}


	/* ---------- Alignment ----------
   * 整列 : 付近の個体の移動量の平均を返す
   */
	Vector3 Alignment(int i)
	{
		Vector3 move = Vector3.zero;
		Vector3 average = Vector3.zero;

		int near = 0;

		for (int j = 0; j < status.Amount; j++)
		{
			if (i != j)//自分は除外
			{
				float dist = Vector3.Distance(fish[i].transform.position, fish[j].transform.position);
				if (dist < status.Sight)//視界の内側にいたら
				{
					near += 1;
					average += fishmove[j];
				}
			}
		}

		if (near > 0)
		{
			average /= near;
			move = average - fishmove[i];
		}

		return move;
	}

}

using System;
using Sandbox;

public sealed class Enemy1 : Component
{	
	[Property]
	[Category("Components")]
	public CharacterController Controller { get; set; }

	/// <summary>
	/// Enemy minimum speed.
	/// </summary>
	[Property]
	[Category("Stats")]
	public int MinSpeed { get; set; } = 80;

	/// <summary>
	/// Enemy maximun speed.
	/// </summary>
	[Property]
	[Category("Stats")]
	public int MaxSpeed { get; set; } = 350;

	/// <summary>
	/// Enemy rotate .
	/// </summary>
	[Property]
	[Category( "Stats" )]
	[Range(-90f, 0f, 1f )]
	public int MinRotateOffset { get; set; } = -45;

	[Property]
	[Category( "Stats" )]
	[Range( 0f, 90f, 1f )]
	public int MaxRotateOffset { get; set; } = 45;

	/// <summary>
	/// If the enemy is too far away from Vector3.Zero, destroy it.
	/// </summary>
	[Property]
	[Category( "Stats" )]
	public float DestroyDistance { get; set; } = 500f;

	protected override void OnStart()
	{
		// EnemyInitialize();
	}

	protected override void OnUpdate()
	{	
		if (Transform.Position.Distance(Vector3.Zero) >= DestroyDistance)
		{	
			GameObject.Destroy();
		}
	}

	protected override void OnFixedUpdate()
	{
		Controller.Move();
	}

	public void EnemyInitialize(int MinRotateOffset, int MaxRotateOffset)
	{
		// 获取场景内玩家方向并且朝向玩家
		var player = Scene.GetAllComponents<DodgePlayer>().FirstOrDefault();
		if ( player == null ) return;

		var targetPosition = player.Transform.Position;

		// 生成的时候把敌人的朝向看向玩家的位置并随机化角度
		Vector3 targetDirection = targetPosition - Transform.Position;
		targetDirection.z = Transform.Position.z;
		Transform.Rotation = Rotation.LookAt( targetDirection );
		var randomOffsetAngle = Random.Shared.Float( MinRotateOffset, MaxRotateOffset );
		Transform.Rotation = Rotation.FromYaw( Transform.Rotation.Yaw() + randomOffsetAngle );
	}
	public void EnemyMove(int MinSpeed, int MaxSpeed)
	{
		int randomSpeed = Random.Shared.Int(MinSpeed, MaxSpeed);
		// 将敌人朝自己的正前方发射出去
		Controller.Punch( Transform.Rotation.Forward * randomSpeed );
	}
}

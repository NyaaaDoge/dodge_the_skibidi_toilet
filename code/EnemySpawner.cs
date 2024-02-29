using Sandbox;
using System;

public sealed class EnemySpawner : Component
{	
	[Property]
	[Category( "Components" )]
	public GameObject Enemy { get; set; }

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
	/// Enemy will spawn at the lines randomly.
	/// </summary>
	[Property]
	[Category( "Components" )]
	public Vector3 EnemySpawnMarker1 { get; set; }

	/// <summary>
	/// Enemy will spawn at the lines randomly.
	/// </summary>
	[Property]
	[Category( "Components" )]
	public Vector3 EnemySpawnMarker2 { get; set; }

	/// <summary>
	/// Enemy will spawn at the lines randomly.
	/// </summary>
	[Property]
	[Category( "Components" )]
	public Vector3 EnemySpawnMarker3 { get; set; }

	/// <summary>
	/// Enemy will spawn at the lines randomly.
	/// </summary>
	[Property]
	[Category( "Components" )]
	public Vector3 EnemySpawnMarker4 { get; set; }

	/// <summary>
	/// An enemy will spawn after the time (second)
	/// </summary>
	[Property]
	[Category( "Components" )]
	[Range(0.1f, 30f, 0.1f)]
	public float EnemySpawnTimeEach {  get; set; }

	[Property]
	public bool EnableEnemySpawning {  get; set; } = true;

	protected override void DrawGizmos()
	{
		// 为四个marker画出一个完整的矩形
		Gizmo.Draw.Line( EnemySpawnMarker1, EnemySpawnMarker2 );
		Gizmo.Draw.Line( EnemySpawnMarker2, EnemySpawnMarker3 );
		Gizmo.Draw.Line( EnemySpawnMarker3, EnemySpawnMarker4 );
		Gizmo.Draw.Line( EnemySpawnMarker4, EnemySpawnMarker1 );
	}
	protected override void OnUpdate()
	{

	}

	public TimeSince Timer;
	protected override void OnAwake()
	{
		Timer = 0f;
	}

	protected override void OnFixedUpdate()
	{	
		// 计时器，每隔几秒就在随机路径上生成一次敌人
		if ( !EnableEnemySpawning ) return;
		if ( Timer >= EnemySpawnTimeEach )
		{
			SpawnEnemy();
			Timer = 0;
		}
	}

	private void SpawnEnemy()
	{	
		int edgeIndex = Random.Shared.Int( 0,3 );

		switch (edgeIndex)
		{	
			case 0:
				GenerateRandomEnemyOnLine( EnemySpawnMarker1, EnemySpawnMarker2 );
				break;
			case 1:
				GenerateRandomEnemyOnLine( EnemySpawnMarker2, EnemySpawnMarker3 );
				break;
			case 2:
				GenerateRandomEnemyOnLine( EnemySpawnMarker3, EnemySpawnMarker4 );
				break;
			case 3:
				GenerateRandomEnemyOnLine( EnemySpawnMarker4, EnemySpawnMarker1 );
				break;
		}
		
	}

	private void GenerateRandomEnemyOnLine( Vector3 startPoint, Vector3 endPoint )
	{	
		// 在四条边上选取随机的点
		float randomRatio = Random.Shared.Float( 0f, 1f );
		Vector3 randomPoint = startPoint + randomRatio * (endPoint - startPoint);
		// Log.Info($"Enemy spawn at randomPoint: {randomPoint}");
		// 设置生成怪物的max和min速度
		var newEnemy = Enemy.Clone( randomPoint );
		var newEnemyComponent = newEnemy.Components.Get<Enemy1>();
		newEnemyComponent.EnemyInitialize( MinRotateOffset, MaxRotateOffset );
		newEnemyComponent.EnemyMove( MinSpeed, MaxSpeed );
	}

}

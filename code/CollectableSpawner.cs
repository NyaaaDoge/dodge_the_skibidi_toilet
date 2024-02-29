using System;
using Sandbox;

public sealed class CollectableSpawner : Component
{	
	[Property]
	[Category( "Components" )]
	public GameObject Collectable { get; set; }

	/// <summary>
	/// Collectable will spawn at the quad randomly.
	/// </summary>
	[Property]
	[Category( "Components" )]
	public Vector3 CollectableSpawnMarker1 { get; set; }

	/// <summary>
	/// Collectable will spawn at the quad randomly.
	/// </summary>
	[Property]
	[Category( "Components" )]
	public Vector3 CollectableSpawnMarker2 { get; set; }

	/// <summary>
	/// Collectable will spawn at the quad randomly.
	/// </summary>
	[Property]
	[Category( "Components" )]
	public Vector3 CollectableSpawnMarker3 { get; set; }

	/// <summary>
	/// Collectable will spawn at the quad randomly.
	/// </summary>
	[Property]
	[Category( "Components" )]
	public Vector3 CollectableSpawnMarker4 { get; set; }

	/// <summary>
	/// An Collectable will spawn after the time (second)
	/// </summary>
	[Property]
	[Category( "Components" )]
	[Range(0.1f, 60f, 0.5f)]
	public float CollectableSpawnTimeEach {  get; set; } = 5f;

	[Property]
	public bool EnableCollectableSpawning {  get; set; } = true;

	protected override void DrawGizmos()
	{
		// 为四个Marker画出一个完整的矩形
		Gizmo.Draw.Line( CollectableSpawnMarker1, CollectableSpawnMarker2 );
		Gizmo.Draw.Line( CollectableSpawnMarker2, CollectableSpawnMarker3 );
		Gizmo.Draw.Line( CollectableSpawnMarker3, CollectableSpawnMarker4 );
		Gizmo.Draw.Line( CollectableSpawnMarker4, CollectableSpawnMarker1 );
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
		// 计时器，每隔几秒就在平面上随机生成一次收集物
		if ( !EnableCollectableSpawning ) return;
		if ( Timer >= CollectableSpawnTimeEach )
		{
			SpawnCollectable();
			Timer = 0;
		}
	}
	private void SpawnCollectable()
	{	
		Vector3 minBound = new Vector3(MathF.Min(CollectableSpawnMarker1.x, MathF.Min(CollectableSpawnMarker2.x, MathF.Min(CollectableSpawnMarker3.x, CollectableSpawnMarker4.x))),
                                       MathF.Min(CollectableSpawnMarker1.y, MathF.Min(CollectableSpawnMarker2.y, MathF.Min(CollectableSpawnMarker3.y, CollectableSpawnMarker4.y))),
                                       MathF.Min(CollectableSpawnMarker1.z, MathF.Min(CollectableSpawnMarker2.z, MathF.Min(CollectableSpawnMarker3.z, CollectableSpawnMarker4.z))));

        Vector3 maxBound = new Vector3(MathF.Max(CollectableSpawnMarker1.x, MathF.Max(CollectableSpawnMarker2.x, MathF.Max(CollectableSpawnMarker3.x, CollectableSpawnMarker4.x))),
                                       MathF.Max(CollectableSpawnMarker1.y, MathF.Max(CollectableSpawnMarker2.y, MathF.Max(CollectableSpawnMarker3.y, CollectableSpawnMarker4.y))),
                                       MathF.Max(CollectableSpawnMarker1.z, MathF.Max(CollectableSpawnMarker2.z, MathF.Max(CollectableSpawnMarker3.z, CollectableSpawnMarker4.z))));

		Vector3 randomPosition = new Vector3(
            Random.Shared.Float(minBound.x, maxBound.x),
            Random.Shared.Float(minBound.y, maxBound.y),
            Random.Shared.Float(minBound.z, maxBound.z)
        );
		// Log.Info($"Collectable spawn at randomPosition: {randomPosition}");
		Collectable.Clone( randomPosition );
	}
	

}
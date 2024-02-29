using System;
using Sandbox;

public sealed class SkibidiHeadTrigger : Component, Component.ITriggerListener
{	
	[Property]
	public SoundEvent DeathSound { get; set; }

	[Property]
	[Range(0.1f, 3f, 0.1f)]
	public float DeathTime { get; set; } = 0.5f;
	private bool isDead = false;
	public TimeSince Timer;
	protected override void OnUpdate()
	{
		if ( !isDead ) return;
		var targetScale = new Vector3( GameObject.Parent.Transform.Scale.x, GameObject.Parent.Transform.Scale.y, 0.1f);
		GameObject.Parent.Transform.Scale = Vector3.Lerp( GameObject.Parent.Transform.Scale, targetScale, 0.1f);
		if ( Timer >= DeathTime )
		{	
			GameObject.Parent.Parent.Destroy();
		}

	}

	void ITriggerListener.OnTriggerEnter( Collider other ) 
	{
		if ( other.Tags.Has("player") )
		{	
			if ( DeathSound != null ) Sound.Play(DeathSound, Transform.Position);
			isDead = true;
			Timer = 0f;
			// Log.Info($"Collider disable");
			GameObject.Components.Get<Collider>().Enabled = false;
			GameObject.Parent.Components.Get<Collider>().Enabled = false;
		}

	}

	void ITriggerListener.OnTriggerExit( Collider other ) 
	{	
		
	}
}

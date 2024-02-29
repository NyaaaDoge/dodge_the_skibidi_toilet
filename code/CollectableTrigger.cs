using System.Threading;
using Sandbox;

public sealed class CollectableTrigger : Component, Component.ITriggerListener
{
	[Property]
	public SoundEvent CollectSound { get; set; }
	[Property]
	public float CollectableExistTime { get; set; } = 7f;
	private TimeSince Timer;

	protected override void OnStart()
	{
		Timer = 0f;
	}

	protected override void OnFixedUpdate()
	{	
		if ( Timer >= CollectableExistTime )
		{	
			GameObject.Parent.Parent.Destroy();
			Timer = 0f;
		}
	}

	void ITriggerListener.OnTriggerEnter( Collider other ) 
	{	
		if ( other.Tags.Has("player") )
		{
			if ( CollectSound != null ) Sound.Play(CollectSound, Transform.Position);
			GameObject.Parent.Parent.Destroy();
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other ) 
	{

	}
}

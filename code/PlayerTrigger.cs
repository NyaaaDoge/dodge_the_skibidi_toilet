using System.Reflection;
using Sandbox;

public sealed class PlayerTrigger : Component, Component.ITriggerListener
{
	MyGameManager manager => Scene.GetAllComponents<MyGameManager>().FirstOrDefault();
	DodgePlayer player => Scene.GetAllComponents<DodgePlayer>().FirstOrDefault();

	void ITriggerListener.OnTriggerEnter( Collider other ) 
	{	
		if ( other.Tags.Has("skibidi_head") )
		{
			if ( player == null ) return;
			// TODO 如果可以，直接计算碰撞体触碰点的法线
			// if (Vector3.Up.Dot(other.collision.GetNormal()) > 0.1f)
			// {
			// 重置玩家速度
			player.Controller.Velocity *= 0.3f;
			// player.Controller.Velocity = new Vector3(currentVelocity.x, currentVelocity.y, 0f);
			// other.Rigidbody.ApplyForce(Vector3.Up * player.BounceStrength);
			// 重新施加跳跃力
			player.Controller.Punch(Vector3.Up * player.BounceStrength);
			player.Animator.TriggerJump();
			// }
			// 为玩家加分
			manager.AddKillCount( 1 );
		}

		if ( other.Tags.Has("collectable") )
		{
			// 鼓励玩家进行移动吃球获取奖励
			// Log.Info("Player collect a collectable");
			if ( player == null ) return;
			manager.AddCollectCount( 1 );
		}

		if ( other.GameObject.Name == "skibidi body" )
		{	
			if ( player == null ) return;
			manager.EndGame();
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other ) 
	{

	}

}

using System;
using Sandbox;
using Sandbox.Citizen;

public sealed class DodgePlayer : Component
{
	/// <summary>
	/// Player Controller
	/// </summary>
	[Property]
	[Category("Components")]
	public CharacterController Controller { get; set; }

	/// <summary>
	/// Citizen Animation Helper
	/// </summary>
	[Property]
	[Category("Components")]
	public CitizenAnimationHelper Animator { get; set; }

	[Property]
	[Category("Components")]
	public SoundEvent DeathSound { get; set; }

	/// <summary>
	/// How fast you can walk (units per second)
	/// </summary>
	[Property]
	[Category("Stats")]
	public float Speed { get; set; } = 200f;

	/// <summary>
	/// How strong you can jump (units per second)
	/// </summary>
	[Property]
	[Category("Stats")]
	[Range(0f, 400f, 1f)]
	public float JumpStrength { get; set; } = 220f;

	/// <summary>
	/// How strong you can bounce after squash the enemy (units per second)
	/// </summary>
	[Property]
	[Category( "Stats" )]
	[Range( 0f, 400f, 1f )]
	public float BounceStrength { get; set; } = 200f;

	protected override void OnUpdate()
	{	
		Vector3 direction = Input.AnalogMove;
		if ( direction != Vector3.Zero )
		{
			// 计算方向向量的yaw角度
			float yaw = MathX.RadianToDegree( MathF.Atan2( direction.y, direction.x ) );
			// 创建一个新的 Angles 对象，设置 yaw 分量
			var targetAngle = new Angles( 0, yaw, 0 );
			// 使用 ToRotation 方法将 Angles 对象转换为 Rotation 对象
			var rotation = targetAngle.ToRotation();
			Transform.Rotation = Rotation.Lerp( Transform.Rotation, rotation, 0.3f );
		}
	}
	protected override void OnFixedUpdate()
	{
		if (Controller == null) return;

		var targetVelocity = Input.AnalogMove.Normal * Speed;
		// Log.Info($"targetVelocity: {targetVelocity}");
		Controller.Accelerate(targetVelocity);
		Controller.Velocity = Controller.Velocity.Clamp(-225f, 225f);
		// Log.Info($"targetVelocity clamped: {Controller.Velocity}");
		

		if (Controller.IsOnGround)
		{
			// 施加摩擦力
			Controller.ApplyFriction(5f);

			// 跳跃操作
			if (Input.Pressed("Jump"))
			{
				Controller.Punch(Vector3.Up * JumpStrength);
				if (Animator != null)
					Animator.TriggerJump();
			}

		}
		else
		{
			Controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
		}

		Controller.Move();

		if (Animator != null)
		{
			Animator.IsGrounded = Controller.IsOnGround;
			Animator.WithVelocity(Controller.Velocity);
		}

		// Log.Info($"Controller.Velocity: {Controller.Velocity}");

	}

	protected override void OnStart()
	{
		if (Components.TryGet<SkinnedModelRenderer>(out var model))
		{
			var clothing = ClothingContainer.CreateFromLocalUser();
			clothing.Apply(model);
		}
	}

	protected override void OnDestroy()
	{

	}

	public void Dead()
	{
		// 处理玩家死亡
		Sound.Play(DeathSound, Transform.Position);
		//Log.Info("Player dead");
		GameObject.Parent.Enabled = false;
	}
	public void Respawn()
	{
		// 玩家重生
		//Log.Info( "Player respawn" );
		GameObject.Parent.Enabled = true;
		Controller.Velocity = Vector3.Zero;
		Transform.Rotation = new Rotation();
		Transform.Position = new Vector3( 0, 0, 3 );
		Transform.Rotation = Rotation.FromYaw(180);
	}
}

using System;
using System.Threading.Tasks;
using Sandbox;

public sealed class MyGameManager : Component
{
	[Property]
	public GameObject GameUI { get; set; }
	
	[Property]
	public GameObject DeathMenu { get; set; }

	[Property]
	public int TimeToNextDifficultyIncrease = 15;

	[Property]
	public int Difficulty = 0;
	public bool Playing { get; private set; } = false;
	public long KillCount { get; private set; } = 0;
	public long CollectCount { get; private set; } = 0;
	public TimeSince ManagerTimer { get; private set; } = 0f;
	public float PlayerTimeSurvived { get; private set;} = 0f;
	public long Score { get; private set; } = 0;
	public long HighScore { get; private set; } = 0;
	public long MostKillCount { get; private set; } = 0;
	public long MostCollectCount { get; private set; } = 0;
	public float MostTimeSurvived { get; private set; } = 0f;
	public Sandbox.Services.Leaderboards.Board Leaderboard;
	GameObject PlayerObject => GameManager.ActiveScene.Directory.FindByName( "player" ).FirstOrDefault();
	EnemySpawner EnemySpawner => Scene.GetAllComponents<EnemySpawner>().FirstOrDefault();
	CollectableSpawner CollectableSpawner => Scene.GetAllComponents<CollectableSpawner>().FirstOrDefault();
	DodgePlayer Player => Scene.GetAllComponents<DodgePlayer>().FirstOrDefault();


	protected override void OnStart()
	{
		RestartGame();
	}
	protected override void OnUpdate()
	{
		if ( !Playing ) return;
		// 设置难度随时间增长
		if (Difficulty >= 7) return;
		var nextDifficulty = (int)MathF.Floor(ManagerTimer / TimeToNextDifficultyIncrease);
		if ( nextDifficulty > Difficulty )
		{
			Difficulty = nextDifficulty;
			SetDifficulty(Difficulty);
		}

	}
	

	public void RestartGame()
	{
		// 游戏开始
		if ( Playing ) return;
		Playing = true;
		ResetAll();
		FetchLeaderboardInfo();
	}

	public void EndGame()
	{
		if ( !Playing ) return;
		Player.Dead();
		SetPlayerTimeAlive( ManagerTimer );
		EnemySpawner.EnableEnemySpawning = false;
		CollectableSpawner.EnableCollectableSpawning = false;
		Playing = false;
		DeathMenu.Enabled = true;
		GameUI.Enabled = false;

		Score = KillCount * 2 + CollectCount * 2 + (long)PlayerTimeSurvived;
		SetLeaderBoard();
	}
	public void ResetAll()
	{
		ResetScore();
		ResetTimer();
		Difficulty = 0;
		SetDifficulty(Difficulty);
		EnemySpawner.EnableEnemySpawning = true;
		EnemySpawner.MaxRotateOffset = 45;
		EnemySpawner.MinRotateOffset = -45;
		EnemySpawner.MaxSpeed = 250;
		EnemySpawner.MinSpeed = 70;
		CollectableSpawner.EnableCollectableSpawning = true;
		PlayerObject.Enabled = true;
		GameUI.Enabled = true;
		Player.Respawn();
		// 销毁所有敌人和收集物
		var enemyList = Scene.GetAllComponents<Enemy1>();
		foreach ( var enemy in enemyList )
		{
			enemy.GameObject.Destroy();
		}
		var collectableList = Scene.GetAllComponents<CollectableTrigger>();
		
		foreach ( var collectable in collectableList )
		{
			collectable.GameObject.Parent.Parent.Destroy();
		}
	}
	public void ResetTimer()
	{
		ManagerTimer = 0f;
		EnemySpawner.Timer = 0f;
		CollectableSpawner.Timer = 0f;
	}
	public void ResetScore()
	{	
		KillCount = 0;
		CollectCount = 0;
		Score = 0;
		PlayerTimeSurvived = 0f;
	}
	public void AddScore( long score )
	{
		Score += score;
	}
	public void AddKillCount( long kills )
	{
		KillCount += kills;
	}
	public void AddCollectCount( long collectCount )
	{
		CollectCount += collectCount;
	}
	public void SetPlayerTimeAlive( float time )
	{
		PlayerTimeSurvived = time;
	}
	// 设置难度
	public void SetDifficulty( int difficulty )
	{
		switch (difficulty)
		{
			case 1:
				EnemySpawner.EnemySpawnTimeEach = 0.65f;
				EnemySpawner.MaxSpeed = 300;
				EnemySpawner.MinSpeed = 80;
				break;
			case 2:
				CollectableSpawner.CollectableSpawnTimeEach = 5.5f;
				EnemySpawner.EnemySpawnTimeEach = 0.6f;
				EnemySpawner.MaxRotateOffset = 42;
				EnemySpawner.MinRotateOffset = -42;
				EnemySpawner.MaxSpeed = 310;
				EnemySpawner.MinSpeed = 80;
				break;
			case 3:
				EnemySpawner.EnemySpawnTimeEach = 0.5f;
				EnemySpawner.MaxRotateOffset = 40;
				EnemySpawner.MinRotateOffset = -40;
				EnemySpawner.MaxSpeed = 320;
				break;
			case 4:
				CollectableSpawner.CollectableSpawnTimeEach = 5f;
				EnemySpawner.EnemySpawnTimeEach = 0.45f;
				EnemySpawner.MinSpeed = 90;
				EnemySpawner.MaxSpeed = 330;
				break;
			case 5:
				EnemySpawner.EnemySpawnTimeEach = 0.4f;
				EnemySpawner.MaxSpeed = 350;
				break;
			case 6:
				EnemySpawner.EnemySpawnTimeEach = 0.35f;
				EnemySpawner.MaxRotateOffset = 35;
				EnemySpawner.MinRotateOffset = -35;
				EnemySpawner.MinSpeed = 100;
				EnemySpawner.MaxSpeed = 360;
				break;
			case > 6:
				CollectableSpawner.CollectableSpawnTimeEach = 4.5f;
				EnemySpawner.EnemySpawnTimeEach = 0.3f;
				EnemySpawner.MaxRotateOffset = 30;
				EnemySpawner.MinRotateOffset = -30;
				EnemySpawner.MinSpeed = 115;
				EnemySpawner.MaxSpeed = 370;
				break;
			default:
				EnemySpawner.EnemySpawnTimeEach = 0.7f;
				CollectableSpawner.CollectableSpawnTimeEach = 5f;
				EnemySpawner.MaxSpeed = 70;
				EnemySpawner.MinSpeed = 270;
				break;
		}
	}


	// 实现分数上传Leaderboard
	public void SetLeaderBoard()
	{
		if ( Score > HighScore ) HighScore = Score;
		if ( KillCount > MostKillCount ) MostKillCount = KillCount;
		if ( PlayerTimeSurvived > MostTimeSurvived ) MostTimeSurvived = PlayerTimeSurvived;
		if ( CollectCount > MostCollectCount ) MostCollectCount = CollectCount;
		// Log.Info($"Set leaderboard Score: {Score}, HighScore: {HighScore}");
		// Log.Info($"Set leaderboard KillCount: {KillCount}, MostKillCount: {MostKillCount}");
		// Log.Info($"Set leaderboard PlayerTimeSurvived: {PlayerTimeSurvived}, MostTimeSurvived: {MostTimeSurvived}");
		Sandbox.Services.Stats.SetValue( "highscores", Score );
		Sandbox.Services.Stats.SetValue( "mostkills", KillCount );
		Sandbox.Services.Stats.SetValue( "mostcoinsgot", CollectCount );
		Sandbox.Services.Stats.SetValue( "mosttimesurvived", PlayerTimeSurvived );
	}
	
	async void FetchLeaderboardInfo()
	{	
		Leaderboard = Sandbox.Services.Leaderboards.Get( "highscores" );
		Leaderboard.MaxEntries = 10;
		await Leaderboard.Refresh();
		foreach ( var entry in Leaderboard.Entries )
		{
			if ( entry.SteamId == Game.SteamId )
			{
				HighScore = (long)entry.Value;
			}
		}
	}
}

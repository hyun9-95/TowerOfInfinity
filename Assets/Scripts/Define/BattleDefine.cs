public class BattleDefine
{
    // 웨이브 관련 상수
    public const float DEFAULT_WAVE_DURATION_SECONDS = 60f;
    public const float CHEAT_WAVE_DURATION_MULTIPLIER = 0.5f;
    
    // 경험치 관련 상수
    public const float CHEAT_EXP_MULTIPLIER = 2f;
    
    // 각도 관련 상수
    public const float FULL_CIRCLE_DEGREES = 360f;
    public const float QUARTER_CIRCLE_DEGREES = 90f;
    
    // 궤도 관련 상수
    public const float ORBIT_SPEED_DEGREES_PER_SECOND = 360f; // 초당 1바퀴
    
    // 거리 관련 상수 (배수)
    public const float CAMERA_DISTANCE_FAR_MULTIPLIER = 2f;
    public const float CAMERA_DISTANCE_VERY_FAR_MULTIPLIER = 2.5f;
    
    // 업데이트 간격 상수
    public const float UPDATE_INTERVAL_VERY_FAR = 1.5f;
    public const float UPDATE_INTERVAL_FAR = 1f;
    public const float UPDATE_INTERVAL_NEARBY = 0.5f;
    
    // Character Repath Cooldowns
    public const float REPATH_COOLTIME_CLOSE = 0.5f;
    public const float REPATH_COOLTIME_NEARBY = 1f;
    public const float REPATH_COOLTIME_DEFAULT = 2f;
    
    // Enemy Spawner Constants
    public const int BURST_SPAWN_COUNT_PER_INTERVAL = 5;
    public const float SPAWN_POSITION_OFFSET = 0.5f;
    public const int ENEMY_SPAWN_SAFE_COUNT = 3;
    
    // Camera Constants
    public const float CAMERA_DIAGONAL_OFFSET = 0.5f;
    public const float CAMERA_ORTHOGRAPHIC_HEIGHT_MULTIPLIER = 2f;
    
    // 안전 카운터
    public const int BATTLE_CARD_DRAW_SAFE_COUNT = 100;
    
    // 확률 계산 상수
    public const float TIER_RATE_DIVISOR = 1000f;
    
    // Bouncing Projectile Constants
    public const float BOUNCE_COOLDOWN_SECONDS = 0.1f;
    public const float BOUNCE_VECTOR_MULTIPLIER = 2f;
    
    // Formula Constants
    public const float MIN_DAMAGE_VALUE = 1f;
    public const float EXP_RANGE_BASE_RADIUS = 1f;
    public const float EXP_RANGE_LEVEL_MULTIPLIER = 0.1f;
    
    // Movement and Animation Constants
    public const float EXP_GEM_DISTANCE_THRESHOLD = 0.8f;
}
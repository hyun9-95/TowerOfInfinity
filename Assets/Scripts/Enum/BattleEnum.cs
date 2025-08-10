public enum AbilitySlotType
{
    Weapon,     // 주무기 슬롯
    Active,     // 액티브 슬롯
    Passive,    // 패시브 슬롯
}

public enum BattleEventTriggerType
{
    None,
    Projectile,             // 투사체로 검출
    FollowCollider,         // 타겟을 따라다니는 콜라이더로 검출
    InRange,                // 범위 안의 타겟 위치에 콜라이더 생성 및 검출 
    InRangeFollow,          // 범위 안의 타겟 위치에 콜라이더 생성 후 Follow
    FollowProjectile,       // 타겟을 추적하는 투사체 생성
    RandomProjectile,       // 랜덤 방향 투사체 생성
    Self,                   // 자기 자신 대상 이벤트 Send
}

public enum BattleEventType
{
    Damage,
    Poison,
}

public enum BattleEventGroup
{
    None,
    Buff,
    Debuff,
}

public enum BattleEventCategory
{
    None,
    Passive,
    Active,
}

public enum BattleEventTargetType
{
    None,
    Ally,
    Enemy,
}

public enum CastingType
{
    Instant,      //즉시 발동
    Auto,         //쿨타임마다 발동
    OnAttack,     //캐릭터가 공격할 때 발동
    OnRoll,       //캐릭터가 구를 때 발동
    OnGuard,      //캐릭터가 가드할 때 발동
}

public enum BattleCardType
{
    None,
    GetAbility,            // 능력 추가 or 기존 능력 레벨업
    ExpGainRangeUp,         // 경험치 획득 범위 증가
}

public enum BattleCardTier
{
    Common,
    Uncommon,
    Rare,
    Epic,
}

public enum BattleState
{
    None,
    Playing,
    Paused,
    Victory,
    Defeat,
}
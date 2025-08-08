public enum CharacterAnimState
{
    Idle = 0,
    Ready = 1,
    Move = 2,
    Slash = 3,
    Jab = 4,
    Push = 5,
    BowShot = 6,
    Roll = 7,
    Crouch = 8,
    Attack = 10,

    Die = 99,
    None = 100,
}

public enum BattleEventTriggerType
{
    None,
    Projectile,             // 투사체로 검출
    FollowCollider,         // 타겟을 따라다니는 콜라이더로 검출
    InRange,                // 범위 안의 타겟 위치에 콜라이더 생성 및 검출 
    InRangeFollow,          // 범위 안의 타겟 위치에 콜라이더 생성 후 Follow
}

public enum BattleEventTargetType
{
    None,
    Ally,
    Enemy,
}

public enum BattleEventType
{
    Damage,
    Poison,
}

public enum PlayerInput
{
    None,
    Attack,
    Roll,
    SkillSlot_1,
    SkillSlot_2,
    SkillSlot_3,
}

public enum UICanvasType
{
    View,
    Popup,
    Front,
    UIWorld,
}

public enum SoundType
{
    Bgm,
    Ambience,
}

public enum FlowType
{
    IntroFlow,
    TownFlow,
    BattleFlow,
    CustomizationFlow,
}

public enum LoadDataType
{
    Local,
    Remote,
}

public enum StatType
{
    MaxHp,
    Attack,
    Defense,
    MoveSpeed,
    None = 99,
}

public enum PortalType
{
    WarpToBattle,
}

[System.Flags]
public enum LayerFlag
{
    None = 0,           
    Default = 1 << 0,     
    TransparentFX = 1 << 1,      
    IgnoreRaycast = 1 << 2,     
    Character = 1 << 3,  
    Water = 1 << 4,    
    UI = 1 << 5,     
    Background = 1 << 6,    
    Obstacle = 1 << 7,      
    Effect = 1 << 8,
    Camera = 1 << 9,
    Object = 1 << 10,
}

public enum LayerInt
{
    Default,
    TransparentFX,
    IgnoreRaycast,
    Character,
    Water,
    UI,
    Background,
    Obstacle,
    Effect,
    Camera,
    Object,
}

public enum TransitionType
{
    //Diagnoal Rectangle
    Default,
}

public enum PathFindType
{
    Navmesh,
    AStar,
}

public enum CastingType
{
    Instant,      //즉시 발동
    Auto,         //쿨타임마다 발동
    OnAttack,     //캐릭터가 공격할 때 발동
    OnRoll,       //캐릭터가 구를 때 발동
    OnGuard,      //캐릭터가 가드할 때 발동
}

public enum TeamTag
{
    Ally,
    Enemy,
}

public enum ModuleType
{
    CollisionDamage,
    CharacterUI,
    SpawnExpGem,
}

public enum CharacterType
{
    Main,
    Sub,
    NPC,
    Enemy,
    EnemyBoss,
}

public enum CharacterSetUpType
{
    Town,
    Battle,
}

public enum DirectionType
{
    None,
    Up,
    Down,
    Left,
    Right,
    Owner,
}

public enum DamageType
{
    Normal,
    Critical,
    Heal,
    Poison,
    Max,
}

public enum BattleEventCategory
{
    None,
    Passive,
    Active,
}

public enum BattleEventGroup
{
    None,
    Buff,
    Debuff,
}

public enum StatusDirection
{
    None,
    Increase,
    Decrease,
}

public enum StatReference
{
    None,
    Sender,
    Receiver,
}

public enum StatReferenceCondition
{
    None,
    BaseStat,
    CurrentStat,
}

public enum CharacterPartsCategory
{
    Race,
    Hair,
    Equipment,
}

public enum CharacterPartsType
{
    Head,
    Ears,
    Eyes,
    Body,
    Arms,
    Hair,
    Armor,
    Helmet,
    Weapon,
    Firearm,
    Shield,
    Cape,
    Back,
    Mask,
    Horns,
    Bracers,
}

public enum CharacterRace
{
    Human,
    Elf,
    DarkElf,
    Demon,
    LizardMan,
    Merman,
    Furry,
    Orc,
    Goblin,
    Vampire,
    Undead,
    Monster,
}

public enum EquipmentType
{
    Armor,
    Helmet,
    Bracers,
    Shield,
    Weapon,
    Mask,
}

public enum AbilitySlotType
{
    Weapon,     // 주무기 슬롯
    Active,     // 액티브 슬롯
    Passive,    // 패시브 슬롯
}

public enum BattleCardType
{
    None,
    GetAbility,            // 능력 추가 or 기존 능력 레벨업
    LevelUpExpGainer,      // 경험치 획득 범위 증가
}

public enum BattleCardTier
{
    Common,
    Uncommon,
    Rare,
    Epic,
}
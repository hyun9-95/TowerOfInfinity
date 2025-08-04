using System;

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
}

public enum BattleEventTriggerType
{
    None,
    Projectile,     // 투사체로 검출
    Range,          // 범위로 검출
    Collider,       // 콜라이더로 검출
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
    KnockBack,
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

public enum AbilityType
{
    Weapon,
    ActiveSkill,
    PassiveSkill,
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
}

public enum CharacterType
{
    Main,
    Sub,
    NPC,
    Enemy,
    EnemyBoss,
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
    BeastMan,
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
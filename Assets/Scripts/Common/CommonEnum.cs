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
    Projectile,
    Range,
    Movement,
}

public enum BattleEventTargetType
{
    None,
    Self,
    Single,
    Multiple,
    AllAllies,
    AllEnemies,
}

public enum BattleEventType
{
    Damage,
    Buff,
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
}

public enum SoundType
{
    Bgm,
    Ambience,
}

public enum FlowType
{
    IntroFlow,
    LobbyFlow,
    BattleFlow,
}

public enum LoadDataType
{
    LocalPath,
    FireBase,
}

public enum StatType
{
    MaxHp,
    Attack,
    Defense,
    MoveSpeed,
    Max = 99,
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
    Leader,
    Servent,
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

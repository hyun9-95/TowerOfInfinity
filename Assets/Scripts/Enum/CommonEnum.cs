public enum  PlatformType
{
    None = 0,
    StandAlone,
    Android,
    IOS,
}

public enum DistanceToTarget
{
    None,
    Close,
    Nearby,
    Far,
    VeryFar,
    OutOfRange,     // 너무 멀어서 비활성화되는 거리
}

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
    Fire = 11,
    Burn = 12,

    Die = 99,
    None = 100,
}

public enum ActionInput
{
    Attack,
    Roll,
    None,
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
    Amb,
    Sfx,
    None = 99,
}

public enum ButtonSoundType
{
    Click,
    Confirm,
    Cancel,
    None = 99,
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
    Editor,
    Addressable,
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
    None,
    Navmesh,
    AStar,
}

public enum TeamTag
{
    Ally,
    Enemy,
}

public enum ModuleType
{
    CollisionDamage,            // 충돌 대미지
    CharacterUI,                // 특정 UI 
    SpawnExpGem,                // 죽을 때 경험치 젠
    DynamicUpdateInterval,      // 타겟과의 거리 기반 및 업데이트 간격 동적 조절
}

public enum CharacterType
{
    Main,
    Sub,
    NPC,
    Enemy,
    Boss,
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
using UnityEngine;

public static class CommonUtils
{
    #region RACE PARTS
    public static int[] GetRacePartsIds(CharacterRace race)
    {
        switch (race)
        {
            case CharacterRace.Human:
                return new int[]
                {
                    (int)CharacterPartsDefine.PARTS_RACE_HEAD_HUMAN,
                    (int)CharacterPartsDefine.PARTS_RACE_EARS_HUMAN,
                    (int)CharacterPartsDefine.PARTS_RACE_EYES_HUMAN,
                    (int)CharacterPartsDefine.PARTS_RACE_ARMS_HUMAN,
                    (int)CharacterPartsDefine.PARTS_RACE_BODY_HUMAN,
                };
            case CharacterRace.Elf:
                return new int[]
                {
                    (int)CharacterPartsDefine.PARTS_RACE_HEAD_ELF,
                    (int)CharacterPartsDefine.PARTS_RACE_EARS_ELF,
                    (int)CharacterPartsDefine.PARTS_RACE_EYES_ELF,
                    (int)CharacterPartsDefine.PARTS_RACE_ARMS_ELF,
                    (int)CharacterPartsDefine.PARTS_RACE_BODY_ELF,
                };
            case CharacterRace.DarkElf:
                return new int[]
                {
                    (int)CharacterPartsDefine.PARTS_RACE_HEAD_DARKELF,
                    (int)CharacterPartsDefine.PARTS_RACE_EARS_DARKELF,
                    (int)CharacterPartsDefine.PARTS_RACE_EYES_DARKELF,
                    (int)CharacterPartsDefine.PARTS_RACE_ARMS_DARKELF,
                    (int)CharacterPartsDefine.PARTS_RACE_BODY_DARKELF,
                };
            case CharacterRace.Demon:
                return new int[]
                {
                    (int)CharacterPartsDefine.PARTS_RACE_HEAD_DEMON,
                    (int)CharacterPartsDefine.PARTS_RACE_EARS_DEMON,
                    (int)CharacterPartsDefine.PARTS_RACE_EYES_DEMON,
                    (int)CharacterPartsDefine.PARTS_RACE_ARMS_DEMON,
                    (int)CharacterPartsDefine.PARTS_RACE_BODY_DEMON,
                    (int)CharacterPartsDefine.PARTS_RACE_HORNS_DEMON,
                };
            case CharacterRace.LizardMan:
                return new int[]
                {
                    (int)CharacterPartsDefine.PARTS_RACE_HEAD_LIZARD,
                    (int)CharacterPartsDefine.PARTS_RACE_EARS_LIZARD,
                    (int)CharacterPartsDefine.PARTS_RACE_EYES_LIZARD,
                    (int)CharacterPartsDefine.PARTS_RACE_ARMS_LIZARD,
                    (int)CharacterPartsDefine.PARTS_RACE_BODY_LIZARD,
                };
            case CharacterRace.Merman:
                return new int[]
                {
                    (int)CharacterPartsDefine.PARTS_RACE_HEAD_MERMAN,
                    (int)CharacterPartsDefine.PARTS_RACE_EARS_MERMAN,
                    (int)CharacterPartsDefine.PARTS_RACE_EYES_MERMAN,
                    (int)CharacterPartsDefine.PARTS_RACE_ARMS_MERMAN,
                    (int)CharacterPartsDefine.PARTS_RACE_BODY_MERMAN,
                };
            case CharacterRace.Furry:
                return new int[]
                {
                    (int)CharacterPartsDefine.PARTS_RACE_HEAD_FURRY,
                    (int)CharacterPartsDefine.PARTS_RACE_EARS_FURRY,
                    (int)CharacterPartsDefine.PARTS_RACE_EYES_FURRY,
                    (int)CharacterPartsDefine.PARTS_RACE_ARMS_FURRY,
                    (int)CharacterPartsDefine.PARTS_RACE_BODY_FURRY,
                };
            case CharacterRace.Orc:
                return new int[]
                {
                    (int)CharacterPartsDefine.PARTS_RACE_HEAD_ORC,
                    (int)CharacterPartsDefine.PARTS_RACE_EARS_ORC,
                    (int)CharacterPartsDefine.PARTS_RACE_EYES_ORC,
                    (int)CharacterPartsDefine.PARTS_RACE_ARMS_ORC,
                    (int)CharacterPartsDefine.PARTS_RACE_BODY_ORC,
                };
            case CharacterRace.Goblin:
                return new int[]
                {
                    (int)CharacterPartsDefine.PARTS_RACE_HEAD_GOBLIN,
                    (int)CharacterPartsDefine.PARTS_RACE_EARS_GOBLIN,
                    (int)CharacterPartsDefine.PARTS_RACE_EYES_GOBLIN,
                    (int)CharacterPartsDefine.PARTS_RACE_ARMS_GOBLIN,
                    (int)CharacterPartsDefine.PARTS_RACE_BODY_GOBLIN,
                };
            case CharacterRace.Vampire:
                return new int[]
                {
                    (int)CharacterPartsDefine.PARTS_RACE_HEAD_VAMPIRE,
                    (int)CharacterPartsDefine.PARTS_RACE_EARS_VAMPIRE,
                    (int)CharacterPartsDefine.PARTS_RACE_EYES_VAMPIRE,
                    (int)CharacterPartsDefine.PARTS_RACE_ARMS_VAMPIRE,
                    (int)CharacterPartsDefine.PARTS_RACE_BODY_VAMPIRE,
                };
            case CharacterRace.Undead:
                return new int[]
                {
                    (int)CharacterPartsDefine.PARTS_RACE_HEAD_SKELETON,
                    (int)CharacterPartsDefine.PARTS_RACE_EARS_SKELETON,
                    (int)CharacterPartsDefine.PARTS_RACE_EYES_SKELETON,
                    (int)CharacterPartsDefine.PARTS_RACE_ARMS_SKELETON,
                    (int)CharacterPartsDefine.PARTS_RACE_BODY_SKELETON,
                };
            case CharacterRace.Monster:
                return new int[]
                {
                    (int)CharacterPartsDefine.PARTS_RACE_HEAD_ZOMBIEA,
                    (int)CharacterPartsDefine.PARTS_RACE_EARS_ZOMBIEA,
                    (int)CharacterPartsDefine.PARTS_RACE_EYES_ZOMBIEA,
                    (int)CharacterPartsDefine.PARTS_RACE_ARMS_ZOMBIEA,
                    (int)CharacterPartsDefine.PARTS_RACE_BODY_ZOMBIEA,
                };
            default:
                return new int[0];
        }
    }
    #endregion
}

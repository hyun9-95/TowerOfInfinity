using UnityEngine;

public static class EnumLocalizations
{
    public static string GetLocalization(this CharacterRace race)
    {
        LocalizationDefine define = LocalizationDefine.None;

        switch (race)
        {
            case CharacterRace.Human:
                define = LocalizationDefine.LOCAL_ENUM_RACE_HUMAN;
                break;
            case CharacterRace.Elf:
                define = LocalizationDefine.LOCAL_ENUM_RACE_ELF;
                break;
            case CharacterRace.DarkElf:
                define = LocalizationDefine.LOCAL_ENUM_RACE_DARK_ELF;
                break;
            case CharacterRace.Demon:
                define = LocalizationDefine.LOCAL_ENUM_RACE_DEMON;
                break;
            case CharacterRace.LizardMan:
                define = LocalizationDefine.LOCAL_ENUM_RACE_LIZARDMAN;
                break;
            case CharacterRace.Merman:
                define = LocalizationDefine.LOCAL_ENUM_RACE_MERMAN;
                break;
            case CharacterRace.Furry:
                define = LocalizationDefine.LOCAL_ENUM_RACE_FURRY;
                break;
            case CharacterRace.Orc:
                define = LocalizationDefine.LOCAL_ENUM_RACE_ORC;
                break;
            case CharacterRace.Goblin:
                define = LocalizationDefine.LOCAL_ENUM_RACE_GOBLIN;
                break;
            case CharacterRace.Vampire:
                define = LocalizationDefine.LOCAL_ENUM_RACE_VAMPIRE;
                break;
            case CharacterRace.Undead:
                define = LocalizationDefine.LOCAL_ENUM_RACE_UNDEAD;
                break;
            case CharacterRace.Monster:
                define = LocalizationDefine.LOCAL_ENUM_RACE_MONSTER;
                break;
        }

        return LocalizationManager.GetLocalization(define);
    }
}

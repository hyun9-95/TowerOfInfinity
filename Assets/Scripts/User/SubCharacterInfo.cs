public class SubCharacterInfo : CharacterAbilityInfo
{
    #region Property
    public int CharacterDataId { get; private set; }
    public int SlotIndex { get; private set; }
    #endregion

    #region Value
    #endregion

    #region Function
    public void SetCharacterDataId(int dataCharacterId)
    {
        CharacterDataId = dataCharacterId;
    }

    public void SetSlotIndex(int slotIndex)
    {
        SlotIndex = slotIndex;
    }
    #endregion
}

public static class FlowFactory
{
    public static BaseFlow Create(FlowType type)
    {
        switch (type)
        {
            case FlowType.IntroFlow:
                return new IntroFlow();

            case FlowType.LobbyFlow:
                return new LobbyFlow();

            case FlowType.BattleFlow:
                return new BattleFlow();
        }

        return null;
    }
}

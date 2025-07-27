public static class FlowFactory
{
    public static BaseFlow Create(FlowType type)
    {
        switch (type)
        {
            case FlowType.IntroFlow:
                return new IntroFlow();

            case FlowType.TownFlow:
                return new TownFlow();

            case FlowType.BattleFlow:
                return new BattleFlow();
        }

        return null;
    }
}

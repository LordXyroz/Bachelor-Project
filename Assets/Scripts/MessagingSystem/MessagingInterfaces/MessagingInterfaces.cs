namespace MessagingInterfaces
{
    public interface IAddDefense
    {
        void AddDefense(DefenseMessage message);
    }

    public interface IAnalyze
    {
        void OnAnalyze(AnalyzeMessage message);
    }

    public interface IAnalyzeResponse
    {
        void OnAnalyzeResponse(AnalyzeResponeMessage message);
    }

    public interface IAttackResponse
    {
        void AttackResponse(SuccessMessage message);
    }

    public interface IDefenseResponse
    {
        void DefenseResponse(SuccessMessage message);
    }

    public interface IDiscover
    {
        void OnDiscover(DiscoverMessage message);
    }

    public interface IDiscoverResponse
    {
        void OnDiscoverResponse(DiscoverResponseMessage message);
    }

    public interface IError
    {
        void OnError(LoggingMessage message);
    }

    public interface ILogging
    {
        void OnLog(LoggingMessage message);
    }

    public interface IProbe
    {
        void OnProbe(Message message);
    }

    public interface IProbeResponse
    {
        void OnProbeResponse(ProbeResponseMessage message);
    }

    public interface IUnderAttack
    {
        void UnderAttack(AttackMessage message);
    }
}

namespace CallProcess.Application.Interfaces.CallProcessing
{
    public interface ICallManager
    {
        Task Init();

        Task Start();

        string GetByCode(string code);
    }
}

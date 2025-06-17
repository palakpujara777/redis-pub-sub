namespace CallProcess.Application.Features.CallPrefixes.Commands
{
    public class DeleteCallPrefixCommand
    {
        public string Code { get; set; }

        public DeleteCallPrefixCommand(string code)
        {
            Code = code;
        }
    }
}
namespace CallProcess.Application.Features.CallPrefixes.Queries
{
    public class CheckCallPrefixExistsQuery
    {
        public string Code { get; }

        public CheckCallPrefixExistsQuery(string code)
        {
            Code = code;
        }
    }
}

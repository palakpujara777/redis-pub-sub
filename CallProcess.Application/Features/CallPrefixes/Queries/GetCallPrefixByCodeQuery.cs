namespace CallProcess.Application.Features.CallPrefixes.Queries
{
    public class GetCallPrefixByCodeQuery
    {
        public string Code { get; }

        public GetCallPrefixByCodeQuery(string code)
        {
            Code = code;
        }
    }
}

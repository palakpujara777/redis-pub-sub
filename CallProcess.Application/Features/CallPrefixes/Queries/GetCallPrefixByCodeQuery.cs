namespace CallProcess.Application.Features.CallPrefixes.Queries
{
    public class GetCallPrefixByCodeQuery
    {
        public string Code { get; set; }

        public GetCallPrefixByCodeQuery(string code)
        {
            Code = code;
        }
    }
}

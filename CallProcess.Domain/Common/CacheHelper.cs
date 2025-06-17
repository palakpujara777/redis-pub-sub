namespace CallProcess.Domain.Common
{
    public static class CacheHelper
    {
        public const string CountryCodeKey = "CountryCodes";

        public const string UpdatePublishChannel = "CodeUpdated";
        
        public const string DeletePublishChannel = "CodeDeleted";

        public const string CountryCodePrefix = "Code_";

        public const int DuplicateRetentionPeriod = 1;
    }
}

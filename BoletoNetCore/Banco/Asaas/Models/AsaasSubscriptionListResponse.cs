using System.Collections.Generic;

namespace BoletoNetCore
{
    public class AsaasSubscriptionListResponse
    {
        public bool hasMore { get; set; }
        public int totalCount { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public List<AsaasSubscriptionResponse> data { get; set; } = new List<AsaasSubscriptionResponse>();
    }
}

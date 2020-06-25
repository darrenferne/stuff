using Brady.Limits.PreliminaryContract.Domain.Enums;

namespace Brady.Limits.PreliminaryContract.Domain.Models
{
    public class Contract
    {
        public long Id { get; set; }
        public ContractStatus ContractStatus { get; set; }
    }
}

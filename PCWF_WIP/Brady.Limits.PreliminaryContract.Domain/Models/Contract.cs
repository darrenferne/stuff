using Brady.Limits.PreliminaryContract.Domain.Enums;

namespace Brady.Limits.PreliminaryContract.Domain.Models
{
    public class Contract
    {
        public Contract()
        {
            GroupHeader = new ContractHeader();
        }

        public long Id { get; set; }
        public decimal? ContractValue { get; set; } 
        public ContractStatus ContractStatus { get; set; }
        public ContractHeader GroupHeader { get; set; }
    }
}

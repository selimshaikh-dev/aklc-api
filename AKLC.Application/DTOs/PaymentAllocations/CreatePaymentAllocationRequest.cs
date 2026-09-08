namespace AKLC.Application.DTOs.PaymentAllocations
{
    public class CreatePaymentAllocationRequest
    {
        public Guid StudentPaymentId { get; set; }

        public Guid PaymentScheduleId { get; set; }

        public decimal AllocatedAmount { get; set; }

        public string? Remarks { get; set; }
    }
}
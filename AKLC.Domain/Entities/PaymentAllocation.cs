using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class PaymentAllocation : BaseEntity
    {
        public Guid StudentPaymentId { get; set; }

        public Guid PaymentScheduleId { get; set; }

        public decimal AllocatedAmount { get; set; }

        public string? Remarks { get; set; }

        // Navigation
        public StudentPayment StudentPayment { get; set; } = null!;

        public PaymentSchedule PaymentSchedule { get; set; } = null!;
    }
}
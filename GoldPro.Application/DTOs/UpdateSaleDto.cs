using System;

namespace GoldPro.Application.DTOs
{
    public record UpdateSaleDto(
        string PaymentStatus,
        DateTime? DateTime
    );
}

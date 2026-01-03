using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldPro.Application.DTOs
{
    public record BusinessProfileDto(
     string BusinessName,
     string Gstin,
     string Pan,
     string Phone,
     string Email,
     string Address,
     string StateCode,
     string StateName,
     decimal Gold24KRate,
     decimal Gold22KRate,
     decimal Gold18KRate,
     decimal SilverRateGram,
     decimal SilverRateKg
 );

}

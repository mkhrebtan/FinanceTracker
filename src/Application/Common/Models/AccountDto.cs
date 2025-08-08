using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models;

public sealed record AccountDto
{
    public Guid Id { get; set; }

    public decimal Balance { get; set; }

    required public string Currency { get; set; }
}

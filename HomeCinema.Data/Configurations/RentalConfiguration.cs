﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCinema.Data.Configurations
{
    public class RentalConfiguration : EntityBaseConfiguration<Entities.Rental>
    {
        public RentalConfiguration()
        {
            Property(r => r.CustomerId).IsRequired();
            Property(r => r.StockId).IsRequired();
            Property(r => r.Status).IsRequired().HasMaxLength(10);
            Property(r => r.ReturnedDate).IsOptional();
        }
    }
}
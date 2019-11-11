﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Core.Models
{
    public abstract class BaseEntity
    {
        public string ID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public BaseEntity()
        {
            this.ID = Guid.NewGuid().ToString();
            this.CreatedDate = DateTime.Now;
        }
    }
}

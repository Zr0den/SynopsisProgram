﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class CreateOrderDto
    {
        public int CustomerId { get; set; }
        public string ProductIds { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Application.OperationResults
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<object> Errors { get; set; }
    }
}

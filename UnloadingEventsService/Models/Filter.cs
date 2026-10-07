using System;
using System.Collections.Generic;
using System.Text;

namespace UnloadingEventsService.Models
{
    internal class Filter
    {
        //{"type": "", "rows": [{"column": "", "value": ""}]}

        public string? type { get; set; }

        public List<Columns>? rows { get; set; }
    }
}

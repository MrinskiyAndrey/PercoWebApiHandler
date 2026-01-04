using System;
using System.Collections.Generic;
using System.Text;

namespace UnloadingEventsService.Models
{
    internal class Filter
    {
        //{"type": "", "rows": [{"column": "", "value": ""}]}

        //{"type": "or", "rows": [{"column": "in", "value": "2"},{"column": "in", "value": "1"},{"column": "in", "value": "3"}]}

        public string? type { get; set; }

        public List<Columns>? rows { get; set; }
    }
}

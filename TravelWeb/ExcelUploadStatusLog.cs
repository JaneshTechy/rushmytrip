using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class ExcelUploadStatusLog
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime? UploadedOn { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public int? TotalRows { get; set; }
        public int? ProcessedRows { get; set; }
    }
}

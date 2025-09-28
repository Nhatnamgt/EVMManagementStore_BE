using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.DTO
{
    public class ReportDTO
    {
        public int ReportId { get; set; }

        public string SenderName { get; set; }    

        public int? UserId { get; set; }

        public int? OrderId { get; set; }

        public string ReportType { get; set; }

        public DateOnly? CreatedDate { get; set; }

        public DateOnly? ResolvedDate { get; set; }

        public string Content { get; set; }

        public string Status { get; set; }
    }

    public class GetReportDTO
    {
        public int ReportId { get; set; }

        public string SenderName { get; set; }

        public string ReportType { get; set; }

        public DateOnly? CreatedDate { get; set; }

        public DateOnly? ResolvedDate { get; set; }

        public string Content { get; set; }

        public string Status { get; set; }
    }
}

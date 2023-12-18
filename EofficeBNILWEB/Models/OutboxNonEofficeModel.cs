

namespace EofficeBNILWEB.Models
{
   
    public class ReportNonOutboxLetterOutput
    {
        public Guid idmailingnoneoffice { get; set; }
        public string? letter_number { get; set; }

        public int delivery_type { get; set; }

        public int type_report { get; set; }

        public string deliveryname { get; set; }
        public string? nmrawb { get; set; }

        public string? tgl { get; set; }
        public DateTime ReceiptDate { get; set; }

        public string? expedition_name { get; set; }

        public string? sender_name { get; set; }
        public string? nip { get; set; }
        public string? unitname { get; set; }

        public string? kodeunit { get; set; }
        public string? docReceiver { get; set; }

        public int statuskirim { get; set; }

        public string statusname { get; set; }

        public string? nmrreferen { get; set; }

        public string address { get; set; }

        public string cretaeby { get; set; }

        public string updateby { get; set; }

        public string? ekspedisiStatus { get; set; }

        public string? kurirStatus { get; set; }

        public string? purposename { get; set; }
        public string? phonenumber { get; set; }
        public DateTime? DateUntil { get; set; }

        public string tgluntil { get; set; }

        public string? nmresi { get; set; }
    }

    public class ParamReportNonOuboxLetter
    {
        public string trackingNumber { get; set; }
        public int type_report { get; set; }
        public int delivery_type { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    #region Detail view
    public class OutputDetailsView
    {
        public string draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<ReportNonOutboxLetterOutput> data { get; set; }

    }

    public class ParamGetDetailsView
    {
        public string draw { get; set; }
        public int start { get; set; }
        public string sortColumn { get; set; }
        public string sortColumnDirection { get; set; }
        public string? searchValue { get; set; }
        public int pageSize { get; set; }

        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }

    }
    #endregion
}

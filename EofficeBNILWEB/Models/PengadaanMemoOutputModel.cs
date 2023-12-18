namespace EofficeBNILWEB.Models
{
    public class PengadaanMemoOutputModel
    {
        public Guid idPengadaan { get; set; }
        public int status_approve { get; set; }
        public string namePengadaan { get; set; }
        public string MinNominal { get; set; }
        public string Max_Nominal { get; set; }

        public string MinMaxNominal { get; set; }

        public string approveOne { get; set; }
        public string approveTwo { get; set; }
        public DateTime CREATED_ON { get; set; }
        public Guid CREATED_BY { get; set; }
        public DateTime MODIFIED_ON { get; set; }
        public Guid MODIFIED_BY { get; set; }
        public int status_delete { get; set; }

        public Guid IdUser { get; set; }
        public string Fullname { get; set; }

        public string PositionName { get; set; }

        public Guid idPosition { get; set; }

        public Guid idUnit { get; set; }
    }

    public class ParamGetMemoPengadaanById
    {
        public Guid idPengadaan { get; set; }

    }

    
}

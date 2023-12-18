namespace EofficeBNILWEB.Models
{
    public class GeneralOutputModel
    {
        public int PageCount { get; set; }
        public string Status { get; set; }
        public object Result { get; set; }
        public string Message { get; set; }
        public int PageNumber { get; set; }
    }
    public class LoginParam
    {
        public string Nip { get; set; }
        public string Password { get; set; }
    }
    public class LoginResponse
    {
        public string Token { get; set; }
        public Guid IdUser { get; set; }
        public string Nip { get; set; }
        public string Fullname { get; set; }
        public string Password { get; set; }
        public Guid IdPosition { get; set; }
        public string PositionName { get; set; }
        public Guid? parentIdPosition { get; set; }
        public Guid? parentIdUser { get; set; }
        public string IdGroup { get; set; }
        public Guid IdUnit { get; set; }
        public string UnitName { get; set; }
        public Guid IdBranch { get; set; }
        public string BranchName { get; set; }
        public string email { get; set; }
        public Int64 phone { get; set; }
        public Guid directorIdUnit { get; set; }
        public string directorUnitName { get; set; }
        public string directorUnitCode { get; set; }
    }
    public class StringmapOutput
    {
        public int attributeValue { get; set; }
        public string value { get; set; }
    }
    public class ParamGetStringmap
    {
        public string objectName { get; set; }
        public string attributeName { get; set; }
    }
    public class ParamGetPenerima
    {
        public string keyword { get; set; }
    }
    public class DashboardOutput
    {
        public int documentInCount { get; set; }
        public int inboxCount { get; set; }
        public int outboxCount { get; set; }
        public int signatureInCount { get; set; }
        public int NonEofficeInCount { get; set; }
        public int deliveryCount { get; set; }

        public int MemoCount { get; set; }

        public int LainnyaCount { get; set; }


        public MemoOutput docmemo { get; set; }
        public List<LetterOutput> listLetter { get; set; }
        public List<LetterOutput> listLetterOutbox { get; set; }
        public List<DocumentOutput> listDocument { get; set; }
        public List<DeliveryReportOutput> listDelivery { get; set; }

        public List<MemoOutput> listLetterMemo { get; set; }

        public List<OuputSignature> listSignature { get; set; }
        public List<OutputletterNonEoffice> listNonEoffice { get; set; }

        public List<OutputNotifikasiLainnya> listlainnya { get; set; }
    }

    public class ParamGetStringmapNonEoffice
    {
        public string objectName { get; set; }
        public string attributeName { get; set; }
    }

    public class StringmapOutputNonEoffice
    {
        public int attributeValue { get; set; }
        public string value { get; set; }
    }
    public class OutputContentTemplate
    {
        public Guid idContentTemplate { get; set; }
        public string templateName { get; set; }
        public string templateContent { get; set; }
        public int? isDeleted { get; set; }
    }
    public class ParamInsertContentTemplate
    {
        public string templateName { get; set; }
        public string templateContent { get; set; }
    }


    public class ParamInsertContentTemplateBulk
    {
        public string templateName { get; set; }
        public string templateContent { get; set; }
        public string[] parameter { get; set; }
    }
    public class ListParameter
    {
        public string parameter { get; set; }
    }
    public class ParamInsertTemplateBulk
    {
        public string templateName { get; set; }
        public string templateContent { get; set; }
        public List<ListParameter> parameter { get; set; }
    }

    public class ParamUpdateContentTemplate
    {
        public Guid idContentTemplate { get; set; }
        public string templateName { get; set; }
        public string templateContent { get; set; }
        public int? isDeleted { get; set; }
    }

    public class OutputNotifikasiLainnya
    {
        public Guid ID_NOTIFIKASI { get; set; }
        public Guid ID_LETTER { get; set; }
        public string PERIHAL { get; set; }
        public string STATUS_DOC { get; set; }
        public Guid ID_USER { get; set; }
        public string NAME_USER { get; set; }

        public Guid ID_USER_APPROVAL { get; set; }
        public string USER_APPROVAL { get; set; }

        public string NOTIFIKASI { get; set; }
        public DateTime? CREATED_ON { get; set; }
        public int STATUS_READ { get; set; }

    }

}

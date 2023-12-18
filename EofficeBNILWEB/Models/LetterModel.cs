namespace EofficeBNILWEB.Models
{
    public class ParamInsertLetterWeb
    {
        public int documentType { get; set; }
        public int saveType { get; set; }
        public int letterTypeCode { get; set; }
        public Guid idresponsesurat { get; set; }
        public Guid? idDocument { get; set; }
        public string trackingNumber { get; set; }
        public DateTime letterDate { get; set; }
        public string about { get; set; }
        public string? attachmentCount { get; set; }
        public string? priority { get; set; }
        public string senderName { get; set; }
        public string senderAddress { get; set; }
        public DateTime senderLetterDate { get; set; }
        public string senderLetterNumber { get; set; }
        public Guid[] idUserReceiver { get; set; }
        public Guid[] idUserCopy { get; set; }
        public string isiSurat { get; set; }
        public string? comment { get; set; }
        public int sumLetterByDoc { get; set; }
        public int receivedDocument { get; set; }
    }
    public class ParamInsertLetter
    {
        public int documentType { get; set; }
        public int saveType { get; set; }
        public int letterTypeCode { get; set; }
        public Guid idresponsesurat { get; set; }
        public Guid? idDocument { get; set; }
        public string? trackingNumber { get; set; }
        public DateTime letterDate { get; set; }
        public string about { get; set; }
        public string? attachmentCount { get; set; }
        public string? priority { get; set; }
        public string senderName { get; set; }
        public string senderAddress { get; set; }
        public DateTime senderLetterDate { get; set; }
        public string senderLetterNumber { get; set; }
        public List<ParamInsertReceiver> idUserReceiver { get; set; }
        public List<ParamInsertCopy> idUserCopy { get; set; }
        public string isiSurat { get; set; }
        public string? comment { get; set; }
    }

    public class ParamInsertReceiver
    {
        public Guid idUserReceiver { get; set; }
    }
    public class ParamInsertCopy
    {
        public Guid idUserCopy { get; set; }
    }
    public class ParamInsertAttachment
    {
        public Guid idLetter { get; set; }
        public string filename { get; set; }
        public int isDocLetter { get; set; }
    }
    public class OutputInsertAttachment
    {
        public Guid idLetter { get; set; }
    }
    public class ParamGetLetterWeb
    {
        public string draw { get; set; }
        public int start { get; set; }
        public string? sortColumn { get; set; }
        public string? sortColumnDirection { get; set; }
        public string? searchValue { get; set; }
        public int pageSize { get; set; }
        public string? letterNumber { get; set; }
        public string? about { get; set; }
        public int? letterType { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public int? readStatus { get; set; }
        public int searchType { get; set; }

        public string? onprogress { get; set; }

    }
    public class LetterOutputWeb
    {
        public string draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int pagelength { get; set; }
        public int limit { get; set; }
        public int draftCount { get; set; }
        public int outboxCount { get; set; }
        public List<LetterOutput> data { get; set; }

    }
    public class LetterOutput
    {
        public Guid idLetter { get; set; }
        public Guid? idDocument { get; set; }
        public string trackingNumber { get; set; }
        public string letterNumber { get; set; }
        public DateTime letterDate { get; set; }
        public string about { get; set; }
        public string? attachmentDesc { get; set; }
        public string? priority { get; set; }
        public int letterTypeCode { get; set; }
        public string letterTypeValue { get; set; }
        public string senderName { get; set; }
        public string senderAddress { get; set; }
        public DateTime? letterDateIn { get; set; }
        public string? letterNumberIn { get; set; }
        public int? documentTypeCode { get; set; }
        public string documentTypeCodeValue { get; set; }
        public int? outboxType { get; set; }
        public string outboxTypeCodeValue { get; set; }
        public int? resultType { get; set; }
        public string resultTypeCodeValue { get; set; }
        public int? signatureType { get; set; }
        public string signatureTypeCodeValue { get; set; }
        public int statusCode { get; set; }
        public string statusCodeValue { get; set; }
        public Guid createdByPositionId { get; set; }
        public string positionName { get; set; }
        public Guid createdBy { get; set; }
        public string fullname { get; set; }
        public DateTime? createdOn { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int readStatus { get; set; }
        public int is_disposisi { get; set; }
        public string approvalcounter { get; set; }
    }
    public class ParamDeleteLetter
    {
        public Guid idLetter { get; set; }

    }
    public class ParamGetDetailLetter
    {
        public Guid idLetter { get; set; }
        public string? lettertype { get; set; }

    }
    public class OutputDetailLetter
    {
        public LetterOutput letter { get; set; }
        public List<OutputListReceiver> receiver { get; set; }
        public List<OutputListCopy> copy { get; set; }
        public List<OutputLogLetter> log { get; set; }
        public OutputListAttachment docLetter { get; set; }
        public List<OutputListAttachment> attachment { get; set; }
        public List<OutputGetDispositionLetter> disposition { get; set; }
        public List<OutputListChecker> checker { get; set; }
        //public List<OutputListPengirim> Pengirim { get; set; }
        //public List<OutputListChecker> Approval { get; set; }

        public List<OutputListLetterSender> sender { get; set; }
        public List<OutputListOutgoingRecipient> outgoingRecipient { get; set; }
        public OutputDetailLetterContent letterContent { get; set; }

        public List<OutputListCheckerDelg> checkerDelg { get; set; }

    }
    public class OutputListReceiver
    {
        public Guid idReceiver { get; set; }
        public Guid idUser { get; set; }
        public Guid idPosition { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
    }
    public class OutputListCopy
    {
        public Guid idCopy { get; set; }
        public Guid idUser { get; set; }
        public Guid idPosition { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
    }
    public class OutputLogLetter
    {
        public Guid idLogLetter { get; set; }
        public Guid idLetter { get; set; }
        public string description { get; set; }
        public string comment { get; set; }
        public DateTime? createdOn { get; set; }
    }
    public class OutputListAttachment
    {
        public Guid idAttachment { get; set; }
        public Guid idLetter { get; set; }
        public string filename { get; set; }
        public int? isDocLetter { get; set; }
    }
    public class ParamDeleteAttachment
    {
        public Guid idAttachment { get; set; }
    }
    public class ParamPreviewNoLetter
    {
        public string letterType { get; set; }
    }
    public class ParamInsertLetterDisposition
    {
        public Guid idLetter { get; set; }
        public List<ListInsertUserDisposition> listUser { get; set; }

    }
    public class ParamInsertUserDisposition
    {
        public Guid idLetter { get; set; }
        public Guid[] idUser { get; set; }
        public Guid[] idPosition { get; set; }
        public string[] notes { get; set; }
    }
    public class ListInsertUserDisposition
    {
        public Guid idLetter { get; set; }
        public Guid idUser { get; set; }
        public Guid idPosition { get; set; }
        public string notes { get; set; }
    }
    public class OutputGetDispositionLetter
    {
        public Guid idDispositionHeader { get; set; }
        public Guid idDispositionChild { get; set; }
        public Guid idUserDispositionMaker { get; set; }
        public Guid idPositionDispositionMaker { get; set; }
        public string fullnameDispositionMaker { get; set; }
        public string positionDispositionMaker { get; set; }
        public Guid idUserDispositionReceiver { get; set; }
        public Guid idPositionDispositionReceiver { get; set; }
        public string fullnameDispositionReceiver { get; set; }
        public string positionDispositionReceiver { get; set; }
        public DateTime dispositionDate { get; set; }
        public string notes { get; set; }
    }
    public class ParamInsertLetterOutboxWeb
    {
        public int? outboxType { get; set; }
        public int saveType { get; set; }
        public int letterTypeCode { get; set; }
        public Guid idresponsesurat { get; set; }
        public DateTime letterDate { get; set; }
        public string about { get; set; }
        public int? resultType { get; set; }
        public int? signatureType { get; set; }
        public Guid bossPositionId { get; set; }
        public Guid bossUserId { get; set; }
        public Guid bossUnitId { get; set; }
        public int bossLevelId { get; set; }
        public string bossPositionName { get; set; }
        public string? attachmentCount { get; set; }
        public string? priority { get; set; }
        public string[] senderName { get; set; }
        public string senderCompanyName { get; set; }
        public string senderAddress { get; set; }
        public string senderPhoneNumber { get; set; }
        public Guid[] idUserChecker { get; set; }
        public int[] idLevelChecker { get; set; }
        public Guid[] idUnitChecker { get; set; }

        //public Guid[] idUserApprover { get; set; }
        //public int[] idLevelApprover { get; set; }
        //public Guid[] idUnitApprover { get; set; }

        public string isiSurat { get; set; }
        public string? comment { get; set; }
    }
    public class ParamInsertLetterOutbox
    {
        public int? outboxType { get; set; }
        public int saveType { get; set; }
        public int letterTypeCode { get; set; }
        public Guid idresponsesurat { get; set; }
        public DateTime letterDate { get; set; }
        public string about { get; set; }
        public int? resultType { get; set; }
        public int? signatureType { get; set; }
        public Guid bossPositionId { get; set; }
        public Guid bossUserId { get; set; }
        public Guid bossUnitId { get; set; }
        public int bossLevelId { get; set; }
        public string bossPositionName { get; set; }
        public string? attachmentCount { get; set; }
        public string? priority { get; set; }
        public List<ParamInsertOutgoingRecipient> senderName { get; set; }
        public string senderCompanyName { get; set; }
        public string senderAddress { get; set; }
        public string senderPhoneNumber { get; set; }
        public List<ParamInsertChecker> idUserChecker { get; set; }

        //public List<ParamInsertApprover> idUserApprover { get; set; }
        //public List<ParamInsertCheckerPengirim> idUserCheckerPengirim { get; set; }


        public string isiSurat { get; set; }
        public string? comment { get; set; }
    }
    public class ParamInsertChecker
    {
        public Guid idUserChecker { get; set; }
        public int idLevelChecker { get; set; }
        public Guid idUnitChecker { get; set; }
    }
    public class ParamInsertOutgoingRecipient
    {
        public string senderName { get; set; }
        public string senderCompanyName { get; set; }
        public string senderAddress { get; set; }
        public string senderPhoneNumber { get; set; }
    }
    public class OutputListLetterSender
    {
        public Guid idLetterSender { get; set; }
        public Guid idUserSender { get; set; }
        public Guid idPositionSender { get; set; }
        public Guid idUnitSender { get; set; }
        public int idLevelSender { get; set; }
        public int isMain { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public string nip { get; set; }
    }
    public class OutputListChecker
    {
        public Guid idChecker { get; set; }
        public Guid idUserChecker { get; set; }
        public Guid idPositionChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public int idLevelChecker { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }
    }

    public class OutputListCheckerDelg
    {
        public Guid idChecker { get; set; }
        public Guid idUserChecker { get; set; }
        public Guid idPositionChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public int idLevelChecker { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }
    }
    public class OutputListOutgoingRecipient
    {
        public Guid idOutgoingRecipient { get; set; }
        public string recipientName { get; set; }
        public string recipientCompany { get; set; }
        public string recipientAddress { get; set; }
        public string recipientPhoneNumber { get; set; }
        public string description { get; set; }
    }
    public class OutputDetailLetterContent
    {
        public Guid idContent { get; set; }
        public string letterContent { get; set; }
    }
    public class ParamApprovalLetter
    {
        public int saveType { get; set; }
        public Guid idresponsesurat { get; set; }
        public string? comment { get; set; }
    }
    public class OutputPreviewLetter
    {
        public string Status { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
    }

    public class ParamInsertApprover
    {
        public Guid idUserApprover { get; set; }
        public int idLevelApprover { get; set; }
        public Guid idUnitApprover { get; set; }
        public int is_approver { get; set; }
    }

    public class ParamInsertCheckerPengirim
    {
        public Guid idUserChecker { get; set; }
        public int idLevelChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public int Is_Approver { get; set; }
        public int Is_Delegasi { get; set; }


    }

    public class OutputListApproval
    {
        public Guid idChecker { get; set; }
        public Guid idUserChecker { get; set; }
        public Guid idPositionChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public int idLevelChecker { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }
    }


    public class OutputListPengirim
    {
        public Guid idChecker { get; set; }
        public Guid idUserChecker { get; set; }
        public Guid idPositionChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public int idLevelChecker { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }
    }


    public class ParamInsertLetterOutboxBackdateWeb
    {
        public int? outboxType { get; set; }
        public int saveType { get; set; }
        public int letterTypeCode { get; set; }
        public Guid idresponsesurat { get; set; }
        public DateTime letterDate { get; set; }
        public string about { get; set; }
        public int? resultType { get; set; }
        public int? signatureType { get; set; }
        public Guid bossPositionId { get; set; }
        public Guid bossUserId { get; set; }
        public Guid bossUnitId { get; set; }
        public int bossLevelId { get; set; }
        public string bossPositionName { get; set; }
        public string? attachmentCount { get; set; }
        public string? priority { get; set; }
        public string[] senderName { get; set; }
        public string senderCompanyName { get; set; }
        public string senderAddress { get; set; }
        public string senderPhoneNumber { get; set; }
        public Guid[] idUserChecker { get; set; }
        public int[] idLevelChecker { get; set; }
        public Guid[] idUnitChecker { get; set; }

        public Guid[] idUserApprover { get; set; }
        public int[] idLevelApprover { get; set; }
        public Guid[] idUnitApprover { get; set; }

        public string isiSurat { get; set; }
        public string? comment { get; set; }
    }
    public class ParamInsertLetterOutboxBackdate
    {
        public int? outboxType { get; set; }
        public int saveType { get; set; }
        public int letterTypeCode { get; set; }
        public Guid idresponsesurat { get; set; }
        public DateTime letterDate { get; set; }
        public string about { get; set; }
        public int? resultType { get; set; }
        public int? signatureType { get; set; }
        public Guid bossPositionId { get; set; }
        public Guid bossUserId { get; set; }
        public Guid bossUnitId { get; set; }
        public int bossLevelId { get; set; }
        public string bossPositionName { get; set; }
        public string? attachmentCount { get; set; }
        public string? priority { get; set; }
        public List<ParamInsertOutgoingRecipient> senderName { get; set; }
        public string senderCompanyName { get; set; }
        public string senderAddress { get; set; }
        public string senderPhoneNumber { get; set; }
        public List<ParamInsertChecker> idUserChecker { get; set; }

        public List<ParamInsertApprover> idUserApprover { get; set; }
        public List<ParamInsertCheckerPengirim> idUserCheckerPengirim { get; set; }


        public string isiSurat { get; set; }
        public string? comment { get; set; }
    }


    public class OutputDetailLetterBackdate
    {
        public LetterOutput letter { get; set; }
        public List<OutputListReceiver> receiver { get; set; }
        public List<OutputListCopy> copy { get; set; }
        public List<OutputLogLetter> log { get; set; }
        public OutputListAttachment docLetter { get; set; }
        public List<OutputListAttachment> attachment { get; set; }
        public List<OutputGetDispositionLetter> disposition { get; set; }
        public List<OutputListChecker> checker { get; set; }
        public List<OutputListPengirim> Pengirim { get; set; }
        public List<OutputListChecker> Approval { get; set; }

        public List<OutputListLetterSender> sender { get; set; }
        public List<OutputListOutgoingRecipient> outgoingRecipient { get; set; }
        public OutputDetailLetterContent letterContent { get; set; }

        public List<OutputListCheckerDelg> checkerDelg { get; set; }



      

    }

    public class ParamUpdateNotif
    {
        public Guid idNotif { get; set; }

    }

    public class ParamPushNotifikasi
    {
        public Guid idletter { get; set; }

        public Guid? idOneSignal { get; set; }



    }
}

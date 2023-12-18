using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EofficeBNILWEB.Models
{
    public class StringmapMemoOutput
    {
        //public Guid idLetter { get; set; }
        public Guid id_stringmap { get; set; }
        public int attributeValue { get; set; }
        public string value { get; set; }
    }
    public class DocumenMemotOutput
    {
        public Guid idDocument { get; set; }
        public int documentType { get; set; }
        public string documentTypeValue { get; set; }
        public string trackingNumber { get; set; }
        public string? returnNumber { get; set; }
        public int qtyTotal { get; set; }
        public string senderName { get; set; }
        public string docReceiver { get; set; }
        public string receivedDate { get; set; }
        public string receivedDateString { get; set; }
        public string distributionTime { get; set; }
        public int statusCode { get; set; }
        public int statusCodeDocument { get; set; }
        public string statusCodeValue { get; set; }
        public string descLog { get; set; }
        public DateTime? modifiedOn { get; set; }
        public DateTime? createdOnLog { get; set; }
        public int editableData { get; set; }
        public DocumentMemoReceiverOutput currentDocumentReceiver { get; set; }
        public List<DocumentMemoReceiverOutput> documentReceiver { get; set; }
        public List<DocumentMemoLogsOutput> documentLog { get; set; }
    }

    public class DocumentMemoReceiverOutput
    {
        public Guid idDocumentReceiver { get; set; }
        public Guid idDocument { get; set; }
        public Guid idUnit { get; set; }
        public string unitName { get; set; }
        public Guid? idUserTu { get; set; }
        public string tuNip { get; set; }
        public string tuName { get; set; }
        public int statusCode { get; set; }
        public string statusCodeValue { get; set; }
        public DateTime? receivedDate { get; set; }
        public int? receivedDocument { get; set; }
        public int? returnedDocument { get; set; }
        public string? returnNumber { get; set; }
        public string mofied_by { get; set; }
    }

    public class DocumentMemoLogsOutput
    {
        public Guid idLogDocument { get; set; }
        public Guid idDocument { get; set; }
        public string description { get; set; }
        public string comment { get; set; }
        public DateTime? createdOn { get; set; }
    }

    public class DocumenMemotOutputWeb
    {
        public string draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<DocumentOutput> data { get; set; }

    }

    public class ParamInsertMemoWeb
    {
        public int? outboxType { get; set; }
        public Guid idMemoType { get; set; }
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
        public string? attachmentCount { get; set; }
        public string? priority { get; set; }

        public Guid idMinMax { get; set; }
        public Guid idMinMax2 { get; set; }
        public decimal? MinMaxNomnal { get; set; }
        //public string senderAddress { get; set; }
        public Guid[] idUserChecker { get; set; }
        public int[] idLevelChecker { get; set; }
        public Guid[] idUnitChecker { get; set; }
        public string isiSurat { get; set; }
        public string? comment { get; set; }

        public string? summary { get; set; }

        public Guid[] idUserCheckerPenerima { get; set; }
        public int[] idLevelCheckerPenerima { get; set; }
        public Guid[] idUnitCheckerPenerima { get; set; }

        public Guid[] idUserCheckerCarbonCopy { get; set; }
        public int[] idLevelCheckerCarbonCopy { get; set; }
        public Guid[] idUnitCheckerCarbonCopy { get; set; }


        public Guid[] idUserApprover { get; set; }
        public int[] idLevelApprover { get; set; }
        public Guid[] idUnitApprover { get; set; }


        public Guid[] idUserCheckerDelibretion { get; set; }
        public int[] idLevelCheckerDelibretion { get; set; }
        public Guid[] idUnitCheckerDelibretion { get; set; }

        public Guid[] idUserApprovalPengadaan { get; set; }
        public Guid[] idUserApprovalPositionPengadaan { get; set; }
        public Guid[] idUserApprovalunitPengadaan { get; set; }

        public Guid[] idUserCheckerlain { get; set; }
        public int[] idLevelCheckerlain { get; set; }
        public Guid[] idUnitCheckerlain { get; set; }
    }

    public class ParamInsertMemo
    {
        public int? outboxType { get; set; }
        public Guid idMemoType { get; set; }
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
        public string? attachmentCount { get; set; }
        public string? priority { get; set; }

        //public List<ParamInsertSendMemogoingRecipient> senderName { get; set; }

        public List<ParamInsertCheckerMemoPenerima> idUserCheckerPenerima { get; set; }

        public List<ParamInsertCheckerMemoCarbonCopy> idUserCheckerCarbonCopy { get; set; }

        public List<ParamInsertCheckerMemoDelibretion> idUserCheckerDelibretion { get; set; }

        public string senderCompanyName { get; set; }
        public string senderAddress { get; set; }
        public List<ParamInsertCheckerMemo> idUserChecker { get; set; }
        public List<ParamInsertApproverMemo> idUserApprover { get; set; }

        public List<ParamInsertCheckerMemoLainya> idUserCheckerlain { get; set; }

        public List<ParamInsertDelibrationMemo> idUserDelibration { get; set; }

        public string isiSurat { get; set; }
        public string? comment { get; set; }

        public string? summary { get; set; }

        public List<ParamInsertApprovalPengadaan> idUserApprovalPengadaan { get; set; }
    }



    public class ParamInsertCheckerMemoPenerima
    {
        public Guid idUserCheckerPenerima { get; set; }
        public int idLevelCheckerPenerima { get; set; }
        public Guid idUnitCheckerPenerima { get; set; }
    }

    public class ParamInsertCheckerMemoCarbonCopy
    {
        public Guid idUserCheckerCarbonCopy { get; set; }
        public int idLevelCheckerCarbonCopy { get; set; }
        public Guid idUnitCheckerCarbonCopy { get; set; }
    }


    public class ParamInsertCheckerMemoDelibretion
    {
        public Guid idUserCheckerDelibretion { get; set; }
        public int idLevelCheckerDelibretion { get; set; }
        public Guid idUnitCheckerDelibretion { get; set; }
    }

    public class ParamInsertCheckerMemo
    {
        public Guid idUserChecker { get; set; }
        public int idLevelChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public int Is_Approver { get; set; }
        public int Is_Delegasi { get; set; }


    }

    public class ParamInsertCheckerMemoLainya
    {
        public Guid idUserCheckerlain { get; set; }
        public int idLevelCheckerlain { get; set; }
        public Guid idUnitCheckerlain { get; set; }
        public int Is_Approver { get; set; }
        public int Is_Delegasi { get; set; }


    }

    public class ParamInsertSendMemogoingRecipient
    {
        public string senderName { get; set; }
        public string senderCompanyName { get; set; }
        public string senderAddress { get; set; }

        
    }

    public class ParamInsertAttachmentMemo
    {
        public Guid idLetter { get; set; }
        public string filename { get; set; }
        public int isDocLetter { get; set; }
    }

    public class OutputInsertAttachmentMemo
    {
        public Guid idLetter { get; set; }
    }



    public class ParamGetMemoWeb
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
    public class MemoOutputWeb
    {
        public string draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int pagelength { get; set; }
        public int limit { get; set; }
        public int draftCount { get; set; }
        public int outboxCount { get; set; }
        public List<MemoOutput> data { get; set; }

    }
    public class MemoOutput
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

        public string? memotypename { get; set; }

        public Guid idmemoType { get; set; }
        public string letterDeliberationNumber { get; set; }
    }

    public class ParamGetDetailMemo
    {
        public Guid idLetter { get; set; }

        public string? lettertype { get; set; }

    }

    public class OutputListReceiverMemo
    {
        public Guid idReceiver { get; set; }
        public Guid idUser { get; set; }
        public Guid idPosition { get; set; }

        public Guid idUnit { get; set; }

        public string fullname { get; set; }
        public string positionName { get; set; }
    }

    public class OutputListCopyMemo
    {
        public Guid idCopy { get; set; }
        public Guid idUser { get; set; }
        public Guid idPosition { get; set; }

        public Guid idUnit { get; set; }

        public string fullname { get; set; }
        public string positionName { get; set; }
    }


    public class OutputLogLetterMemo
    {
        public Guid idLogLetter { get; set; }
        public Guid idLetter { get; set; }
        public Guid idUserLog { get; set; }
        public string description { get; set; }
        public string comment { get; set; }
        public DateTime? createdOn { get; set; }
    }


    public class OutputListAttachmentMemo
    {
        public Guid idAttachment { get; set; }
        public Guid idLetter { get; set; }
        public string filename { get; set; }
        public int? isDocLetter { get; set; }
    }


    public class OutputGetDispositionMemo
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

    public class OutputListCheckerMemo
    {
        public Guid idChecker { get; set; }
        public Guid idUserChecker { get; set; }
        public Guid idPositionChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public Guid idSekertaris { get; set; }
        public int idLevelChecker { get; set; }
        public string nip { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }

        public int ordernumber { get; set; }

        public int? isapproval { get; set; }
    }


    public class OutputListCheckerMemoLainya
    {
        public Guid idChecker { get; set; }
        public Guid idUserChecker { get; set; }
        public Guid idPositionChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public Guid idSekertaris { get; set; }
        public int idLevelChecker { get; set; }
        public string nip { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }

        public int ordernumber { get; set; }
    }


    public class OutputListCheckerMemoDelg
    {
        public Guid idChecker { get; set; }
        public Guid idUserChecker { get; set; }
        public Guid idPositionChecker { get; set; }
        public Guid idUnitChecker { get; set; }

        public Guid idSekertaris { get; set; }
        public int idLevelChecker { get; set; }
        public string nip { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }

        public int ordernumber { get; set; }

        public int? isapproval { get; set; }
    }

    public class OutputListCheckerMemoLainyaDelg
    {
        public Guid idChecker { get; set; }
        public Guid idUserChecker { get; set; }
        public Guid idPositionChecker { get; set; }
        public Guid idUnitChecker { get; set; }

        public Guid idSekertaris { get; set; }
        public int idLevelChecker { get; set; }
        public string nip { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }

        public int ordernumber { get; set; }
    }


    public class OutputListLetterSenderMemo
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

    //public class OutputListOutgoingRecipientMemo
    //{
    //    public Guid idOutgoingRecipient { get; set; }
    //    public string recipientName { get; set; }
    //    public string recipientCompany { get; set; }
    //    public string recipientAddress { get; set; }
    //    public string description { get; set; }
    //}

    public class OutputDetailLetterContentMemo
    {
        public Guid idContent { get; set; }
        public string letterContent { get; set; }
        public string? summary { get; set; }
    }
    public class OutputDetailMemo
    {
        public MemoOutput letter { get; set; }
        
        public List<OutputListReceiverMemo> receiver { get; set; }
        
        public List<OutputListCopyMemo> copy { get; set; }
        
        public List<OutputLogLetterMemo> log { get; set; }
        
        public OutputListAttachmentMemo docLetter { get; set; }
        
        public List<OutputListAttachmentMemo> attachment { get; set; }
        
        public List<OutputGetDispositionMemo> disposition { get; set; }

        public List<OutputListCheckerMemoPenerima> checkerPenerima { get; set; }

        public List<OutputListCheckerMemoPenerimaDelg> checkerPenerimaDelg { get; set; }

        public List<OutputListCheckerMemo> checker { get; set; }
        
        public List<OutputListCheckerMemoLainya> checkerlain { get; set; }

        public List<OutputListCheckerMemoDelg> checkerDelg { get; set; }

        public List<OutputListCheckerMemoLainyaDelg> checkerlainDelg { get; set; }

        public List<OutputListCheckerMemoSek> checkerSekertaris { get; set; }

        public List<OutputListLetterSenderMemo> sender { get; set; }
        
        //public List<OutputListOutgoingRecipientMemo> outgoingRecipient { get; set; }
        
        public OutputDetailLetterContentMemo letterContent { get; set; }

        public List<OutputListDeliberationMemoModel> delibration { get; set; }

        public List<OutputListDeliberationDelegasiMemoModel> delibrationdelegasi { get; set; }

        public OutputDeliberationMemoModel delibrationRow { get; set; }




    }

    public class ParamApprovalLetterMemo
    {
        public int saveType { get; set; }
        public Guid idresponsesurat { get; set; }
        public string? comment { get; set; }

        public string commentdlbrt { get; set; }

        public Guid? idUserDelibration { get; set; }
    }


    //public class ParamApprovalLetterMemoDelebration
    //{
    //    public int saveType { get; set; }
    //    public Guid idresponsesurat { get; set; }
    //    public string? comment { get; set; }

    //    public string? commentdlbrt { get; set; }

    //    public Guid? idUserDelibration { get; set; }
    //}

    public class OutputPreviewMemo
    {
        public string Status { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
    }

    public class ParamDeleteAttachmentMemo
    {
        public Guid idAttachment { get; set; }
    }

    public class ParamGetMemoTypeById
    {
        //public Guid stingMapId { get; set; }
        public int? stingMapVal { get; set; }
    }

    public class OutputListDeliberationMemoModel
    {
        public Guid idLetter { get; set; }
        public Guid idDeliberation { get; set; }
        public Guid idUser { get; set; }
        public Guid idPosition { get; set; }
        public Guid idUnit { get; set; }
        public int statuscode { get; set; }
        public string? commentdlbrt { get; set; }

        public string fullname { get; set; }
        public string positionName { get; set; }

    }


    public class OutputListDeliberationDelegasiMemoModel
    {
        public Guid idLetter { get; set; }
        public Guid idDeliberation { get; set; }
        public Guid idUser { get; set; }
        public Guid idPosition { get; set; }
        public Guid idUnit { get; set; }
        public int statuscode { get; set; }
        public string? commentdlbrt { get; set; }

        public string fullname { get; set; }
        public string positionName { get; set; }

    }

    public class OutputDeliberationMemoModel
    {
        public Guid idLetter { get; set; }
        public Guid idDeliberation { get; set; }
        public Guid idUser { get; set; }
        public Guid idPosition { get; set; }
        public Guid idUnit { get; set; }
        public int statuscode { get; set; }
        public string? commentdlbrt { get; set; }

        public string fullname { get; set; }
        public string positionName { get; set; }

    }


    public class MemotypeOuput
    {
        public Guid idmemotype { get; set; }
        public Guid idstringmap { get; set; }

        public string memotypename { get; set; }

        public DateTime? createdon { get; set; }
        public Guid createdby { get; set; }
        public DateTime? modifiedon { get; set; }
        public Guid modifiedby { get; set; }

        public int statuscode { get; set; }
    }
    public class ParamInsertApproverMemo
    {
        public Guid idUserApprover { get; set; }
        public int idLevelApprover { get; set; }
        public Guid idUnitApprover { get; set; }
        public int is_approver { get; set; }
    }

    public class ParamInsertDelibrationMemo
    {
        public Guid idUserDelibration { get; set; }
        public int idLevelDelibration { get; set; }
        public Guid idUnitDelibration { get; set; }
    }


    public class ParamInsertMemoBackDateWeb
    {
        public int? outboxType { get; set; }
        public Guid idMemoType { get; set; }
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
        public string? attachmentCount { get; set; }
        public string? priority { get; set; }
        //public string[] senderName { get; set; }
        //public string senderCompanyName { get; set; }
        //public string senderAddress { get; set; }
        public Guid[] idUserChecker { get; set; }
        public int[] idLevelChecker { get; set; }
        public Guid[] idUnitChecker { get; set; }
        public string isiSurat { get; set; }
        public string? comment { get; set; }

        public string? summary { get; set; }

        public Guid[] idUserCheckerPenerima { get; set; }
        public int[] idLevelCheckerPenerima { get; set; }
        public Guid[] idUnitCheckerPenerima { get; set; }

        public Guid[] idUserCheckerCarbonCopy { get; set; }
        public int[] idLevelCheckerCarbonCopy { get; set; }
        public Guid[] idUnitCheckerCarbonCopy { get; set; }

        public Guid[] idUserApprover { get; set; }
        public int[] idLevelApprover { get; set; }
        public Guid[] idUnitApprover { get; set; }

        public Guid[] idUserCheckerDelibretion { get; set; }
        public int[] idLevelCheckerDelibretion { get; set; }
        public Guid[] idUnitCheckerDelibretion { get; set; }

        public Guid[] idUserCheckerlain { get; set; }
        public int[] idLevelCheckerlain { get; set; }
        public Guid[] idUnitCheckerlain { get; set; }
    }


    public class ParamInsertMemoBackdate
    {
        public int? outboxType { get; set; }
        public Guid idMemoType { get; set; }
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
        public string? attachmentCount { get; set; }
        public string? priority { get; set; }

        //public List<ParamInsertSendMemogoingRecipient> senderName { get; set; }

        public List<ParamInsertCheckerMemoPenerima> idUserCheckerPenerima { get; set; }

        public List<ParamInsertCheckerMemoCarbonCopy> idUserCheckerCarbonCopy { get; set; }
        public string senderCompanyName { get; set; }
        public string senderAddress { get; set; }
        public List<ParamInsertCheckerMemo> idUserChecker { get; set; }

        public List<ParamInsertCheckerMemoPengirim> idUserCheckerPengirim { get; set; }

        public List<ParamInsertApproverMemo> idUserApprover { get; set; }

        public List<ParamInsertDelibrationMemo> idUserDelibration { get; set; }

        public List<ParamInsertCheckerMemoLainya> idUserCheckerlain { get; set; }

        public string isiSurat { get; set; }
        public string? comment { get; set; }

        public string? summary { get; set; }


    }
    public class OutputListApproverMemo
    {
        public Guid idApprover { get; set; }
        public Guid idUserApprover { get; set; }
        public Guid idPositionApprover { get; set; }
        public Guid idUnitApprover { get; set; }
        public int idLevelApprover { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }
    }

    public class OutputListApproverMemoDelg
    {
        public Guid idApprover { get; set; }
        public Guid idUserApprover { get; set; }
        public Guid idPositionApprover { get; set; }
        public Guid idUnitApprover { get; set; }
        public int idLevelApprover { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }
    }


    public class OutputDetailMemoBackDate
    {
        public MemoOutput letter { get; set; }

        public List<OutputListReceiverMemo> receiver { get; set; }

        public List<OutputListCopyMemo> copy { get; set; }

        public List<OutputLogLetterMemo> log { get; set; }

        public OutputListAttachmentMemo docLetter { get; set; }

        public List<OutputListAttachmentMemo> attachment { get; set; }

        public List<OutputGetDispositionMemo> disposition { get; set; }

        public List<OutputListCheckerMemo> checker { get; set; }
        public List<OutputListCheckerMemoPengirim> checkerPengirim { get; set; }

        public List<OutputListCheckerMemoPenerima> checkerPenerima { get; set; }



        public List<OutputListCheckerMemoLainya> checkerlain { get; set; }

        public List<OutputListCheckerMemoSek> checkerSekertaris { get; set; }

        public List<OutputListLetterSenderMemo> sender { get; set; }

        //public List<OutputListOutgoingRecipientMemo> outgoingRecipient { get; set; }

        public OutputDetailLetterContentMemo letterContent { get; set; }

        public List<OutputListDeliberationMemoModel> delibration { get; set; }

        
        public OutputDeliberationMemoModel delibrationRow { get; set; }

        public List<OutputListApproverMemo> Approver { get; set; }


        public List<OutputListCheckerMemoPengirimDelg> checkerPengirimDelg { get; set; }
        public List<OutputListCheckerMemoDelg> checkerDelg { get; set; }
        public List<OutputListCheckerMemoLainyaDelg> checkerlainDelg { get; set; }
        public List<OutputListApproverMemoDelg> ApproverDelg { get; set; }
        public List<OutputListCheckerMemoPenerimaDelg> checkerPenerimaDelg { get; set; }
        public List<OutputListDeliberationDelegasiMemoModel> delibrationdelegasi { get; set; }

    }

    public class ParamCheckMinMaxNomial
    {
        public string minMaxNomnal { get; set; }

        public Guid IdPengadaan { get; set; }
    }


    public class ParamInsertApprovalPengadaan
    {
        public Guid idUserApprovalPengadaan { get; set; }
        public Guid idUserApprovalPositionPengadaan { get; set; }
        public Guid idUserApprovalunitPengadaan { get; set; }

        public int is_approver { get; set; }
    }

    public class ParamInsertMemoPengadaan
    {
        public int? outboxType { get; set; }
        public Guid idMemoType { get; set; }
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
        public string? attachmentCount { get; set; }
        public string? priority { get; set; }

        public Guid idMixMax { get; set; }

        //public Guid idMixMax2 { get; set; }

        public decimal? MinMaxNomnal { get; set; }

        //public List<ParamInsertSendMemogoingRecipient> senderName { get; set; }

        public List<ParamInsertCheckerMemoPenerima> idUserCheckerPenerima { get; set; }

        public List<ParamInsertCheckerMemoCarbonCopy> idUserCheckerCarbonCopy { get; set; }

        public List<ParamInsertCheckerMemoDelibretion> idUserCheckerDelibretion { get; set; }

        public string senderCompanyName { get; set; }
        public string senderAddress { get; set; }
        public List<ParamInsertCheckerMemo> idUserChecker { get; set; }

        public List<ParamInsertCheckerMemoPengirim> idUserCheckerPengirim { get; set; }

        public List<ParamInsertCheckerMemoLainya> idUserCheckerlain { get; set; }

        public List<ParamInsertApproverMemo> idUserApprover { get; set; }

        public List<ParamInsertDelibrationMemo> idUserDelibration { get; set; }

        public string isiSurat { get; set; }
        public string? comment { get; set; }

        public string? summary { get; set; }

        public List<ParamInsertApprovalPengadaan> idUserApprovalPengadaan { get; set; }
    }

    public class OutputDetailMemoPengadaan
    {
        public MemoOutput letter { get; set; }

        public List<OutputListReceiverMemo> receiver { get; set; }

        public List<OutputListCopyMemo> copy { get; set; }

        public List<OutputLogLetterMemo> log { get; set; }

        public OutputListAttachmentMemo docLetter { get; set; }

        public List<OutputListAttachmentMemo> attachment { get; set; }

        public List<OutputGetDispositionMemo> disposition { get; set; }

        public List<OutputListCheckerMemo> checker { get; set; }

        public List<OutputListCheckerMemoLainya> checkerlain { get; set; }
        public List<OutputListCheckerMemoSek> checkerSekertaris { get; set; }

        public List<OutputListLetterSenderMemo> sender { get; set; }

        //public List<OutputListOutgoingRecipientMemo> outgoingRecipient { get; set; }

        public OutputDetailLetterContentMemo letterContent { get; set; }

        public List<OutputListDeliberationMemoModel> delibration { get; set; }

       

        public OutputDeliberationMemoModel delibrationRow { get; set; }

        public List<OutputListApproverMemo> ApproverOne { get; set; }

        public List<OutputListApproverMemo> ApproverTwo { get; set; }

        public List<OutputListApproverMemo> ApproverListAllPd { get; set; }

        public OutputDetailNominalMemoPengadaan Nominal { get; set; }

        public List<OutputListCheckerMemoPengirim> checkerPengirim { get; set; }

        public List<OutputListCheckerMemoPenerima> checkerPenerima { get; set; }


        public List<OutputListCheckerMemoPenerimaDelg> checkerPenerimaDelg { get; set; }
        public List<OutputListCheckerMemoPengirimDelg> checkerPengirimDelg { get; set; }

        public List<OutputListCheckerMemoDelg> checkerDelg { get; set; }
        public List<OutputListCheckerMemoLainyaDelg> checkerlainDelg { get; set; }
        public List<OutputListApproverMemoDelg> ApproverOneDelg { get; set; }

        public List<OutputListApproverMemoDelg> ApproverTwoDelg { get; set; }
        
        public List<OutputListDeliberationDelegasiMemoModel> delibrationdelegasi { get; set; }

    }

    public class OutputDetailNominalMemoPengadaan
    {
        public Guid idpr { get; set; }
        public Guid idprocurement { get; set; }
        public Guid idletter { get; set; }
        public decimal? nominal { get; set; }

        public string nameProcurement { get; set; }

    }


    public class OutputListCheckerMemoSek
    {
        public Guid idChecker { get; set; }
        public Guid idUserChecker { get; set; }
        public Guid idPositionChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public Guid idSekertaris { get; set; }
        public int idLevelChecker { get; set; }
        public string nip { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }

        public int ordernumber { get; set; }
    }


    public class OutputListCheckerMemoPenerima
    {
        public Guid idChecker { get; set; }
        public Guid idUserChecker { get; set; }
        public Guid idPositionChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public Guid idSekertaris { get; set; }
        public int idLevelChecker { get; set; }
        public string nip { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }

        public int ordernumber { get; set; }
    }

    public class OutputListCheckerMemoPenerimaDelg
    {
        public Guid idChecker { get; set; }
        public Guid idUserChecker { get; set; }
        public Guid idPositionChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public Guid idSekertaris { get; set; }
        public int idLevelChecker { get; set; }
        public string nip { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }

        public int ordernumber { get; set; }
    }

    public class ParamInsertCheckerMemoPengirim
    {
        public Guid idUserChecker { get; set; }
        public int idLevelChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public int Is_Approver { get; set; }
        public int Is_Delegasi { get; set; }


    }


    

    public class OutputListCheckerMemoPengirim
    {
        public Guid idChecker { get; set; }
        public Guid idUserChecker { get; set; }
        public Guid idPositionChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public Guid idSekertaris { get; set; }
        public int idLevelChecker { get; set; }
        public string nip { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }

        public int ordernumber { get; set; }
    }


    public class OutputListCheckerMemoPengirimDelg
    {
        public Guid idChecker { get; set; }
        public Guid idUserChecker { get; set; }
        public Guid idPositionChecker { get; set; }
        public Guid idUnitChecker { get; set; }
        public Guid idSekertaris { get; set; }
        public int idLevelChecker { get; set; }
        public string nip { get; set; }
        public string fullname { get; set; }
        public string positionName { get; set; }
        public char approvalStatus { get; set; }

        public int ordernumber { get; set; }
    }

    public class ParamGetLetterWebMemo
    {
        public string draw { get; set; }
        public int start { get; set; }
        public string? sortColumn { get; set; }
        public string? sortColumnDirection { get; set; }
        public string? searchValue { get; set; }
        public int pageSize { get; set; }
        public string? letterNumber { get; set; }

        //public int? letterOutboxType { get; set; }
        //public Guid? idMemoType { get; set; }

        public string? about { get; set; }
        public int? letterType { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public int? readStatus { get; set; }
        public int searchType { get; set; }

    }

}

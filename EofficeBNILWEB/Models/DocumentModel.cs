using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EofficeBNILWEB.Models
{
    public class DocumentOutput
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
        public DocumentReceiverOutput currentDocumentReceiver { get; set; }
        public List<DocumentReceiverOutput> documentReceiver { get; set; }
        public List<DocumentLogsOutput> documentLog { get; set; }
        public string receiverDivisionName { get; set; }
        public string createdByName { get; set; }
    }
    public class DocumentReceiverOutput
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
    public class DocumentLogsOutput
    {
        public Guid idLogDocument { get; set; }
        public Guid idDocument { get; set; }
        public string description { get; set; }
        public string comment { get; set; }
        public DateTime? createdOn { get; set; }
    }
    public class ParamGetDocument
    {
        public int pageNumber { get; set; }
        public Guid idUnit { get; set; }
        public Guid idBranch { get; set; }
    }
    public class ParamInsertDocument
    {
        public int documentType { get; set; }
        public string? trackingNumber { get; set; }
        public int? qtyTotal { get; set; }
        public string senderName { get; set; }
        public string docReceiver { get; set; }
        //public DateTime receivedDate { get; set; }
        public DateTime? distributionTime { get; set; }
        public Guid idUnit { get; set; }
        public Guid? idUserTu { get; set; }
        public string? comment { get; set; }
    }
    public class ParamCheckTrackingNumber
    {
        public string trackingNumber { get; set; }
    }
    public class ParamGetDetailDocument
    {
        public Guid idDocument { get; set; }
    }
    public class ParamGetDetailDocumentReceiver
    {
        public Guid idDocumentReceiver { get; set; }
    }
    public class ParamUpdateReceiver
    {
        public Guid idDocumentReceiver { get; set; }
        public Guid idUnit { get; set; }
        public Guid idUserTu { get; set; }
        public string returnNumber { get; set; }
    }
    public class ParamUpdateDocumentWeb
    {
        public Guid idDocument { get; set; }
        public int documentType { get; set; }
        public string trackingNumber { get; set; }
        public string senderName { get; set; }
        public string docReceiver { get; set; }
        //public DateTime receivedDate { get; set; }
        public DateTime? distributionTime { get; set; }
        public Guid[] idDocumentReceiver { get; set; }
        public Guid[] idUnit { get; set; }
        public Guid[] idUserTu { get; set; }
        public string[] returnNumber { get; set; }
        public Guid[] idUnitNew { get; set; }
        public Guid[] idUserTuNew { get; set; }
        public string[] returnNumberNew { get; set; }
        public string? comment { get; set; }
    }
    public class ParamUpdateDocument
    {
        public Guid idDocument { get; set; }
        public int documentType { get; set; }
        public string trackingNumber { get; set; }
        public string senderName { get; set; }
        public string docReceiver { get; set; }
        //public DateTime receivedDate { get; set; }
        public DateTime? distributionTime { get; set; }
        public List<ParamUpdateReceiver> receiverDocument { get; set; }
        public string? comment { get; set; }
    }
    public class ParamReceiveDocumentWeb
    {
        public Guid idDocument { get; set; }
        public Guid idDocumentReceiver { get; set; }
        public int qtyTotal { get; set; }
        public int receivedDocument { get; set; }
        public int returnedDocument { get; set; }
        public DateTime receivedDate { get; set; }
        public DateTime? distributionTime { get; set; }
        public string? comment { get; set; }
        public int flagReceive { get; set; }
        public string[] UnitNameNew { get; set; }
    }
    public class ParamReceiveDocument
    {
        public Guid idDocument { get; set; }
        public Guid idDocumentReceiver { get; set; }
        public int qtyTotal { get; set; }
        public int receivedDocument { get; set; }
        public int returnedDocument { get; set; }
        public DateTime receivedDate { get; set; }
        public DateTime? distributionTime { get; set; }
        public string? comment { get; set; }
        public int flagReceive { get; set; }
        public string UnitNameNew { get; set; }
    }
    public class ParamGetDocumentWeb
    {
        public string draw { get; set; }
        public int start { get; set; }
        public string sortColumn { get; set; }
        public string sortColumnDirection { get; set; }
        public string? searchValue { get; set; }
        public int pageSize { get; set; }

    }
    public class DocumentOutputWeb
    {
        public string draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<DocumentOutput> data { get; set; }

    }

    public class ParamInsertGenerateNoDoc
    {
        public int type { get; set; }
        public string userCode { get; set; }  
        public string barcode { get; set; }

        public string pdfbarcodeVal { get; set; }
        public string pdfbarcodeUrl { get; set; }

        public string reprintbarcode { get; set; }
    }
    public class ParamUploadDocumentString
    {
        public List<ParamUploadDocument> jsonDataString { get; set; }
    }
    public class ParamUploadDocument
    {
        public string documentType { get; set; }
        public string trackingNumber { get; set; }
        public string receiverUnit { get; set; }
        public string senderName { get; set; }
        public string docReceiver { get; set; }
        //public DateTime receivedDate { get; set; }
    }
    public class UploadDocumentOutput
    {
        public string documentType { get; set; }
        public string trackingNumber { get; set; }
        public string receiverUnit { get; set; }
        public string senderName { get; set; }
        public string docReceiver { get; set; }
        public DateTime receivedDate { get; set; }
        public string statusUpload { get; set; }
    }
    public class ParamGetDetailReportDocument
    {
        public string trackingNumber { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
    public class ParamReceiveCheckedDoc
    {
        public string listIdDoc { get; set; }
    }
    public class DocumentDistributionOutput
    {
        public Guid idDocument { get; set; }
        public Guid idDocumentReceiver { get; set; }
        public int? receivedDocument { get; set; }
        public int documentType { get; set; }
        public string documentTypeValue { get; set; }
        public string trackingNumber { get; set; }
        public string? returnNumber { get; set; }
        public int qtyTotal { get; set; }
        public string senderName { get; set; }
        public string docReceiver { get; set; }
        public DateTime? receivedDate { get; set; }
        public DateTime? distributionTime { get; set; }
        public int statusCode { get; set; }
        public string statusCodeValue { get; set; }
        public DateTime? modifiedOn { get; set; }
        public int letterByDoc { get; set; }
    }

    public class ParamGetDetailSearchoutgoingDocument
    {
        public string trackingNumber { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    public class DataOuputValidasiBarcode
    {
        public int type_brcd { get; set; }
        public string nmr_brcd { get; set; }
    }


    public class ParamInsertNonEoffice
    {
        
        public Guid idmailingnoneoffice { get; set; }

        public string letter_number { get; set; }

        public string? nip { get; set; }

        public int delivery_type { get; set; }

        public int type_report { get; set; }

        public string? nmrawb { get; set; }

        public string ReceiptDate { get; set; }

        public string? expedition_name { get; set; }

        public string? sender_name { get; set; }

        public Guid idUnit { get; set; }

        public string? docReceiver { get;set; }

        public string? nmrreferen { get; set; }

        public string? address { get; set; }

        public int statuskirim { get; set; }

        public int updtae { get;set;}

        public int qrcode { get; set; }

        public string? purposename { get; set; }

        public string? phonenumber { get; set; }

        public DateTime? DateUntil { get; set; }
    }
    public class UpdateNonEoffice
    {
        public string? letter_number { get; set; }

        public string? docReceiver { get; set; }

        public DateTime startDate { get; set; }

        public string? nmrawb { get; set; }

        public string? nmrreferen { get; set; }

        public int type_report { get; set; }

        public int statuskirim { get; set; }
    }
    public class getByIdNonEoffice
    {
        public string? letter_number { get; set; }
        public int type_report { get; set; }

        public Guid idmailingnoneoffice { get; set; }
    }

    public class OutputDetailDataNotifNon
    {
        public OutputletterNonEoffice NotifNonEoffice { get; set; }
    }
    public class OutputNotifLainyya
    {
        public OutputNotifikasiLainnya NotifNonEoffice { get; set; }
    }

    public class OutputletterNonEoffice
    {
        public Guid idmailingnoneoffice { get; set; }
        public string? letter_number { get; set; }

        public int delivery_type { get; set; }

        public int type_report { get; set; }

        public string? nmrawb { get; set; }

        public DateTime ReceiptDate { get; set; }

        public string? expedition_name { get; set; }

        public string? sender_name { get; set; }

        public string? nip { get; set; }
        public string? unitname { get; set; }
        public string? kodeunit { get; set; }

        public string? docReceiver { get; set; }

        public int statuskirim { get; set; }

        public string? nmrreferen { get; set; }

        public string address { get; set; }

        public int updtae { get; set; }

        public int qrcode { get; set; }

        public string statusname { get; set; }

        public string deliveryname { get; set; }

        public string cretaeby { get; set; }

        public string updateby { get; set; }

        public string? purposename { get; set; }
        public string? phonenumber { get; set; }
        public DateTime? DateUntil { get; set; }

        public string tgluntil { get; set; }

        public string? qrcodenumber { get; set; }

    }
    public class ParamUploadLetterNonOfficeString
    {
        public List<ParamUploadputletterNonEoffice> jsonDataString { get; set; }
    }
    public class ParamUploadputletterNonEoffice
    {
        public string letter_number { get; set; }

        public DateTime ReceiptDate { get; set; }

        public string type_report { get; set; }

        public string? nmrawb { get; set; }
        public string expedition_name { get; set; }

        public string sender_name { get; set; }

        public string nip { get; set; }

        public string unitname { get; set; }

        public string kodeunit { get; set; }

        public string docReceiver { get; set; }

        public string statuskirim { get; set; }

        public string nmrreferen { get; set; }

        public string address { get; set; }

        public string cretaeby { get; set; }

        public string updateby { get; set; }

        public string? purposename { get; set; }
        public string? phonenumber { get; set; }
        public DateTime? DateUntil { get; set; }

        public string tgluntil { get; set; }
    }
    public class UploadputletterNonEoffice
    {
        public string letter_number { get; set; }

        public DateTime ReceiptDate { get; set; }
        public string type_report { get; set; }
        public string expedition_name { get; set; }
        public string? nmrawb { get; set; }
        public string sender_name { get; set; }

        public string nip { get; set; }
        public string unitname { get; set; }

        public string docReceiver { get; set; }

        public string statuskirim { get; set; }

        public string nmrreferen { get; set; }

        public string address { get; set; }

        public string statusUpload { get; set; }

        public string? purposename { get; set; }
        public string? phonenumber { get; set; }
        public DateTime? DateUntil { get; set; }
    }

    public class UpdtaeUploadputletterNonEoffice
    {
        public string letter_number { get; set; }

        public DateTime ReceiptDate { get; set; }
        public string type_report { get; set; }
        public string expedition_name { get; set; }
        public string? nmrawb { get; set; }
        public string sender_name { get; set; }

        public string nip { get; set; }
        public string unitname { get; set; }
        public string kodeunit { get; set; }
        public string docReceiver { get; set; }

        public string statuskirim { get; set; }

        public string nmrreferen { get; set; }

        public string address { get; set; }

        public string statusUpload { get; set; }

        public string? purposename { get; set; }
        public string? phonenumber { get; set; }
        public DateTime? DateUntil { get; set; }
    }
    public class ParamGetEkspedisi
    {
        public string keyword { get; set; }
    }

    public class ParamGetKurir
    {
        public string keyword { get; set; }
    }
    public class ParamInsertGenerateNoDocNonEoffice
    {
        public Guid idmailingnoneoffice { get; set; }
        public int type { get; set; }
        public string userCode { get; set; }
        public string barcode { get; set; }

        public string pdfbarcodeVal { get; set; }
        public string pdfbarcodeUrl { get; set; }

        public string reprintbarcode { get; set; }

        public string letter_number { get; set; }

        public int type_report { get; set; }

        public string? qrcodenumber { get; set; }
    }

    #region kurir non ekspedisi
    public class OutputletterKurirNonEoffice
    {
        public Guid idmailingnoneoffice { get; set; }
        public string? letter_number { get; set; }

        public int delivery_type { get; set; }

        public int type_report { get; set; }

        public DateTime ReceiptDate { get; set; }

        public string? expedition_name { get; set; }

        public string? sender_name { get; set; }

        public string? nip { get; set; }
        public string? unitname { get; set; }

        public string? qrcodenumber { get; set; }

        public string? docReceiver { get; set; }

        public int statuskirim { get; set; }

        public string? nmrreferen { get; set; }

        public string address { get; set; }

        public int updtae { get; set; }

        public int qrcode { get; set; }

        public string cretaeby { get; set; }

        public string updateby { get; set; }

        public string? purposename { get; set; }

        public string? phonenumber { get; set; }

        public DateTime? DateUntil { get; set; }

        public string tgluntil { get; set; }
    }


    public class ParamGetDetailKurirNonEOffice
    {
        public Guid idDelivery { get; set; }

    }

    public class UpdateKurirNonEoffice
    {
        public string? letter_number { get; set; }
        public Guid idmailingnoneoffice { get; set; }

        public string? docReceiver { get; set; }

        public DateTime? startDate { get; set; }

        public int statuskirim { get; set; }

    }

    #endregion

    //Laporan Surat Keluar Ekspedisi dan Kurir

    public class ReportOutgoingMailOutput
    {
        public Guid idmailingnoneoffice { get; set; }
        public string? letter_number { get; set; }

        public int delivery_type { get; set; }

        public string deliveryname { get; set; }

        public DateTime ReceiptDate { get; set; }

        public string? expedition_name { get; set; }

        public string? sender_name { get; set; }

        public string? unitname { get; set; }

        public string? docReceiver { get; set; }

        public int statuskirim { get; set; }

        public string statusname { get; set; }

        public string nmrreferen { get; set; }

        public string address { get; set; }
    }

    public class ParamReportOutgoingMailEksnKurir
    {
        public string trackingNumber { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }


    public class getByIdNotifikasiLainnya
    {
 
        public Guid ID_NOTIFIKASI { get; set; }
    }



}

using EofficeBNILWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EofficeBNILWEB.DataAccess
{
    public interface IDataAccessProvider
    {
        Task<GeneralOutputModel> GetDataMenuAsync(string param, string accessUrl, string token);
        Task<GeneralOutputModel> LoginAsync(LoginParam pr, string accessUrl);
        Task<GeneralOutputModel> GetUnitAsync(string accessUrl,string token);
        Task<GeneralOutputModel> GetPositionAsync(string accessUrl,string token);
        Task<GeneralOutputModel> GetDocumentAsync(string accessUrl, string token, ParamGetDocumentWeb pr);
        Task<GeneralOutputModel> GetDocumentByTrackingNumberAsync(string accessUrl, string token, ParamCheckTrackingNumber pr);
        Task<GeneralOutputModel> GetDetailDocumentAsync(string accessUrl, string token, ParamGetDetailDocument pr);
        Task<GeneralOutputModel> GetStringmapAsync(string accessUrl, ParamGetStringmap pr, string token);
        Task<GeneralOutputModel> PostInsertDocumentAsync(string accessUrl, ParamInsertDocument pr, string token);
        Task<GeneralOutputModel> PutUpdateDocumentAsync(string accessUrl, string token, ParamUpdateDocument pr);
        Task<GeneralOutputModel> PutReceiveDocumentAsync(string accessUrl, string token, ParamReceiveDocument pr);
        Task<GeneralOutputModel> PostInsertGenerateNoDoc(string accessUrl, ParamInsertGenerateNoDoc pr ,string token);
        Task<GeneralOutputModel> PostUploadDocumentAsync(string accessUrl, string token, ParamUploadDocumentString pr);
        Task<GeneralOutputModel> GetDataPenerima(string accessUrl, string token, ParamGetPenerima pr);
        Task<GeneralOutputModel> GetDataUserAsync(string accessUrl, string token, ParamGetUsertWeb pr);
        Task<GeneralOutputModel> InsertAttachmentLetter(string accessUrl, string token, ParamInsertAttachment pr);
        Task<GeneralOutputModel> InsertLetter(string accessUrl, string token, ParamInsertLetter pr);
        Task<GeneralOutputModel> GetDocumentReport(string accessUrl, string token, ParamGetDetailReportDocument pr);

        Task<GeneralOutputModel> GetDetailUserAsync(string accessUrl, string token, ParamGetDetailUser pr);
        Task<GeneralOutputModel> PutUpdateUserAsync(string accessUrl, string token, ParamUpdateUser pr);


        Task<GeneralOutputModel> GetLetterDraft(string accessUrl, string token, ParamGetLetterWeb pr);
        Task<GeneralOutputModel> GetInboxLetter(string accessUrl, string token, ParamGetLetterWeb pr);
        Task<GeneralOutputModel> DeleteLetter(string accessUrl, string token, ParamDeleteLetter pr);
        Task<GeneralOutputModel> ResetPassword(string accessUrl, string token, ParamUpdatePassword pr);
        Task<GeneralOutputModel> GetDetailLetter(string accessUrl, string token, ParamGetDetailLetter pr);
        Task<GeneralOutputModel> DeleteAttachment(string accessUrl, string token, ParamDeleteAttachment pr);
        Task<GeneralOutputModel> ReceiveCheckedDoc(string accessUrl, string token, ParamReceiveCheckedDoc pr);
        Task<GeneralOutputModel> GetDetailDocumentReceiverAsync(string accessUrl, string token, ParamGetDetailDocumentReceiver pr);
        Task<GeneralOutputModel> GetUserByUnitAsync(string accessUrl, string token, ParamGetUserByUnit pr);
        Task<GeneralOutputModel> UpdateAdminDivisi(string accessUrl, string token, ParamUpdateAdminDivisi pr);
        Task<GeneralOutputModel> PreviewNoLetter(string accessUrl, string token, ParamPreviewNoLetter pr);
        Task<GeneralOutputModel> GetDocumentSearchOutgongMail(string accessUrl, string token, ParamGetDetailSearchoutgoingDocument pr);
        Task<GeneralOutputModel> MailForgotPassword(string accessUrl, ParamForgotPassword pr);
        Task<GeneralOutputModel> GetDashboardContent(string accessUrl, string token);
        Task<GeneralOutputModel> PostValidasiBarcode(string accessUrl, string token, DataOuputValidasiBarcode pr);
        Task<GeneralOutputModel> PostInsertDisposition(string accessUrl, string token, ParamInsertLetterDisposition pr);
        Task<GeneralOutputModel> PostForgotPassword(string accessUrl, ParamUpdateForgotPassword pr);
        Task<GeneralOutputModel> UpdatePasswordLogin(string accessUrl, ParamUpdatePasswordLogin pr);
        
        Task<GeneralOutputModel> GetUserPGAasync(string accessUrl, string token);
        Task<GeneralOutputModel> GetUserDirekturasync(string accessUrl, string token);

        Task<GeneralOutputModel> GetUserSeketarisasync(string accessUrl, string token);
        Task<GeneralOutputModel> UpdateDataSettingSeketaris(string accessUrl, string token, ParamUpdateUserSekdirWeb pr);
        Task<GeneralOutputModel> GetUserByDirekturAsync(string accessUrl, string token, ParamUpdateUserSekdirWeb pr);
      
        Task<GeneralOutputModel> GetDataUserSekdirAsync(string accessUrl, string token);

        Task<GeneralOutputModel> PutUpdateUserSekDirAsync(string accessUrl, string token, ParamUpdateUserSekdirWeb pr);

        Task<GeneralOutputModel> PutDeleteUserSekdirAsync(string accessUrl, string token, ParamGetDetailUserSekdir pr);

        Task<GeneralOutputModel> GetUserSuperasync(string accessUrl, string token);

        Task<GeneralOutputModel> GetDataSuperUserAsync(string accessUrl, string token, ParamGetUsertWeb pr);

        Task<GeneralOutputModel> PutUpdateSuperUserAsync(string accessUrl, string token, ParamGetDetailUser pr);

        Task<GeneralOutputModel> PutDeleteSuperUserAsync(string accessUrl, string token, ParamGetDetailUser pr);

       
        Task<GeneralOutputModel> GetUserAdminHctsync(string accessUrl, string token);

        Task<GeneralOutputModel> GetDataAdminHctAsync(string accessUrl, string token, ParamGetUsertWeb pr);

        Task<GeneralOutputModel> PutUpdateAdminHctAsync(string accessUrl, string token, ParamGetDetailUser pr);

        Task<GeneralOutputModel> PutDeleteAdminHctAsync(string accessUrl, string token, ParamGetDetailUser pr);
     
        Task<GeneralOutputModel> GetEmployeesync(string accessUrl, string token);

        Task<GeneralOutputModel> GetAllDataposition(string accessUrl, string token);

        Task<GeneralOutputModel> SearchEmployeeGetById(string accessUrl, string token, ParamGetDetailUser pr);

        Task<GeneralOutputModel> PutUpdatePositionEmpAsync(string accessUrl, string token, ParamUpdateUser pr);

      

        Task<GeneralOutputModel> GetSuratKeluarNon(string accessUrl, string token);
        Task<GeneralOutputModel> PostInsertSendNonEofficeAsync(string accessUrl, ParamInsertNonEoffice pr, string token);

        Task<GeneralOutputModel> PostUpdateNonEofficeAsync(string accessUrl, UpdateNonEoffice pr, string token);

        Task<GeneralOutputModel> PostUploadLetterNonEoffcieAsync(string accessUrl, string token, ParamUploadLetterNonOfficeString pr);

        Task<GeneralOutputModel> GetStringmapNonEofficeAsync(string accessUrl, ParamGetStringmapNonEoffice pr, string token);

        Task<GeneralOutputModel> GetDataEksepdisiNonEoffice(string accessUrl, string token, ParamGetEkspedisi pr);
        Task<GeneralOutputModel> GetDetailsDataEkspediNonEoffice(string accessUrl, getByIdNonEoffice pr, string token);

        Task<GeneralOutputModel> GetDetailsDataKurirNonEoffice(string accessUrl, getByIdNonEoffice pr, string token);

        Task<GeneralOutputModel> GetDataKurirNameNonEoffice(string accessUrl, string token, ParamGetKurir pr);
        Task<GeneralOutputModel> GeDataByIdNonEoffice(string accessUrl, string token, getByIdNonEoffice pr);

        Task<GeneralOutputModel> PostInsertGenerateNoDocNonEoffice(string accessUrl, ParamInsertGenerateNoDocNonEoffice pr, string token);
        Task<GeneralOutputModel> InsertLevelEmpl(string accessUrl, string token, ParamUpdateLevelemp pr);

        Task<GeneralOutputModel> UpdateLevelEMp(string accessUrl, string token, ParamUpdateLevelemp pr);

        Task<GeneralOutputModel> GetUnitHctAsync(string accessUrl, string token);

        Task<GeneralOutputModel> GetContentTemplate(string accessUrl, string token);
        Task<GeneralOutputModel> AddContentTemplate(string accessUrl, string token, ParamInsertContentTemplate pr);
        Task<GeneralOutputModel> AddContentTemplateBulk(string accessUrl, string token, ParamInsertTemplateBulk pr);
        Task<GeneralOutputModel> UpdateContentTemplate(string accessUrl, string token, ParamUpdateContentTemplate pr);

        Task<GeneralOutputModel> GetDataReportNonEoffice(string accessUrl, string token, ParamReportNonOuboxLetter pr);

        Task<GeneralOutputModel> InsertLetterOutbox(string accessUrl, string token, ParamInsertLetterOutbox pr);

        Task<GeneralOutputModel> ApprovalLetter(string accessUrl, string token, ParamApprovalLetter pr);
        Task<GeneralOutputModel> GetDataReportOutgoingMailEkspedisinKurir(string accessUrl, string token, ParamReportOutgoingMailEksnKurir pr);

        Task<GeneralOutputModel> PutUploadSignatureAsync(string accessUrl, string token, ImageUploadTTD pr);

        Task<GeneralOutputModel> GetOuputSignatureUser(string accessUrl, string token);

        Task<GeneralOutputModel> PutApprovalSigantureOneData(string accessUrl, string token, ParamGetApprovalSignatureOnedata pr);
        Task<GeneralOutputModel> PutRejectSigantureOneData(string accessUrl, string token, ParamGetApprovalSignatureOnedata pr);
        Task<GeneralOutputModel> PutApprovalUserSigantureAsync(string accessUrl, string token, ParamJsonStirngSiganture pr);
       
        Task<GeneralOutputModel> PutRejectUserSigantureAsync(string accessUrl, string token, ParamJsonStirngSiganture pr);

        #region Kurir Non eoffice
        Task<GeneralOutputModel> GetSuratKeluarKurirNon(string accessUrl, string token);
        Task<GeneralOutputModel> PostUpdateKurirNonEofficeAsync(string accessUrl, UpdateKurirNonEoffice pr, string token);

        #endregion


        Task<GeneralOutputModel> DeleterSignatureUserProfile (string accessUrl, string token, OuputSignature pr);
        Task<GeneralOutputModel> DetailSignatureUserProfile(string accessUrl, string token, OuputSignature pr);

        Task<GeneralOutputModel> DetailApprovalSignatureUserProfile(string accessUrl, string token, OuputSignature pr);

        Task<GeneralOutputModel> PostUpdateUploadLetterNonEoffcieAsync(string accessUrl, string token, ParamUploadLetterNonOfficeString pr);

        Task<GeneralOutputModel> DetailLetterNonEofficeNotifikasi(string accessUrl, string token, getByIdNonEoffice pr);

        Task<GeneralOutputModel> GetDetailsViewEkspedisi_(string accessUrl, string token, ParamGetDetailsView pr);

        Task<GeneralOutputModel> GetDetailsViewKurir_(string accessUrl, string token, ParamGetDetailsView pr);
        Task<GeneralOutputModel> InsertDelivery(string accessUrl, string token, ParamInsertDelivery pr);

        Task<GeneralOutputModel> GetDropDownUserBod(string accessUrl, string token);

        Task<GeneralOutputModel> GetViewDataUserBod(string accessUrl, string token, ParamGetUsertWeb pr);

        Task<GeneralOutputModel> CreatSettingBodAsync(string accessUrl, string token, ParamGetDetailUser pr);

        Task<GeneralOutputModel> DeleteDataBodAsync(string accessUrl, string token, ParamGetDetailUser pr);

        Task<GeneralOutputModel> GenerateQrCodeDeliveryEoffice(string accessUrl, string token, ParamGenerateDeliveryNumber pr);
        Task<GeneralOutputModel> GetDetailDeliveryEoffice(string accessUrl, string token, ParamGetDetailDelivery pr);
        Task<GeneralOutputModel> UpdateDeliveryEoffice(string accessUrl, string token, ParamUpdateDelivery pr);
        Task<GeneralOutputModel> GetReportDeliveryEoffice(string accessUrl, string token, ParamGetReportEkspedisiEoffice pr);
        Task<GeneralOutputModel> PostUploadExpeditionEoffice(string accessUrl, string token, ParamUploadEkspedisiEofficeString pr);

		Task<GeneralOutputModel> GetSkDelivery(string accessUrl, string token, ParamGetSKDelivery pr);
        #region
        Task<GeneralOutputModel> InsertAttachmentMemo(string accessUrl, string token, ParamInsertAttachmentMemo pr);

        Task<GeneralOutputModel> InsertLetterMemo(string accessUrl, string token, ParamInsertMemo pr);

        Task<GeneralOutputModel> InsertLetterMemoBackdate(string accessUrl, string token, ParamInsertMemoBackdate pr);


        Task<GeneralOutputModel> GetMemoDistribusi(string accessUrl, string token, ParamGetMemoWeb pr);

        Task<GeneralOutputModel> GetDetailMemo(string accessUrl, string token, ParamGetDetailMemo pr);


        Task<GeneralOutputModel> ApprovalMemo(string accessUrl, string token, ParamApprovalLetterMemo pr);


        Task<GeneralOutputModel> DeleteAttachmentMemo(string accessUrl, string token, ParamDeleteAttachmentMemo pr);

        Task<GeneralOutputModel> GetDetailMemoBackDate(string accessUrl, string token, ParamGetDetailMemo pr);

        #endregion


        Task<GeneralOutputModel> GetMemotypeByIdAsync(string accessUrl, string token, ParamGetMemoTypeById pr);

        Task<GeneralOutputModel> ApprovalDelebrationMemo(string accessUrl, string token, ParamApprovalLetterMemo pr);


        #region Pengadaan

        Task<GeneralOutputModel> GetDataPengadaan(string accessUrl, string token);
        Task<GeneralOutputModel> GetPengadaanByIdAsync(string accessUrl, string token, ParamGetMemoPengadaanById pr);

        Task<GeneralOutputModel> GetDataMinMaxNomialByIdAsync(string accessUrl, string token, ParamCheckMinMaxNomial pr);

        Task<GeneralOutputModel> InsertMemoPengadaan(string accessUrl, string token, ParamInsertMemoPengadaan pr);
        

        Task<GeneralOutputModel> GetDetailMemoPengadaan(string accessUrl, string token, ParamGetDetailMemo pr);
        #endregion




        Task<GeneralOutputModel> GetUserAdminPengadaansync(string accessUrl, string token);

        Task<GeneralOutputModel> GetDataAdminPengadaanAsync(string accessUrl, string token, ParamGetUsertWeb pr);

        Task<GeneralOutputModel> PutUpdateAdminPengadaanAsync(string accessUrl, string token, ParamGetDetailUser pr);

        Task<GeneralOutputModel> PutDeleteAdminPengadaanAsync(string accessUrl, string token, ParamGetDetailUser pr);

        Task<GeneralOutputModel> InsertDataPengadaan(string accessUrl, string token, ParamInsertPengadaan pr);
        Task<GeneralOutputModel> GetUserSetPengadaansync(string accessUrl, string token);
        Task<GeneralOutputModel> GetDataSetPengadaanAsync(string accessUrl, string token, ParamGetUsertWeb pr);

        Task<GeneralOutputModel> GetDataSetDelegasiAsync(string accessUrl, string token, ParamGetUsertWeb pr);

        Task<GeneralOutputModel> GetUserSetDelegasisync(string accessUrl, string token);

        Task<GeneralOutputModel> InsertDataDelegasi(string accessUrl, string token, ParamInsertDelegasi pr);

        Task<GeneralOutputModel> GetDataPengadaansync(string accessUrl, string token);
        Task<GeneralOutputModel> GetDataPengadaanApprovalsync(string accessUrl, string token);

        Task<GeneralOutputModel> PutApprovalPengadaanAsync(string accessUrl, string token, ParamJsonStirngPengadaan pr);

        Task<GeneralOutputModel> GetDataPengadaanModalsync(string accessUrl, string token, ParamGetPengadaanModal pr);
        Task<GeneralOutputModel> GetDataDelegasisync(string accessUrl, string token);
        Task<GeneralOutputModel> GetDataDelegasiApprovalsync(string accessUrl, string token);

        Task<GeneralOutputModel> PutApprovalDelegasiAsync(string accessUrl, string token, ParamJsonStirngDelegasi pr);

        Task<GeneralOutputModel> NotifikasiLainnya(string accessUrl, string token, getByIdNotifikasiLainnya pr);

        Task<GeneralOutputModel> GetLetterDraftMemoSrch(string accessUrl, string token, ParamGetMemoWeb pr);

        Task<GeneralOutputModel> InsertLetterOutboxBackdate(string accessUrl, string token, ParamInsertLetterOutboxBackdate pr);

        Task<GeneralOutputModel> UpdateStatusNotifikasi(string accessUrl, string token, ParamUpdateNotif pr);

        Task<GeneralOutputModel> PushNotifikasi(string accessUrl, string token, ParamPushNotifikasi pr);

        Task<GeneralOutputModel> GetSeacrhoutGoingEoffice(string accessUrl, string token, ParamGetReportSeacrhoutGoing pr);

    }
}

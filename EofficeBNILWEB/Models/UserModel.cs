namespace EofficeBNILWEB.Models
{
    public class DataOuputUser
    {
        public Guid Iduser { get; set; }
        public string Nip { get; set; }
        public string Fullname { get; set; }

        public string Email { get; set; }

        public Int64 Phone { get; set; }

        public int Status { get; set; }

        public string IDgroup { get; set; }

        public Guid IdPosition { get; set; }
        public string Position_name { get; set; }

        public string Unit_name { get; set; }


    }
    public class ParamUpdateUserWeb
    {
        public Guid Iduser { get; set; }

        public string IDgroup { get; set; }


    }

    public class ParamUpdateUser
    {
        public Guid Iduser { get; set; }

        
        public string IDgroup { get; set; }

        public Guid idposition { get; set; }

    }

    public class ParamUpdateDataUserWeb
    {
        public Guid Iduser { get; set; }

        public string Fullname { get; set; }

        public string Email { get; set; }

        public decimal Phone { get; set; }

        public int Status { get; set; }

        public string IDgroup { get; set; }

        public Guid Iposition { get; set; }

        public string Position_name { get; set; }
    }

    public class ParamGetDetailUser
    {
        public Guid Iduser { get; set; }
        public Guid Idunit { get; set; }
        public Guid IdBod { get; set; }
    }

    public class UserOutputWeb
    {
        public string draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<DataOuputUser> data { get; set; }

    }
    public class ParamGetUsertWeb
    {
        public string draw { get; set; }
        public int start { get; set; }
        public string sortColumn { get; set; }
        public string sortColumnDirection { get; set; }
        public string? searchValue { get; set; }
        public int pageSize { get; set; }

    }

    public class ParamUpdatePassword
    {
        public string passLama { get; set; }
        public string newPassword { get; set; }
    }
    public class ParamGetUserByUnit
    {
        public Guid idUnit { get; set; }

    }
    public class ParamUpdateAdminDivisi
    {
        public Guid idUnit { get; set; }
        public Guid idUser { get; set; }

    }
    public class ParamForgotPassword
    {
        public string token { get; set; }


        public string email { get; set; }
    }
    public class ParamUpdateForgotPassword
    {
        public Guid recoveryToken { get; set; }
        public string newPassword { get; set; }
    }

    public class ParamUpdatePasswordLogin
    {
       
        public string nip { get; set; }
        public string newPassword { get; set; }
    }

    public class DataOuputUserPGA
    {
        public Guid Iduser { get; set; }
        public string Nip { get; set; }
        public string Fullname { get; set; }

        public string IDgroup { get; set; }
    }

    public class DataOuputUserDirektur
    {
        public Guid IdDirektur { get; set; }
        public Guid idPositionDirektur { get; set; }
        public Guid idUnit { get; set; }
        public string Nip { get; set; }
        public string Fullname { get; set; }
        public string Position_Name { get; set; }
        public string unitName { get; set; }


    }

    public class DataOuputUserSeketaris
    {
        public Guid Iduser { get; set; }
        public Guid idPositionSeketaris { get; set; }
        public Guid idUnit { get; set; }
        public string Nip { get; set; }
        public string Fullname { get; set; }
        public string Position_Name { get; set; }
        public string unitName { get; set; }

    }

    //public class UserSekdirOutputWeb
    //{
    //    public string draw { get; set; }
    //    public int recordsTotal { get; set; }
    //    public int recordsFiltered { get; set; }
    //    public List<DataOuputSekdir> data { get; set; }

    //}

    public class DataOuputSekdir
    {
        public Guid IduserSeketaris { get; set; }
        public Guid idPositionDirektur { get; set; }
        public Guid idPositionSeketaris { get; set; }
        public string FullnameDirektur { get; set; }
        public string Position_NameDirektur { get; set; }
        public string FullnameSeketaris { get; set; }
        public string Position_NameSeketaris { get; set; }

        public Guid ID_SETDIRKOM { get; set; }
    }

    public class ParamUpdateUserSekdirWeb
    {
        public Guid Iduser { get; set; }
        public Guid IdDirektur { get; set; }
        public Guid idPositionDirektur { get; set; }


    }

    public class ParamGetDetailUserSekdir
    {
        public Guid IduserSeketaris { get; set; }
        public Guid ID_SETDIRKOM { get; set; }
    }


    public class DataOuputSuperUser
    {
        public Guid Iduser { get; set; }
        public string Nip { get; set; }
        public string Fullname { get; set; }
        public string Position_Name { get; set; }
        public string unitName { get; set; }

    }


    public class SuperUserOutputWeb
    {
        public string draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<DataOuputSuperUserWeb> data { get; set; }

    }

    public class DataOuputSuperUserWeb
    {
        public Guid IdBod { get; set; }
        public Guid Iduser { get; set; }
        public Guid Idunit { get; set; }
        public string Nip { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public Int64 Phone { get; set; }
        public int Status { get; set; }
        public string IDgroup { get; set; }
        public Guid IdPosition { get; set; }
        public string Position_name { get; set; }
        public string Unit_name { get; set; }
        public string Unit_code { get; set; }
        public string password { get; set; }
    }


    public class DataOuputUserHctWeb
    {
        public Guid Iduser { get; set; }
        public string Nip { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public Int64 Phone { get; set; }
        public int Status { get; set; }
        public string IDgroup { get; set; }
        public Guid IdPosition { get; set; }
        public string Position_name { get; set; }
        public string Unit_name { get; set; }
        public string password { get; set; }
    }

    public class AdminHCTOutputWeb
    {
        public string draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<DataOuputAdminHCT> data { get; set; }

    }

    public class DataOuputAdminHCT
    {
        public Guid Iduser { get; set; }
        public string Nip { get; set; }
        public string Fullname { get; set; }
        public string Position_Name { get; set; }
        public string unitName { get; set; }

    }


    public class ParamUpdateLevelemp
    {
        public Guid idemplevel { get; set; }
        public Guid idUnit { get; set; }
        public Guid idUser { get; set; }
        public int idLevel { get; set; }

        public int statuscode { get; set; }

    }
    public class DataOuputPosition
    {
        public Guid idPosition { get; set; }
        public string Position_Name { get; set; }
    }

    public class OuputLevelemp
    {
        public Guid idemplevel { get; set; }
        public Guid idUnit { get; set; }
        public string unitName { get; set; }
        public Guid idUser { get; set; }
        public string userName { get; set; }

        public string levelName { get; set; }

    }

    public class ImageUploadTTD
    {
        public Guid? idUser { get; set; }
        public string? NameImage { get; set; }
        public string? TypeImage { get; set; }
        public int? LenghtImage { get; set; }
        //public IFormFile FileName { get; set; }
    }
    public class OutputDetailApprovalSignature
    {
        public OuputSignature Signature { get; set; }
    }
    public class OuputSignature
    {
        public Guid idMg { get; set; }
        public Guid idUser { get; set; }
        public string? nip { get; set; }
        public string? fullname { get; set; }
        public string? PositionName { get; set; }
        public string? idgroup { get; set; }
        public string? UnitName { get; set; }
        public string? NameImage { get; set; }
        public string? TypeImage { get; set; }
        public int? LenghtImage { get; set; }
        public int? status_code { get; set; }
    }

    public class ParamJsonStirngSiganture
    {
        public List<ParamGetApprovalSignature> jsonDataString { get; set; }
    }
    public class ParamGetApprovalSignature
    {
        public Guid idMg { get; set; }
        public bool isChecked { get; set; }
    }

    public class ParamGetApprovalSignatureOnedata
    {
        public Guid idMg { get; set; }
    }

 
    public class AdminPengadaanOutputWeb
    {
        public string draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<DataOuputAdminHCT> data { get; set; }

    }

    public class DataOuputAdminPengadaan
    {
        public Guid Iduser { get; set; }
        public string Nip { get; set; }
        public string Fullname { get; set; }
        public string Position_Name { get; set; }
        public string unitName { get; set; }

    }
    public class ParamInsertPengadaan
    {
        public Guid Id { get; set; }

        public string name { get; set; }
        public string min_nominal { get; set; }
        public string max_nominal { get; set; }
        public string approver1 { get; set; }
        public string approver2
        {
            get; set;

        }
    }


    public class DataOuputSetPengadaan
    {
        public Guid Iduser { get; set; }
        public Guid Id { get; set; }
        public string Nip { get; set; }
        public string Fullname { get; set; }
        public string Position_Name { get; set; }
        public string unitName { get; set; }
        public string NAME_PENGADAAN { get; set; }
        public string APPROVER_NAME { get; set; }
        public string APPROVER2_NAME { get; set; }
        public string POSITION1 { get; set; }
        public string POSITION2 { get; set; }
        public string JABATAN1 { get; set; }
        public string JABATAN2 { get; set; }
        public string MIN_NOMINAL { get; set; }
        public string MAX_NOMINAL { get; set; }
        public string APPROVER { get; set; }
        public string APPROVER2 { get; set; }

        public int STATUS_APPROVAL { get; set; }
        public string STATUS { get; set; }



    }


    public class SetPengadaanOutputWeb
    {
        public string draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<DataOuputSetPengadaan> data { get; set; }

    }


    public class DataOuputSetDelegasi
    {
        public Guid Iduser { get; set; }
        public Guid Id { get; set; }
        public string Nip { get; set; }
        public string Fullname { get; set; }
        public string Position_Name { get; set; }
        public string unitName { get; set; }
        public string USER_NAME { get; set; }
        public string user_name_delegasi { get; set; }
        public string POSITION { get; set; }
        public string POSITION_DELEGASI { get; set; }
        public string JABATAN { get; set; }
        public string JABATAN_DELEGASI { get; set; }
        public DateTime STARTDATE { get; set; }
        public DateTime ENDDATE { get; set; }
        public string startdatestring { get; set; }
        public string enddatestring { get; set; }
        public int STATUS_APPROVER { get; set; }
        public string STATUS { get; set; }


    }

    public class SetDelegasiOutputWeb
    {
        public string draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<DataOuputSetDelegasi> data { get; set; }

    }

    public class DataOuputSetDelegasiTable
    {

        public Guid Id { get; set; }
        public Guid id_user { get; set; }
        public Guid id_user_delegasi { get; set; }
        public Guid id_user_approved { get; set; }
        public DateTime? startdate { get; set; }
        public string startdatestring { get; set; }

        public DateTime? enddate { get; set; }
        public string enddatestring { get; set; }
        public string user_name { get; set; }
        public string user_name_delegasi { get; set; }
        public string user_name_approved { get; set; }
        public string POSITION { get; set; }
        public string POSITION_DELEGASI { get; set; }
        public string POSITION_APPROVED { get; set; }

        public string JABATAN { get; set; }
        public string JABATAN_DELEGASI { get; set; }
        public string JABATAN_APPROVED { get; set; }
        public string Status_approver { get; set; }

        public string Approved_by { get; set; }


    }

    public class ParamInsertDelegasi
    {
        public Guid Id { get; set; }

        public Guid id_user { get; set; }
        public Guid id_user_delegasi { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int status_approver { get; set; }

    }


    public class DataOuputSetPengadaanApproval
    {
        public Guid Iduser { get; set; }
        public Guid Id { get; set; }
        public string Nip { get; set; }
        public string Fullname { get; set; }
        public string Position_Name { get; set; }
        public string unitName { get; set; }
        public string NAME_PENGADAAN { get; set; }
        public string APPROVER_NAME { get; set; }
        public string APPROVER2_NAME { get; set; }
        public string POSITION1 { get; set; }
        public string POSITION2 { get; set; }
        public string JABATAN1 { get; set; }
        public string JABATAN2 { get; set; }
        public string MIN_NOMINAL { get; set; }
        public string MAX_NOMINAL { get; set; }
        public string APPROVER { get; set; }
        public string APPROVER2 { get; set; }

        public int STATUS_APPROVAL { get; set; }
        public string STATUS { get; set; }

    }


    public class ParamGetApprovalPengadaan
    {
        public Guid Id { get; set; }
        public bool isChecked { get; set; }
    }

    public class ParamJsonStirngPengadaan
    {
        public List<ParamGetApprovalPengadaan> jsonDataString { get; set; }
    }

    public class ParamGetPengadaanModal
    {
        public Guid Id { get; set; }

    }

    public class DataOuputSetDelegasiApproval
    {
        public Guid Id { get; set; }
        public Guid id_user { get; set; }
        public Guid id_user_delegasi { get; set; }
        public Guid id_user_approved { get; set; }
        public DateTime? startdate { get; set; }
        public string startdatestring { get; set; }

        public DateTime? enddate { get; set; }
        public string enddatestring { get; set; }
        public string user_name { get; set; }
        public string user_name_delegasi { get; set; }
        public string user_name_approved { get; set; }
        public string POSITION { get; set; }
        public string POSITION_DELEGASI { get; set; }
        public string POSITION_APPROVED { get; set; }

        public string JABATAN { get; set; }
        public string JABATAN_DELEGASI { get; set; }
        public string JABATAN_APPROVED { get; set; }
        public string Status_approver { get; set; }

        public string Approved_by { get; set; }



    }

    public class ParamGetApprovalDelegasi
    {
        public Guid Id { get; set; }
        public bool isChecked { get; set; }
    }

    public class ParamJsonStirngDelegasi
    {
        public List<ParamGetApprovalDelegasi> jsonDataString { get; set; }
    }



}

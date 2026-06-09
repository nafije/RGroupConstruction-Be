namespace RGroupConstruction.Application.Resources;

public static class MessageKeys
{
    public static class Result
    {
        public const string ValidationFailed = "Result_ValidationFailed";
        public const string UnexpectedError = "Result_UnexpectedError";
        public const string Unauthorized = "Result_Unauthorized";
        public const string NotFound = "Result_NotFound";
        public const string Conflict = "Result_Conflict";
        public const string Forbidden = "Result_Forbidden";
    }
    public static class Validation
    {

        public const string LoginRequired = "Validation_LoginRequired";
        public const string PasswordRequired = "Validation_PasswordRequired";
        public const string PasswordMinLength6 = "Validation_PasswordMinLength6"; 
        public const string EmailRequired = "Validation_EmailRequired";
        public const string EmailInvalid = "Validation_EmailInvalid";
        public const string TokenRequired = "Validation_TokenRequired";
        public const string TitleMaxLength200 = "Validation_TitleMaxLength200";
        public const string TitleRequired = "Validation_TitleRequired";
        public const string LanguageCodeRequired = "Validation_LanguageCodeRequired";
        public const string LanguageCodeSupported = "Validation_LanguageCodeSupported";
        public const string IdRequired = "Validation_IdRequired";
        public const string FullNameRequired = "Validation_FullNameRequired";
        public const string NameMaxLength50 = "Validation_NameMaxLength50";
        public const string AddressRequired = "Validation_AddressRequired";
        public const string AtLeastOneTranslationRequired = "Validation_AtLeastOneTranslationRequired";
        public const string TranslationNameRequired = "Validation_TranslationNameRequired";
        public const string PhoneNumberRequired = "Validation_PhoneNumberRequired";
        public const string PhoneNumberInvalidInternational = "Validation_PhoneNumberInvalidInternational";
        public const string NameMaxLength200 = "Validation_NameMaxLength200";
    }

    
    public static class Auth
    {
        public const string NotFound = "Auth_NotFound";
        public const string AlreadyExists = "Auth_AlreadyExists";
        public const string InvalidOperation = "Auth_InvalidOperation";
        public const string Unauthorized = "Auth_Unauthorized";
        public const string Forbidden = "Auth_Forbidden";
        public const string ValidationFailed = "Auth_ValidationFailed";
        public const string ConcurrentUpdate = "Auth_ConcurrentUpdate";
        public const string ExternalServiceError = "Auth_ExternalServiceError";
        public const string InvalidArgument = "Auth_InvalidArgument";
        public const string DatabaseError = "Auth_DatabaseError";
        public const string UserNotFound = "Auth_UserNotFound";
        public const string InvalidCredentials = "Auth_InvalidCredentials";

        public const string InvalidToken = "Auth_InvalidToken";
        public const string TokenExpired = "Auth_TokenExpired";
        public const string RefreshTokenInvalid = "Auth_RefreshTokenInvalid";
        public const string RefreshTokenExpired = "Auth_RefreshTokenExpired";
        public const string PasswordResetTokenInvalid = "Auth_PasswordResetTokenInvalid";
        public const string PasswordResetTokenExpired = "Auth_PasswordResetTokenExpired";
        public const string EmailNotConfirmed = "Auth_EmailNotConfirmed";
        public const string EmailAlreadyConfirmed = "Auth_EmailAlreadyConfirmed";
        public const string AccountLocked = "Auth_AccountLocked";
        public const string AccountSuspended = "Auth_AccountSuspended";
        public const string TokenBlacklisted = "Auth_TokenBlacklisted";
    }

    public static class Api
    {
        public const string UserNotAuthenticated = "Api_UserNotAuthenticated";
        public const string InvalidRequest = "Api_InvalidRequest";
        public const string Unauthorized = "Api_Unauthorized";
        public const string UnauthorizedDetail = "Api_UnauthorizedDetail";
        public const string UnauthorizedAccessResource = "Api_UnauthorizedAccessResource";
        public const string ResourceNotFound = "Api_ResourceNotFound";
        public const string RequestTimeout = "Api_RequestTimeout";
        public const string RequestTimeoutDetail = "Api_RequestTimeoutDetail";
        public const string InternalServerError = "Api_InternalServerError";
        public const string InternalServerErrorDetail = "Api_InternalServerErrorDetail";
    }

    public static class Exception
    {
        public const string JwtSecretKeyNotConfigured = "Exception_JwtSecretKeyNotConfigured";
        public const string HangfireConnectionRequired = "Exception_HangfireConnectionRequired";
        public const string ConnectionStringNotFound = "Exception_ConnectionStringNotFound";
        public const string ConnectionStringNotFoundPostgres = "Exception_ConnectionStringNotFoundPostgres";
        public const string InvalidToken = "Exception_InvalidToken";
        public const string UserIdNotFound = "Exception_UserIdNotFound";
    }

    public static class Project
    {
        public const string IdRequired = "Project_IdRequired";
        public const string NameRequired = "Project_NameRequired";
        public const string CityRequired = "Project_CityRequired";
        public const string ProjectTypeRequired = "Project_ProjectTypeRequired";
        public const string ProjectStatusRequired = "Project_ProjectStatusRequired";
        public const string TotalUnitsGraterThan0 = "Project_TotalUnitsGraterThan0";
    }

    public static class Unit
    {
        public const string IdRequired = "Unit_IdRequired"; 
        public const string ProjectIdRequired = "Unit_ProjectIdRequired";
        public const string CategoryIdRequired = "Unit_CategoryIdRequired";
        public const string RefNumberRequired = "Unit_RefNumberRequired";
        public const string StatusRequired = "Unit_StatusRequired";
    }

    public static class Layout
    {
        public const string IdRequired = "Layout_IdRequired";
        public const string NameRequired = "Layout_NameRequired";
    }

    public static class Status
    {
        public const string IdRequired = "Status_IdRequired";
        public const string NameRequired = "Status_NameRequired";
    }

    public static class Category
    {
        public const string IdRequired = "Category_IdRequired";
        public const string NameRequired = "Category_NameRequired";
    }
    
    public static class JobApplication
    {
        public const string JobIdRequired = "JobApplication_JobIdRequired";
        public const string FullNameRequired = "JobApplication_NameRequired";
        public const string PhoneRequired = "JobApplication_PhoneRequired";
    }
    public static class Department
    {
        public const string IdRequired = "Department_IdRequired";
        public const string NameRequired = "Department_NameRequired";
    }

    public static class Job
    {
        public const string IdRequired = "Career_IdRequired";
        public const string TiteRequired = "Career_TiteRequired";
        public const string DepartmentIdRequired = "Career_DepartmentIdRequired";
        public const string EmploymentTypeRequired = "Career_EmploymentTypeRequired";
    }


    public static class Success
    {
        public static class Notification
        {
            public const string MarkedRead = "Success_Notification_MarkedRead";
            public const string MarkedUnread = "Success_Notification_MarkedUnread";
            public const string AllMarkedRead = "Success_Notification_AllMarkedRead";
            public const string Deleted = "Success_Notification_Deleted";
        }
        public static class CompanyProfile
        {
            public const string Added = "Success_CompanyProfile_Added";
        }
        public static class Subscribe
        {
            public const string Created = "Success_Subscribe_Created";
            public const string Removed = "Success_Subscribe_Removed";
        }
        public static class Contact
        {
            public const string Created = "Success_Contact_Created";
        }

        public static class General
        {
            public const string OperationCompleted = "Success_General_OperationCompleted";
            public const string ResourceCreated = "Success_General_ResourceCreated";
            public const string ResourceUpdated = "Success_General_ResourceUpdated";
            public const string ResourceDeleted = "Success_General_ResourceDeleted";
        }
        public static class Project
        {
            public const string Created = "Success_Project_Created";
            public const string Updated = "Success_Project_Updated";
            public const string Deleted = "Success_Project_Deleted";
            public const string FeaturedUpdated = "Success_Project_FeaturedUpdated";
        }

        public static class Department
        {
            public const string Created = "Success_Department_Created";
            public const string Updated = "Success_Department_Updated";
            public const string Deleted = "Success_Department_Deleted";
        }
        public static class Job
        {
            public const string Created = "Career_Job_Created";
            public const string Updated = "Career_Job_Updated";
            public const string Deleted = "Career_Job_Deleted";
            public const string CareerStatusUpdated = "Success_Career_CareerStatusUpdated";
        }

        public static class JobApplication
        {
            public const string Created = "Career_JobApplication_Created";
        }

        public static class Layout
        {
            public const string Created = "Success_Layout_Created";
            public const string Updated = "Success_Layout_Updated";
            public const string Deleted = "Success_Layout_Deleted";
        }

        public static class Category
        {
            public const string Created = "Success_Category_Created";
            public const string Updated = "Success_Category_Updated";
            public const string Deleted = "Success_Category_Deleted";
        }

        public static class Unit
        {
            public const string Created = "Success_Unit_Created";
            public const string Updated = "Success_Unit_Updated";
            public const string Deleted = "Success_Unit_Deleted";
        }
        public static class Parking
        {
            public const string Created = "Success_Parking_Created";
            public const string Updated = "Success_Parking_Updated";
            public const string Deleted = "Success_Parking_Deleted";
        }
        public static class Ads
        {
            public const string Created = "Success_Ads_Created";
            public const string Updated = "Success_Ads_Updated";
            public const string Deleted = "Success_Ads_Deleted";
        }

        public static class Status
        {
            public const string Created = "Success_Status_Created";
            public const string Updated = "Success_Status_Updated";
            public const string Deleted = "Success_Status_Deleted";
        }

        public static class City
        {
            public const string Created = "Success_City_Created";
            public const string Updated = "Success_City_Updated";
            public const string Deleted = "Success_City_Deleted";
        }

        public static class UnitClient
        {
            public const string Created = "Success_UnitClient_Created";
            public const string Updated = "Success_UnitClient_Updated";
            public const string Deleted = "Success_UnitClient_Deleted";
        }
    }

    public static class Error
    {
        public static class CompanyInfo
        {
            public const string NotFound = "Error_CompanyInfo_NotFound";
        }

        public static class Project
        {

            public const string ProjectExists = "Error_Project_ProjectExists";
            public const string CreateFailed = "Error_Project_CreateFailed";
            public const string NotFound = "Error_Project_NotFound";
            public const string UpdateFailed = "Error_Project_UpdateFailed";
            public const string StatusNotFound = "Error_Project_StatusNotFound";
            public const string CityNotFound = "Error_Project_CityNotFound";
        }
        

        public static class Job
        {

            public const string JobExists = "Error_Jobt_JobExists";
            public const string CreateFailed = "Error_Job_CreateFailed";
            public const string NotFound = "Error_Job_NotFound";
            public const string DepartmentNotFound = "Error_Job_NotFound";
            public const string UpdateFailed = "Error_Job_UpdateFailed";
        }

        public static class JobApplication
        {

            public const string JobNotFound = "Error_JobApplication_JobNotFound";
            public const string CreateFailed = "Error_JobApplication_CreateFailed";
        }

        public static class Unit
        {
            public const string UnitExists = "Error_Unit_UnitExists";
            public const string CreateFailed = "Error_Unit_CreateFailed";
            public const string NotFound = "Error_Unit_NotFound";
            public const string UpdateFailed = "Error_Unit_UpdateFailed";
            public const string ProjectNotFound = "Error_Unit_ProjectNotFound";
            public const string CategoryNotFound = "Error_Unit_CategoryNotFound";
            public const string LayoutNotFound = "Error_Unit_LayoutNotFound";
        }

        public static class Parking
        {
            public const string ParkingExists = "Error_Unit_ParkingExists";
            public const string CreateFailed = "Error_Unit_CreateFailed";
            public const string NotFound = "Error_Unit_NotFound";
            public const string UpdateFailed = "Error_Unit_UpdateFailed";
            public const string ProjectNotFound = "Error_Parking_ProjectNotFound";
        }
        public static class Layout
        {
            public const string LayoutExists = "Error_Layout_LayoutExists";
            public const string CreateFailed = "Error_Layout_CreateFailed";
            public const string NotFound = "Error_Layout_NotFound";
            public const string UpdateFailed = "Error_Layout_UpdateFailed";
        }

        public static class Category
        {

            public const string CategoryExists = "Error_Category_CategoryExists";
            public const string CreateFailed = "Error_Category_CreateFailed";
            public const string NotFound = "Error_Category_NotFound";
            public const string UpdateFailed = "Error_Category_UpdateFailed";
        }

        public static class Email
        {
            public const string SendFailed = "Error_Email_SendFailed";
        }

        public static class Notification
        {
            public const string NotFound = "Error_Notification_NotFound";
            public const string NotFoundOrForbidden = "Error_Notification_NotFoundOrForbidden";
            public const string AdminFailed = "Error_Notification_AdminFailed";
        }

        public static class Ads
        {
            public const string NotFound = "Error_Ads_NotFound";
            public const string InvalidImageBase64 = "Error_Ads_InvalidImageBase64";
            public const string InvalidVideoBase64 = "Error_Ads_InvalidVideoBase64";
            public const string NotFoundById = "Error_Ads_NotFoundById";
        }

        public static class Subscribe
        {
            public const string AlreadySubscribed = "Error_Subscribe_AlreadySubscribed";
            public const string NotSubscribed = "Error_Subscribe_NotSubscribed";
            public const string InvalidToken = "Error_Subscribe_InvalidToken";
        }

        public static class Department
        {

            public const string DepartmentExists = "Error_Department_DepartmentExists";
            public const string CreateFailed = "Error_Department_CreateFailed";
            public const string NotFound = "Error_Department_NotFound";
            public const string UpdateFailed = "Error_Department_UpdateFailed";
        }

        public static class Status
        {
            public const string StatusExists = "Error_Status_StatusExists";
            public const string CreateFailed = "Error_Status_CreateFailed";
            public const string NotFound = "Error_Status_NotFound";
            public const string UpdateFailed = "Error_Status_UpdateFailed";
        }

        public static class City
        {
            public const string CityExists = "Error_City_CityExists";
            public const string CreateFailed = "Error_City_CreateFailed";
            public const string NotFound = "Error_City_NotFound";
            public const string UpdateFailed = "Error_City_UpdateFailed";
        }

        public static class UnitClient
        {

            public const string UnitClientExists = "Error_UnitClientt_UnitClientExists";
            public const string CreateFailed = "Error_UnitClient_CreateFailed";
            public const string NotFound = "Error_UnitClient_NotFound";
            public const string UnitNotFound = "Error_UnitClient_UnitNotFound";
            public const string UpdateFailed = "Error_UnitClient_UpdateFailed";
            public const string StatusNotFound = "Error_UnitClient_StatusNotFound";
        }
    }

    public static class Notification
    {
        public static class Subscribe
        {
            public const string Subscribed = "Notification_Subscribe_Subscribed";
            public const string Unsubscribed = "Notification_Subscribe_Unsubscribed";
        }

        public static class Contact
        {
            public const string NewMessage = "Notification_Contact_NewMessage";
        }

    }
}


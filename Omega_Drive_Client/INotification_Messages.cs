using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    internal interface INotification_Messages
    {
        public static string connection_failed_message = "Connection failed";
        public static string passwords_do_not_match = "Passwords do not match";
        public static string email_already_in_use_message = "Email already in use";
        public static string account_registration_successful = "Registration successful";
        public static string invalid_email_address = "Invalid email address";
        public static string invalid_password = "Invalid password";
        public static string account_not_validated = "Un-validated account";
        public static string login_successful = "Log in successful";
        public static string invalid_account_validation_code = "Invalid account validation code";
        public static string invalid_log_in_code = "Invalid log in code";
        public static string account_validation_successful = "Account validation successful";
        public static string invalid_password_length = "Invalid password length";
        public static string invalid_ssl_certificate_password= "Invalid certificate password";
        public static string ssl_certificate_loadup_successful= "SSL certificate loadup successful";
        public static string account_authentification_successful = "Account authentification successful";
        public static string log_in_session_key_is_valid = "Log in session key is valid";
        public static string error_loading_user_cache = "Error loading user cache";



        public static readonly Dictionary<string, string> notifications_and_values = new Dictionary<string, string>
        {
            {connection_failed_message, "The connection to the server failed. Please ensure that the server's IP address and port number are valid, or that the SSL protocol used is the same as the one used by the server, or that the SSL certificate used is valid."},
            {passwords_do_not_match, "The passwords provided do not match."},
            {email_already_in_use_message, "The email provided is already in use. Please use another email address."},
            {invalid_email_address, "The email address provided is invalid."},
            {invalid_password, "The password provided isn't valid."},
            {account_not_validated, "The account is not validated. Check your email and insert your account validation code."},
            {invalid_account_validation_code, "This account validation code is not valid. Please enter a valid account validation code."},
            {invalid_log_in_code, "This log in code is not valid. Please enter a valid log in code."},
            {invalid_password_length, "The length of the password must be greater than 6 characters long."},
            {account_registration_successful, "Check your account validation code sent to this account's email address and log in."},
            {account_validation_successful, "Account validation is successful, please log in into your account."},
            {invalid_ssl_certificate_password, "The password provided for the certificate is invalid."},
            {ssl_certificate_loadup_successful, "The certificate was added successfuly to the user's certificate store."}
        };

      

        public void Log_In_Result_Processing(string result, object obj);

        public void Register_Result_Processing(string result, object obj);

        public void SslCertificate_Result_Processing(string result, object obj);

        public void Log_In_Code_Result_Processing(string result, object obj);

        public void Account_Validation_Result_Processing(string result, object obj);

        public void Password_Window_Account_Authentification_Result_Processing(string result, Tuple<object, object> obj);

        public void Log_In_Or_Register_Window_Account_Authentification_Result_Processing(string result, object obj);

        public void Log_In_Session_Key_Verification_Result_Processing(string result, object obj);

        public void Log_Out_Result_Processing(string result, object obj);

        public void Files_Loadup_Result_Processing(string result, object obj);
    }
}

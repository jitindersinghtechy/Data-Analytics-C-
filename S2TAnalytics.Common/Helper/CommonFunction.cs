using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Security.Cryptography;
using System.Configuration;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Net.Http;
using S2TAnalytics.Common.Utilities;
using System.Reflection;
using System.Web;

namespace S2TAnalytics.Common.Helper
{
    public static class CommonFunction
    {
        public static string GetResetPasswordUrl(string email, string code, int mode)
        {
            string relativePath = System.Web.VirtualPathUtility.ToAbsolute("~/");
            return GetWebSiteUrl() + relativePath + "#/Confirm/" + email + "/" + code;
        }

        public static string GetWelcomeUrl()
        {
            string relativePath = System.Web.VirtualPathUtility.ToAbsolute("~/");
            return GetWebSiteUrl() + relativePath + "#/login";
        }

        public static string GetSetPasswordUrl(string email, string code, int mode)
        {
            string relativePath = System.Web.VirtualPathUtility.ToAbsolute("~/");
            return GetWebSiteUrl() + relativePath + "#/SetPassword/" + email + "/" + code;
        }
        //public static string ConfigureResetPasswordMailBody(string url)
        //{
        //    string body = string.Empty;
        //    body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/Activation.html"));
        //    body.Replace("{{MYLINK}}", url);
        //    //body += "<html><body>"
        //    //      + "We have received your request for reset password.<br/><br/><a href=" + url + "> Click here</a> to confirmed your email<br><br>Regards,<br>The S2TAnalytics</body> </html>";
        //    return body;
        //}
        public static string ConfigureNewPasswordMailBody(string url, string name)
        {
            string body = string.Empty;
            body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/Activation.html"));
            body = body.Replace("{{NAME}}", name);
            body = body.Replace("{{MYLINK}}", url);
            //body += "<html><body>"
            //      + "Hi " + name + ", <br/>You have been successfully registered on S2TAnalytics.<br/><br/><a href=" + url + "> Click here</a> to confirm your email .<br><br>Regards,<br>The S2TAnalytics</body> </html>";
            return body;
        }

        public static string WelcomeMailBody(string url, string name)
        {
            string body = string.Empty;
            body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/Welcome.html"));
            body = body.Replace("{{UNAME}}", name);
            body = body.Replace("{{MYLINK}}", url);
            //body += "<html><body>"
            //      + "Hi " + name + ", <br/>You have been successfully registered on S2TAnalytics.<br/><br/><a href=" + url + "> Click here</a> to confirm your email .<br><br>Regards,<br>The S2TAnalytics</body> </html>";
            return body;
        }

        public static string SetPasswordMailBody(string url, string name)
        {
            string body = string.Empty;
            body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/InitialPasswordSetup.html"));
            body = body.Replace("{{NAME}}", name);
            body = body.Replace("{{MYLINK}}", url);
            //body += "<html><body>"
            //      + "Hi " + name + ", <br/>You have been successfully added on S2TAnalytics.<br/><br/><a href=" + url + "> Click here</a> to set your password .<br><br>Regards,<br>The S2TAnalytics</body> </html>";
            return body;
        }
        public static string ForgotPasswordMailBody(string url, string name)
        {
            string body = string.Empty;
            body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/ResetPassword.html"));
            body = body.Replace("{{NAME}}", name);
            body = body.Replace("{{MYLINK}}", url);
            //body += "<html><body>"
            //      + "Hi " + name + ", <br/>Please reset your password by clicking on below link.<br/><br/><a href=" + url + "> Click here</a> <br><br>Regards,<br>The S2TAnalytics</body> </html>";
            return body;
        }

        public static string InvoiceMailBody(string name)
        {
            string body = string.Empty;
            //body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/ResetPassword.html"));
            // body = body.Replace("{{NAME}}", name);
            //body = body.Replace("{{MYLINK}}", url);
            body += "<html><body>"
                  + "Hi " + name + ", <br/>Please find attached invoice(s).<br/><br/> <br><br>Regards,<br>The S2TAnalytics</body> </html>";
            return body;
        }

        public static string OTPMailBody(string name,string OTP)
        {
            string body = string.Empty;
            body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/OTP.html"));
            body = body.Replace("{{NAME}}", name);
            body = body.Replace("{{OTP}}", OTP);
        
            return body;
        }


        public static string ReminderMailBody(string name, string type)
        {
            string body = string.Empty;
            body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/Reminder.html"));
            body = body.Replace("{{NAME}}", name);
            body = body.Replace("{{TYPE}}", type);
         
            return body;
        }
        public static string PaymentOverDueReminderMailBody(string name, string type)
        {
            string body = string.Empty;
            body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/PaymentOverdue.html"));
            body = body.Replace("{{UNAME}}", name);
            return body;
        }
        public static string AfterTrailPeriodReminderMailBody(string name, string type)
        {
            var url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = string.Empty;
            body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/AfterTrailPeriod.html"));
            body = body.Replace("{{UNAME}}", name);
            body = body.Replace("{{MYLINK}}",url);
            return body;
        }

        public static string ContactUsMailBody(string name, string query)
        {
            string body = string.Empty;
            body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/ContactUs.html"));
            body = body.Replace("{{UNAME}}", name);
            body = body.Replace("{{Query}}", query);
            return body;
        }

        public static string TrailPeriodReminderMailBody(string name, string type)
        {
            string body = string.Empty;
            body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/TrailPeriod.html"));
            body = body.Replace("{{UNAME}}", name);
            return body;
        }




        public static string ContactUs()
        {
            string body = string.Empty;
            body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/Reminder.html"));

            return body;
        }

        public static string GetWebSiteUrl()
        {
            Uri _url = HttpContext.Current.Request.Url;
            return _url.Scheme + "://" + _url.Authority;
        }
        public static string GenerateToken()
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Guid.NewGuid().ToString().Trim());
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string GenerateUniqueRecordNumber()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
            return String.Format("{0,D8}", random);
        }

        public static string RemoveHTML(string strHTML, int charLength = 1, bool isSubject = false)
        {
            var removeHtml = Regex.Replace(strHTML, "<(.|\n)*?>", "").Replace("&nbsp;", " ");

            if (removeHtml.Length > charLength && !isSubject)
                removeHtml = removeHtml.Substring(0, charLength - 1).Insert(charLength - 1, "......");
            if (removeHtml.Length > charLength && isSubject)
                removeHtml = removeHtml.Substring(0, charLength - 1);

            return removeHtml;
        }

        public static string Encrypt(this Int32 toEncrypt)
        {
            if (toEncrypt == 0)
                return "";

            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt.ToString());

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            // Get the key from config file

            string key = "App";
            // (string)settingsReader.GetValue("SecurityKey",
            //                                 typeof(String));
            //System.Windows.Forms.MessageBox.Show(key);
            ////If hashing use get hashcode regards to your key
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //Always release the resources and flush data
            // of the Cryptographic service provide. Best Practice

            //hashmd5.Clear();
            //else
            //keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format

            return Convert.ToBase64String(resultArray, 0, resultArray.Length).Replace("/", "^").Replace("+", "~");

        }
        public static int Decrypt(this string cipherString)
        {
            int checkInt = 0;
            if (string.IsNullOrEmpty(cipherString) || (int.TryParse(cipherString, out checkInt) && checkInt == 0))
                return 0;

            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString.Replace("^", "/").Replace("~", "+"));

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            //Get your key from config file to open the lock!
            string key = "App";
            //(string)settingsReader.GetValue("SecurityKey",
            //                                       typeof(String));

            //if hashing was used get the hash code with regards to your key
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            ////release any resource held by the MD5CryptoServiceProvider

            //hashmd5.Clear();

            //else
            //{
            //if hashing was not implemented get the byte code of the key
            // keyArray = UTF8Encoding.UTF8.GetBytes(key);
            //}

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            string result = UTF8Encoding.UTF8.GetString(resultArray);

            int iresult = 0;
            if (int.TryParse(result, out iresult))
                return iresult;

            return -1;
        }


        public static bool ComparePassword(string password, string pwdEncrypt)
        {
            if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(pwdEncrypt))
                return false;
            var passwordHasher = new PasswordHasher();
            return passwordHasher.CompareStringToHash(password, pwdEncrypt);
        }
        public static string PasswordEncrypt(this string toPwdEncrypt)
        {
            if (string.IsNullOrEmpty(toPwdEncrypt))
                return null;

            var passwordHasher = new PasswordHasher();
            passwordHasher.PasswordEncrypt(toPwdEncrypt);
            return passwordHasher.PasswordEncrypt(toPwdEncrypt);

            //const string password = "SampleP455w0rd";
            //byte[] salt = GenerateSalt();
            //var hashedPassword = HashPasswordWithSalt(Encoding.UTF8.GetBytes(toPwdEncrypt), salt);
            //return Convert.ToBase64String(hashedPassword);

        }
        private static byte[] GenerateSalt()
        {
            const int saltLength = 32;

            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[saltLength];
                randomNumberGenerator.GetBytes(randomNumber);

                return randomNumber;
            }
        }
        private static byte[] Combine(byte[] first, byte[] second)
        {
            var ret = new byte[first.Length + second.Length];

            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);

            return ret;
        }
        public static byte[] HashPasswordWithSalt(byte[] toBeHashed, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combinedHash = Combine(toBeHashed, salt);

                return sha256.ComputeHash(combinedHash);
            }
        }

    }
    public static class Helper
    {

        private static string[] formats = new string[] {
              "MM/d/yyyy",
              "MM/dd/yyyy",
              "MM-dd-yyyy",
              "yyyy-MM-dd",
              "yyyy-MM-dd H,mm,ss tt",
              "yyyy-MM-dd HH,mm,ss tt",
              "yyyy-MM-dd HH,mm,ss",
              "yyyy-MM-dd hh,mm,ss tt",
              "yyyy-MM-dd h,mm,ss tt",
              "yyyy-m-dd h,mm,ss tt",
              "yyyy-mm-d h,mm,ss tt",
              "yyyy-m-d h,mm,ss tt",
              "MM/dd/yyyy H,mm,ss tt",
              //"mm-dd-yyyy",
              // "MM-dd-yyyy",
              "MM/dd/yyyy hh,mm,ss tt",
              "M/d/yyyy h,mm,ss tt",
              "M/d/yyyy HH,mm,ss",
              "M/d/yyyy HH,mm,s",
              "M/d/yyyy HH,m,ss",
              "M/d/yyyy H,m,ss",
              "M/d/yyyy H,m,s",
              "M/dd/yyyy h,mm,ss tt",
              "M-dd-yyyy h,mm,ss tt",
              "M-d-yyyy h,mm,ss tt",
              "MM-dd-yyyy hh,mm,ss tt",
              //"dd-MM-yyyy hh,mm,ss tt",
              // "dd-MM-yyyy h,mm,ss tt",
              // "d-MM-yyyy hh,mm,ss tt",
              //  "dd-M-yyyy h,mm,ss tt",
              //  "dd-mm-yyyy"
            };
        public static string GetEnumDisplayName(this Enum enumValue)
        {
            return enumValue.GetType().GetMember(enumValue.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()
                           .Name;
        }
        public static string ToYear(this DateTime date)
        {
            return date.ToString("yyyy");
        }
        //June 28, 2016 11,42,57
        public static string ToTimerFormat(this DateTime date)
        {
            return date.ToString("MMMM dd, yyyy HH,mm,ss");
        }


        public static DateTime ToDatetime(this string date)
        {
            return DateTime.ParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);


        }




        public static string ToMonthDay(this DateTime date)
        {
            return date.ToString("MMMM dd");
        }
        public static string ToHHMMTT(this DateTime date)
        {
            return date.ToString("hh,mm tt");
        }

        public static string ToHHMMSS(this DateTime date)
        {
            return date.ToString("HH,mm,ss");
        }

        //It's using on Order listing page
        //public static string ToMonthYear(this DateTime date)
        //{
        //    return date.ToString("MMM yyyy");
        //}


        public static string ToYear(this string date)
        {
            return DateTime.ParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy");
        }

        public static string ToMonthDay(this string date)
        {
            return DateTime.ParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MMMM dd");
        }


        //It's using on Order listing page
        public static string ToMonthYear(this DateTime date)
        {
            return date.ToString("MMM. yyyy");
        }


        public static string ToMMDDYYYY(this string date)
        {
            return DateTime.ParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("MM/dd/yyyy");
        }

        public static string ToMMDDYYYY(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy");
        }
        public static string ToDateTime(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy hh,mm tt");
        }

        //public static string ToMMDDYYYYHHMM(this DateTime date)
        //{
        //    return date.ToString("MM/dd/yyyy hh,mm tt");
        //}


        public static string ToYYYYMMDD(this string date)
        {
            return DateTime.ParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("yyyy-MM-dd");
        }

        public static string ToUPSTime(this TimeSpan time)
        {
            return new DateTime(time.Ticks).ToString("HHmm");
        }
        public static string ToUPSDate(this DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }
        public static string ToDateFormat(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy");
        }
        public static TimeSpan ToTimeSpan(this string timeString)
        {
            var dt = DateTime.ParseExact(timeString, "h,mm tt", System.Globalization.CultureInfo.InvariantCulture);
            return dt.TimeOfDay;
        }

        public static DateTime ToDateTime(this TimeSpan timeSpan)
        {
            return DateTime.Parse(timeSpan.ToString());
        }

        public static DateTime ToDateTime(this string date)
        {
            return DateTime.ParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public static class EnumHelper<T>
        {
            public static T GetValueFromName(string name)
            {
                var type = typeof(T);
                if (!type.IsEnum) throw new InvalidOperationException();

                foreach (var field in type.GetFields())
                {
                    var attribute = Attribute.GetCustomAttribute(field,
                        typeof(DisplayAttribute)) as DisplayAttribute;
                    if (attribute != null)
                    {
                        if (attribute.Name == name)
                        {
                            return (T)field.GetValue(null);
                        }
                    }
                    else
                    {
                        if (field.Name == name)
                            return (T)field.GetValue(null);
                    }
                }

                throw new ArgumentOutOfRangeException("name");
            }
        }

        
    }

    public static class Timelines {
        public static List<int> MasterTimelines = new List<int>() { 1, 2, 3, 4, 6, 45, 46, 47, 48, 49 };
        public static List<int> timelinesTillToday = new List<int>() { 45, 46, 47, 48, 49 };
        public static List<int> MonthlyTimelines = new List<int>() { 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 73, 72, 71, 70, 69, 68, 67, 66, 65, 64, 63, 62 };
        public static List<int> WeeklyTimelines = new List<int>() { 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50 };
        public static List<int> DailyTimelines = new List<int>() { 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31 };
    }

    public static class MathCalculations
    {
        private static Random rnd = new Random();
        public static Int32 GenerateRandomNo(int toPlaces)
        {
            var min = "";
            var max = "";

            for (int i = 0; i < toPlaces; i++)
            {
                if (i == 0)
                {
                    min += "1";
                }
                else if (i < toPlaces - 1)
                {
                    min += "0";
                }
                max += "9";
            }
            return rnd.Next(Convert.ToInt32(min), Convert.ToInt32(max));
        }
    }
}

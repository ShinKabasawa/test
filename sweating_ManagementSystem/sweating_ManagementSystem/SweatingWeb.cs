
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
namespace sweating_ManagementSystem
{
    class SweatingWeb
    {
        private static String requestTime;
        public String login_Id = "testuser1";
        public String login_Pass = "testuser1";
        public String card_no = "A0000001";
        private static String access_Key = "Gr7hNBQ0rZU5vx2y6NM34rDFQfG_U6XM";
        public static String api_Id;
        private static String api_Pass;
        private static String hex;
        public static String Status;
        public String errorResp;
        public static String IP;
        private ErrorLog ELog = new ErrorLog();
        private ErrorLog.ErrorLocation location = ErrorLog.ErrorLocation.Def;

        // データ送信先（仮）
        private static string URL = "http://crispy.stars.ne.jp/receive.php";

        /// <summary>
        /// 取得
        /// </summary>
        public enum Response
        {
            UserResponse,
            GuidanceResponse,
            PlaqueResponse,
            TextHelpResponse,
            PatientInfoResponse,
        }

        /*
        /// <summary>
        /// 取得
        /// </summary>
        public enum Response
        {
            UserResponse,
            GuidanceResponse,
            PlaqueResponse,
            TextHelpResponse,
            PatientInfoResponse,
        }
         * */

        /// <summary>
        /// 送信
        /// </summary>
        public enum Request
        {
            DevaiceDataRequest,
            UserAuthRequest,
            DeleteUser
        }


        /*
        /// <summary>
        /// 送信
        /// </summary>
        public enum Request
        {
            GuidanceRequest,
            PlaqueRequest,
            UserDataRequest,
            ParseRequest,
            DeleteBrush
        }
         * */


        /// <summary>
        /// 応答
        /// </summary>
        public enum Rcv
        {
            DevDataRcv,
            UserAuthRcv,
        }

        /// <summary>
        /// トークン作成
        /// </summary>
        /// <param name="unix"></param>
        /// <returns></returns>
        public static String GetNonce(long unix)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            var n = md5.ComputeHash(Encoding.UTF8.GetBytes(unix.ToString()));
            var nonce = sha1.ComputeHash(n);
            hex = BitConverter.ToString(nonce).ToLower().Replace("-", "");
            var nonce_base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(hex)).Replace("-", "");

            return nonce_base64;
        }

        /// <summary>
        /// パスワードダイジェスト作成
        /// </summary>
        /// <param name="nonce"></param>
        /// <param name="created"></param>
        /// <param name="pwd"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static String GetDigest(String nonce, String created, String pwd, String key)
        {
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            var dg = nonce + pwd + requestTime + key;
            var digest = Encoding.UTF8.GetBytes(dg);
            var pass_digest = sha256.ComputeHash(digest);
            var low = BitConverter.ToString(pass_digest).ToLower().Replace("-", "");
            var pass_digest2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(low)).Replace("-", "");

            return pass_digest2;
        }

        /// <summary>
        /// 受信API
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public Object API_Response(Response api)
        {
            Object obj = new object();

            obj = APIIdRequest();

            switch (api)
            {
                case Response.UserResponse:
                    obj = APIIdRequest();
                    break;
                case Response.GuidanceResponse:
                    //obj = APIGuidanceRequest();
                    break;
                case Response.PlaqueResponse:
                    //obj = APIPlaqueRequest();
                    break;
                case Response.PatientInfoResponse:
                    obj = APIPatientInfoRequest();
                    break;
            }
            return obj;
        }

        /// <summary>
        /// 送信API
        /// </summary>
        /// <param name="api"></param>
        /// <param name="obj"></param>
        public static String API_Request(Request api, Object obj)
        {
            String status = String.Empty;

            switch (api)
            {
                case Request.UserAuthRequest:
                    status = APIUserInfoResponse((API_USER_Auth_Info)obj);
                    break;
                case Request.DevaiceDataRequest:
                    status = APIDevaiceDataResponse((API_Device)obj);
                    break;
                case Request.DeleteUser:
                    status = APIPlaqueResultResponse((API_User_Del)obj);
                    break;
                default:
                    break;
            }

            if (status != null)
            {
                if (!status.Equals("OK"))
                {
                    // 送信失敗
                    
                }
                else
                {
                    // 送信成功
                }
            }
            else
            {
                status = "null";
            }

            return status;

        }




        /// <summary>
        /// 受信正常API
        /// </summary>
        /// <param name="api"></param>
        public void API_OK(Rcv api)
        {
            switch (api)
            {
                case Rcv.DevDataRcv:
                    APIDeviceDataOkResponse();
                    break;
                case Rcv.UserAuthRcv:
                    APIUserAuthOkResponse();
                    break;
            }
        }

        //public API_TOOTHBRUSH_USER API_Brush(API_SERIAL_NO json)
        //{
        //    var info = APIToothbrushUserResponse(json);
        //    return info;
        //}

        private static DateTime n;
        private static long GetUnixTime(DateTime target)
        {
            var UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var _target = target.ToUniversalTime();

            return (long)_target.Subtract(UNIX_EPOCH).TotalSeconds;
        }

        private String NO_CLOUD_PASS = "kja50e1m3543G6kGwjrs#kh6";
        private string result;
        //***********************************************************//
        //                                                           //
        //                        Request                            //
        //                                                           //
        //***********************************************************//
        /// <summary>
        /// APIID取得
        /// </summary>
        /// <returns></returns>
        private API_USER APIIdRequest()
        {
            n = DateTime.Now;
           
            requestTime = n.ToString("yyyy-MM-ddTHH:mm:ss+9:00");

            if (requestTime.Length < 24)
            {
                requestTime = String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}+9:00", n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
            }


            var _n = GetUnixTime(n);
            var nonce = GetNonce(_n);
            var digest = GetDigest(hex, requestTime, NO_CLOUD_PASS, access_Key);

            var wsse = String.Format("UsernameToken Username={0}, PasswordDigest={1}, Nonce={2}, Created={3}", login_Id, digest, nonce, requestTime);

            //if (!IP.StartsWith("http")) { 
            //    IP = "http://" + IP;
            //}
            //var url = IP + "/WebAPI/user_auth_pc_apl.php";

            var url = URL;

            String bodyData = "{\"p\":\"" + login_Pass + "\"}";
            Byte[] b = Encoding.ASCII.GetBytes(bodyData);

            var req = System.Net.HttpWebRequest.Create(url);
            req.Method = "POST";
            req.Headers.Add("X-WSSE", wsse);
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = b.Length;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            try
            {
                var stream = req.GetRequestStream();
                stream.Write(b, 0, b.Length);
                stream.Close();

                System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();
                var resStream = res.GetResponseStream();
                var serializer = new DataContractJsonSerializer(typeof(API_USER));
                API_USER info = (API_USER)serializer.ReadObject(resStream);
                Status = res.StatusCode.ToString();
                resStream.Close();
                res.Close();
                api_Id = info.api_id;
                api_Pass = info.api_key;

                return info;
            }
            catch (System.Net.WebException e)
            {
                Status = e.Status.ToString();

                return new API_USER();
            }
            catch (Exception e)
            {
                return new API_USER();
            }
        }

        /// <summary>
        /// 患者情報データ取得
        /// </summary>
        /// <returns></returns>
        private API_PATIENT_INFO APIPatientInfoRequest()
        {
            location = ErrorLog.ErrorLocation.UserSearch;
            n = DateTime.Now;
            requestTime = n.ToString("yyyy-MM-ddTHH:mm:ss+9:00");

            // -- 2018.01.29 ADD START --
            if (requestTime.Length < 24)
            {
                requestTime = String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}+9:00", n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
            }
            // -- 2018.01.29 ADD END   --

            var _n = GetUnixTime(n);
            var nonce = GetNonce(_n);
            var digest = GetDigest(hex, requestTime, api_Pass, access_Key);

            var wsse = String.Format("UsernameToken Username={0}, PasswordDigest={1}, Nonce={2}, Created={3}", api_Id, digest, nonce, requestTime);

            //if (!IP.StartsWith("http"))
            //{
            //    IP = "http://" + IP;
            //}

            var url = URL ;

            String bodyData = "{\"c\":\"" + card_no + "\"}";
            Byte[] b = Encoding.ASCII.GetBytes(bodyData);

            var req = System.Net.HttpWebRequest.Create(url);
            req.Method = "POST";
            req.Headers.Add("X-WSSE", wsse);
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = b.Length;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072; 

            try
            {
                var stream = req.GetRequestStream();
                stream.Write(b, 0, b.Length);
                stream.Close();

                System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();

                var resStream = res.GetResponseStream();

                var serializer = new DataContractJsonSerializer(typeof(API_PATIENT_INFO));
                API_PATIENT_INFO info = (API_PATIENT_INFO)serializer.ReadObject(resStream);
                Status = res.StatusCode.ToString();
                resStream.Close();
                res.Close();

                return info;
            }
            catch (System.Net.WebException e)
            {
                Status = e.Status.ToString();

                //ErrorLog.ErrorOutput(Status, location);
            }
            catch (Exception e)
            {
                //ErrorLog.ErrorOutput(e.Message, location);
            }
            return new API_PATIENT_INFO();
        }

        //***********************************************************//
        //                                                           //
        //                        Response                           //
        //                                                           //
        //***********************************************************//

        /// <summary>
        /// 使用者情報送信
        /// </summary>
        /// <param name="json"></param>
        public static String APIUserInfoResponse(API_USER_Auth_Info json)
        {
            n = DateTime.Now;
            requestTime = n.ToString("yyyy-MM-ddTHH:mm:ss+9:00");

            if (requestTime.Length < 24)
            {
                requestTime = String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}+9:00", n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
            }
            var _n = GetUnixTime(n);
            var nonce = GetNonce(_n);
            var digest = GetDigest(hex, requestTime, api_Pass, access_Key);

            var wsse = String.Format("UsernameToken Username={0}, PasswordDigest={1}, Nonce={2}, Created={3}", api_Id, digest, nonce, requestTime);

            //if (!IP.StartsWith("http"))
            //{
            //    IP = "http://" + IP;
            //}
            //var url = IP + "/WebAPI/user_info_send.php";
            var url = URL;


            var req = System.Net.HttpWebRequest.Create(url);
            req.Method = "POST";
            req.Headers.Add("x-wsse", wsse);

            try
            {
                DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(API_USER_Auth_Info));
                MemoryStream ms = new MemoryStream();
                jsonSer.WriteObject(ms, json);
                ms.Position = 0;
                using (StreamWriter w = new StreamWriter(req.GetRequestStream()))
                {
                    w.Write(new StreamReader(ms).ReadToEnd());
                }
                System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();

                var resStream = res.GetResponseStream();
                Status = res.StatusCode.ToString();
                Console.WriteLine(Status);
                resStream.Close();
                res.Close();
            }
            catch (System.Net.WebException e)
            {
                Status = e.Status.ToString();
            }
            catch
            {
            }
            return Status;
        }

        /// <summary>
        /// プラーク結果送信
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static String APIPlaqueResultResponse(API_User_Del json)
        {
            n = DateTime.Now;
            requestTime = n.ToString("yyyy-MM-ddTHH:mm:ss+9:00");

            // -- 2018.01.29 ADD START --
            if (requestTime.Length < 24)
            {
                requestTime = String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}+9:00", n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
            }
            // -- 2018.01.29 ADD END   --

            var _n = GetUnixTime(n);
            var nonce = GetNonce(_n);
            var digest = GetDigest(hex, requestTime, api_Pass, access_Key);

            var wsse = String.Format("UsernameToken Username={0}, PasswordDigest={1}, Nonce={2}, Created={3}", api_Id, digest, nonce, requestTime);

            //if (!IP.StartsWith("http"))
            //{
            //    IP = "http://" + IP;
            //}
            //var url = IP + "/WebAPI/plaque_send.php";
            var url = URL;

            var req = System.Net.HttpWebRequest.Create(url);
            req.Method = "POST";
            req.Headers.Add("x-wsse", wsse);

            try
            {
                DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(API_User_Del));
                MemoryStream ms = new MemoryStream();
                jsonSer.WriteObject(ms, json);
                ms.Position = 0;
                using (StreamWriter w = new StreamWriter(req.GetRequestStream()))
                {
                    w.Write(new StreamReader(ms).ReadToEnd());
                }
                System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();

                var resStream = res.GetResponseStream();
                Status = res.StatusCode.ToString();
                Console.WriteLine(Status);
                resStream.Close();
                res.Close();
            }

            catch (System.Net.WebException e)
            {
                Status = e.Status.ToString();
            }
            catch
            {
            }
            return Status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static String APIDevaiceDataResponse(API_Device json)
        {
            n = DateTime.Now;
            requestTime = n.ToString("yyyy-MM-ddTHH:mm:ss+9:00");

           if (requestTime.Length < 24)
            {
                requestTime = String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}+9:00", n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
            }

            var _n = GetUnixTime(n);
            var nonce = GetNonce(_n);
            var digest = GetDigest(hex, requestTime, api_Pass, access_Key);
            var wsse = String.Format("UsernameToken Username={0}, PasswordDigest={1}, Nonce={2}, Created={3}", api_Id, digest, nonce, requestTime);

            //if (!IP.StartsWith("http"))
            //{
            //    IP = "http://" + IP;
            //}
            //var url = IP + "/WebAPI/guidance_send.php";

            var url = URL ;


            var req = System.Net.HttpWebRequest.Create(url);
            req.Method = "POST";
            req.Headers.Add("x-wsse", wsse);
            req.ContentType = "application/x-www-form-urlencoded";

            try
            {
                DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(API_Device));
                MemoryStream ms = new MemoryStream();
                jsonSer.WriteObject(ms, json);
                ms.Position = 0;
                using (StreamWriter w = new StreamWriter(req.GetRequestStream()))
                {
                    w.Write(new StreamReader(ms).ReadToEnd());
                }

                string JSON = getJson(json);

                System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();

                var resStream = res.GetResponseStream();
                Status = res.StatusCode.ToString();
                resStream.Close();
                res.Close();
            }
            catch (System.Net.WebException e)
            {
                Status = e.Status.ToString();
            }
            catch
            {
            }
            return Status;
        }

        private static string getJson(API_Device json)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(json.GetType());
                serializer.WriteObject(stream, json);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private String APIAnalysisResponse(API_ANALYSIS_DATA json)
        {
            location = ErrorLog.ErrorLocation.Analysis;
            n = DateTime.Now;
            requestTime = n.ToString("yyyy-MM-ddTHH:mm:ss+9:00");

            if (requestTime.Length < 24)
            {
                requestTime = String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}+9:00", n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
            }
            var _n = GetUnixTime(n);
            var nonce = GetNonce(_n);
            var digest = GetDigest(hex, requestTime, api_Pass, access_Key);

            var wsse = String.Format("UsernameToken Username={0}, PasswordDigest={1}, Nonce={2}, Created={3}", api_Id, digest, nonce, requestTime);

            //if (!IP.StartsWith("http"))
            //{
            //    IP = "http://" + IP;
            //}

            var url = IP + "/WebAPI/analysis.php";
            var req = System.Net.HttpWebRequest.Create(url);
            req.Method = "POST";
            req.Headers.Add("x-wsse", wsse);
            req.ContentType = "application/zip";
            
            try
            {
                byte[] b;
            FileStream fs = new FileStream( System.Environment.CurrentDirectory + @"\analysis\json.zip", FileMode.Open, FileAccess.Read);
                    b = new byte[fs.Length];
                    fs.Read(b, 0, b.Length);
                    var stream = req.GetRequestStream();
                    stream.Write(b, 0, b.Length);
                    fs.Close();
                    stream.Close();

                System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();
                var resStream = res.GetResponseStream();
                Status = res.StatusCode.ToString();

                resStream.Close();
                res.Close();
                return Status;
            }
            catch (System.Net.WebException e)
            {
                Status = e.Status.ToString();
                return Status;
            }
            catch (Exception ex)
            {
                return Status;
            }
        }

        /// <summary>
        /// 指導設定受信OK
        /// </summary>
        /// <param name="json"></param>
        private void APIDeviceDataOkResponse()
        {
            n = DateTime.Now;
            requestTime = n.ToString("yyyy-MM-ddTHH:mm:ss+9:00");

            if (requestTime.Length < 24)
            {
                requestTime = String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}+9:00", n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
            }

            var _n = GetUnixTime(n);
            var nonce = GetNonce(_n);
            var digest = GetDigest(hex, requestTime, api_Pass, access_Key);

            var wsse = String.Format("UsernameToken Username={0}, PasswordDigest={1}, Nonce={2}, Created={3}", api_Id, digest, nonce, requestTime);

            //if (!IP.StartsWith("http"))
            //{
            //    IP = "http://" + IP;
            //}
            //var url = IP + "/WebAPI/guidance_rcv_ok.php";
            var url = URL;

            var req = System.Net.HttpWebRequest.Create(url);
            req.Method = "POST";
            req.Headers.Add("x-wsse", wsse);

            try
            {
                System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();

                var resStream = res.GetResponseStream();
                Status = res.StatusCode.ToString();
                resStream.Close();
                res.Close();
            }
            catch (System.Net.WebException e)
            {
                Status = e.Status.ToString();
            }
            catch
            {

            }
        }

        /// <summary>
        /// プラーク情報受信OK
        /// </summary>
        /// <param name="json"></param>
        private void APIUserAuthOkResponse()
        {
            n = DateTime.Now;
            requestTime = n.ToString("yyyy-MM-ddTHH:mm:ss+9:00");

            if (requestTime.Length < 24)
            {
                requestTime = String.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}+9:00", n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
            }

            var _n = GetUnixTime(n);
            var nonce = GetNonce(_n);
            var digest = GetDigest(hex, requestTime, api_Pass, access_Key);

            var wsse = String.Format("UsernameToken Username={0}, PasswordDigest={1}, Nonce={2}, Created={3}", api_Id, digest, nonce, requestTime);

            //if (!IP.StartsWith("http"))
            //{
            //    IP = "http://" + IP;
            //}
            //var url = "http://" + IP + "/WebAPI/plaque_rcv_ok.php";
            var url = URL;

            var req = System.Net.HttpWebRequest.Create(url);
            req.Method = "POST";
            req.Headers.Add("x-wsse", wsse);

            try
            {
                System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();

                var resStream = res.GetResponseStream();
                Status = res.StatusCode.ToString();
                resStream.Close();
                res.Close();
            }
            catch (System.Net.WebException e)
            {
                Status = e.Status.ToString();
            }
            catch
            {

            }
        }

        //***********************************************************//
        //                                                           //
        //                          Class                            //
        //                                                           //
        //***********************************************************//

        [DataContract]
        public class API_SERIAL_NO
        {
            [DataMember(Name="s")]
            public String s { get; set; }
        }
        /// <summary>
        /// ID取得
        /// </summary>
        [DataContract]
        public class API_USER
        {
            [DataMember(Name = "api_id")]
            public String api_id { get; set; }

            [DataMember(Name = "api_key")]
            public String api_key { get; set; }

            [DataMember(Name = "user_type", IsRequired = false)]
            public Char user_type { get; set; }

            [DataMember(Name = "user_name",IsRequired = false)]
            public String user_name { get; set; }
        }

        /// <summary>
        /// 指導設定受信
        /// </summary>
        [DataContract]
        public class API_Device
        {
            [DataMember(Name = "guidance", IsRequired = false)]
            public List<Guidance> guidance { get; set; }

            [DataContract]
            public class Guidance
            {
                [DataMember(Name = "card_no", IsRequired = false)]
                public String card_no { get; set; }

                [DataMember(Name = "serial_no")]
                public String serial_no { get; set; }

                [DataMember(Name = "setting_no",IsRequired = false)]
                public Char setting_no { get; set; }

                [DataMember(Name = "care_data", IsRequired = false)]
                public List<Care_Data> care_data { get; set; }

                [DataContract]
                public class Care_Data
                {
                    [DataMember(Name = "guidance_date", IsRequired = false)]
                    public String guidance_date { get; set; }

                    [DataMember(Name = "setting_contents", IsRequired = false)]
                    public Char setting_contents { get; set; }

                    [DataMember(Name = "toothbrushes_type", IsRequired = false)]
                    public Char toothbrushes_type { get; set; }

                    [DataMember(Name = "guidance_dentist",IsRequired = false)]
                    public string guidance_dentist { get; set; }

                    [DataMember(Name = "setting_staff", IsRequired = false)]
                    public string setting_staff { get; set; }

                    [DataMember(Name = "memo", IsRequired = false)]
                    public string memo { get; set; }

                    [DataMember(Name = "mouth_data",IsRequired = false)]
                    public List<Mouth_Data> mouth_data { get; set; }

                    [DataContract]
                    public class Mouth_Data
                    {
                        [DataMember(Name = "no")]
                        public int? no { get; set; }

                        [DataMember(Name = "time")]
                        public int? time { get; set; }

                        [DataMember(Name = "strength")]
                        public int? strength { get; set; }

                        [DataMember(Name = "angle")]
                        public int? angle { get; set; }

                        [DataMember(Name = "turn")]
                        public int? turn { get; set; }

                        [DataMember(Name = "direction")]
                        public Char? direction { get; set; }
                    }
                }
            }
        }

        /// <summary>
        /// プラーク情報
        /// </summary>
        [DataContract]
        public class API_PLAQUE
        {
            [DataMember(Name = "plaque")]
            public List<Plaque> plaque { get; set; }

            [DataContract]
            public class Plaque
            {
                [DataMember(Name = "card_no")]
                public String card_no { get; set; }

                [DataMember(Name = "guidance_date")]
                public String guidance_date { get; set; }

                [DataMember(Name = "plaque_data")]
                public List<PlaqueData> plaque_data { get; set; }

                [DataContract]
                public class PlaqueData
                {
                    [DataMember(Name = "no")]
                    public int? no { get; set; }

                    [DataMember(Name = "plaque_check")]
                    public char? plaque_check { get; set; }
                }
            }
        }

        /// <summary>
        /// 使用者削除
        /// </summary>
        [DataContract]
        public class API_User_Del
        {
            [DataMember(Name = "plaque")]
            public List<Plaque> plaque { get; set; }

            [DataContract]
            public class Plaque
            {
                [DataMember(Name = "card_no")]
                public String card_no { get; set; }

                [DataMember(Name = "check_date")]
                public String check_date { get; set; }

                [DataMember(Name = "plaque_data")]
                public List<PlaqueData> plaque_data { get; set; }

                [DataContract]
                public class PlaqueData
                {
                    [DataMember(Name = "no")]
                    public int no { get; set; }

                    [DataMember(Name = "plaque_check")]
                    public char? plaque_check { get; set; }
                }
            }
        }

        /// <summary>
        /// 使用者情報
        /// </summary>
        [DataContract]
        public class API_USER_Auth_Info
        {
            [DataMember(Name = "user_info",IsRequired = false)]
            public List<UserInfo> user_info { get; set; }

            [DataContract]
            public class UserInfo
            {
                [DataMember(Name = "card_no",IsRequired = false)]
                public String card_no { get; set; }

                [DataMember(Name = "pass",IsRequired = false)]
                public String pass { get; set; }

                [DataMember(Name = "last_name_kana")]
                public String last_name_kana { get; set; }

                [DataMember(Name = "first_name_kana")]
                public String first_name_kana { get; set; }

                [DataMember(Name = "last_name",IsRequired = false)]
                public String last_name { get; set; }

                [DataMember(Name = "first_name", IsRequired = false)]
                public String first_name { get; set; }

                [DataMember(Name = "birthday",IsRequired = false)]
                public String birthday { get; set; }

                [DataMember(Name = "sex",IsRequired = false)]
                public char sex { get; set; }

                [DataMember(Name = "memo",IsRequired = false)]
                public String memo { get; set; }

                [DataMember(Name = "serial_data",IsRequired = false)]
                public List<SerialData> serial_data { get; set; }



                [DataContract]
                public class SerialData
                {
                    [DataMember(Name = "serial_no",IsRequired = false)]
                    public String serial_no { get; set; }

                    [DataMember(Name = "setting_no")]
                    public char? setting_no { get; set; }
                }
            }
        }

        /// <summary>
        /// 使用者情報
        /// </summary>
        [DataContract]
        public class API_USER_Auth_Info_copy
        {
            [DataMember(Name = "user_info", IsRequired = false)]
            public List<UserInfo> user_info { get; set; }

            [DataContract]
            public class UserInfo
            {
                [DataMember(Name = "name", IsRequired = false)]
                public String name { get; set; }

                [DataMember(Name = "birthday", IsRequired = false)]
                public String birthday { get; set; }

                [DataMember(Name = "gender")]
                public String gender { get; set; }

                [DataMember(Name = "greeting")]
                public String greeting { get; set; }

            }
        }


        /// <summary>
        /// 使用者情報
        /// </summary>
        [DataContract]
        public class API_PATIENT_INFO
        {
            [DataMember(Name = "status", IsRequired = false)]
            public string status { get; set; }


            [DataMember(Name = "patient", IsRequired = false)]
            public UserInfo user_info { get; set; }
            
            [DataContract]
            public class UserInfo
            {
                [DataMember(Name = "hospital_id", IsRequired = false)]
                public String hospital_id { get; set; }

                [DataMember(Name = "card_no", IsRequired = false)]
                public String card_no { get; set; }

                [DataMember(Name = "password", IsRequired = false)]
                public String password { get; set; }

                [DataMember(Name = "last_name_kana")]
                public String last_name_kana { get; set; }

                [DataMember(Name = "first_name_kana")]
                public String first_name_kana { get; set; }

                [DataMember(Name = "last_name", IsRequired = false)]
                public String last_name { get; set; }

                [DataMember(Name = "first_name", IsRequired = false)]
                public String first_name { get; set; }

                [DataMember(Name = "birthday", IsRequired = false)]
                public String birthday { get; set; }

                [DataMember(Name = "sex", IsRequired = false)]
                public char sex { get; set; }
            }
        }

        /// <summary>
        /// 使用者情報
        /// </summary>
        [DataContract]
        public class API_ERROR_INFO
        {
            [DataMember(Name = "status", IsRequired = false)]
            public string status { get; set; }
        }

        /// <summary>
        /// 歯ブラシ使用者設定受信
        /// </summary>
        [DataContract]
        public class API_TOOTHBRUSH_USER
        {
            [DataMember(Name = "toothbrush_user",IsRequired = false)]
            public List<Toothbrush_User> toothbrush_user { get; set; }

            [DataContract]
            public class Toothbrush_User
            {
                [DataMember(Name = "card_no",IsRequired = false)]
                public String card_no { get; set; }

                [DataMember(Name = "setting_no")]
                public char setting_no { get; set; }

                [DataMember(Name = "setting_contents")]
                public char? setting_contents { get; set; }
            }
        }

        /// <summary>
        /// 歯みがき分析結果送信
        /// </summary>
        [DataContract]
        public class API_ANALYSIS_DATA
        {
            [DataMember(Name = "analysis", IsRequired = false)]
            public Analysis_Data analysis { get; set; }

            [DataContract]
            public class Analysis_Data
            {
                [DataMember(Name = "hospital_id")]
                public String hospital_id { get; set; }

                [DataMember(Name = "card_no")]
                public String card_no { get; set; }

                [DataMember(Name = "serial_no")]
                public String serial_no { get; set; }

                [DataMember(Name = "setting_no")]
                public char setting_no { get; set; }

                [DataMember(Name = "toothbrush_date")]
                public String toothbrush_date { get; set; }

                [DataMember(Name = "setting_contents")]
                public char setting_contents { get; set; }

                [DataMember(Name = "toothbrushes_type")]
                public char toothbrushes_type { get; set; }

                [DataMember(Name = "mouth_data")]
                public List<Mouth_Data> mouth_data { get; set; }

                [DataContract]
                public class Mouth_Data
                {
                    [DataMember(Name = "no")]
                    public int no { get; set; }

                    [DataMember(Name = "time")]
                    public int time { get; set; }

                    [DataMember(Name = "angle_time")]
                    public int angle_time { get; set; }

                    [DataMember(Name = "turn")]
                    public int turn { get; set; }

                    [DataMember(Name = "locus")]
                    public List<Locus> locus { get; set; }

                    [DataContract]
                    public class Locus
                    {
                        [DataMember(Name = "x")]
                        public int x { get; set; }

                        [DataMember(Name = "y")]
                        public int y { get; set; }
                    }
                }
            }
        }
     }
  }


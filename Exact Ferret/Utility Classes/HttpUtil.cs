using Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Http
{
    class HttpUtil
    {
        private class CustomWebClient : WebClient
        {
            public int Timeout { get; set; }

            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = (HttpWebRequest) base.GetWebRequest(address);
                request.Timeout = Timeout;

                request.GetResponse();

                if (!validateServerIdentity(request))
                {
                    Log.warning("Could not validate the server identity.");
                    return null;
                }

                return request;
            }
        }

        private class CertificatePair
        {
            public string issuer, thumbprint;
            public CertificatePair(string issuer, string thumbprint)
            {
                this.issuer = issuer;
                this.thumbprint = thumbprint.ToLower();
            }
        }

        private static string userAgent = null;
        private static string referer = null;
        private static Hashtable headers = new Hashtable();
        private static int httpRequestTimeout = 10;
        private static List<CertificatePair> certificateWhiteList = null;

        private static HttpStatusCode httpErrorCode = HttpStatusCode.OK;
        private static string httpErrorResponse = null;

        public static void trustAllCertificates()
        {
            certificateWhiteList = null;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        public static void trustCertificate(string issuerDN, string thumbprintHex)
        {
            // Needs to be called in order to trust any certificates at all
            trustAllCertificates();

            // Add the certificate to the current white list, create if no exist
            if (certificateWhiteList == null)
                certificateWhiteList = new List<CertificatePair>();
            certificateWhiteList.Add(new CertificatePair(issuerDN,
                    thumbprintHex.ToLower()));
        }

        public static void trustCertificates(string issuerDN, string thumbprintHex1, string thumbprintHex2)
        {
            // Needs to be called in order to trust any certificates at all
            trustAllCertificates();

            // Add the certificate to the current white list, create if no exist
            if (certificateWhiteList == null)
                certificateWhiteList = new List<CertificatePair>();
            certificateWhiteList.Add(new CertificatePair(issuerDN,
                    thumbprintHex1.ToLower()));
            certificateWhiteList.Add(new CertificatePair(issuerDN,
                    thumbprintHex2.ToLower()));
        }

        public static void setUserAgent(string userAgent)
        {
            HttpUtil.userAgent = userAgent;
        }

        public static void setHeader(string name, string value)
        {
            if (headers.ContainsKey(name))
                removeHeader(name);

            headers.Add(name, value);
        }

        public static void removeHeader(string name)
        {
            headers.Remove(name);
        }

        public static void removeAllCustomHeaders()
        {
            headers.Clear();
        }

        public static void setReferer(string referer)
        {
            HttpUtil.referer = referer;
        }

        public static void setRequestTimeoutSeconds(int seconds)
        {
            httpRequestTimeout = seconds;
        }

        public static HttpStatusCode getLastErrorCode()
        {
            return httpErrorCode;
        }

        public static string getLastErrorResponse()
        {
            return httpErrorResponse;
        }

        public static string get(string url)
        {
            Log.trace("Performing a GET request to " + url);

            try {
                Stream responseStream = getStream(url);
                if (responseStream == null)
                    return null;
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string responseText = reader.ReadToEnd();
                reader.Close();
                return responseText;
            }
            catch (Exception e)
            {
                Log.warning("Could not read the response stream from " + url);
                return null;
            }
        }

        public static Stream getStream(string url)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);

            Log.trace("Setting HTTP headers.");
            if (userAgent != null)
                request.UserAgent = userAgent;
            if (referer != null)
                request.Referer = referer;
            foreach (string name in headers.Keys) {
                string value = headers[name].ToString();
                request.Headers.Add(name, value);
            }

            Log.trace("Setting HTTP request timeout to " + httpRequestTimeout + " seconds.");
            request.Timeout = httpRequestTimeout * 1000;

            Log.trace("Sending an HTTP request to " + url);
            try
            {
                WebResponse response = request.GetResponse();

                if (!validateServerIdentity(request))
                {
                    Log.warning("Could not validate the server identity.");
                    return null;
                }

                Stream responseStream = response.GetResponseStream();
                return responseStream;
            }
            catch (WebException e)
            {
                HttpWebResponse errorResponse = (HttpWebResponse)e.Response;
                if (errorResponse != null)
                {
                    Stream responseStream = errorResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string errorText = reader.ReadToEnd();
                    Log.warning("Encountered an HTTP error: " + errorResponse.StatusCode);
                    httpErrorCode = errorResponse.StatusCode;
                    Log.special("The content of the error was: " + errorText, "RESULTS");
                    httpErrorResponse = errorText;
                }
                else
                {
                    Log.warning("Encountered an unknown HTTP error.");
                }
                return null;
            }
        }

        public static bool downloadFile(string url, string fileName)
        {
            Log.trace("Downloading file from " + url + " to location " + fileName);

            CustomWebClient client = new CustomWebClient();

            Log.trace("Setting HTTP headers.");
            if (userAgent != null)
                client.Headers["User-Agent"] = userAgent;
            if (referer != null)
                client.Headers["Referer"] = referer;
            foreach (string name in headers.Keys)
            {
                string value = headers[name].ToString();
                client.Headers.Add(name, value);
            }

            Log.trace("Setting HTTP request timeout to " + httpRequestTimeout + " seconds.");
            client.Timeout = httpRequestTimeout * 1000;

            Log.trace("Sending an HTTP request to " + url);
            try
            {
                client.DownloadFile(url, fileName);
            }
            catch (WebException e)
            {
                HttpWebResponse errorResponse = (HttpWebResponse)e.Response;
                if (errorResponse != null)
                {
                    Stream responseStream = errorResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string errorText = reader.ReadToEnd();
                    Log.warning("Encountered an HTTP error: " + errorResponse.StatusCode);
                    httpErrorCode = errorResponse.StatusCode;
                    Log.special("The content of the error was: " + errorText, "RESULTS");
                    httpErrorResponse = errorText;
                }
                else
                {
                    Log.warning("Encountered an unknown HTTP error.");
                }

                client.Dispose();
                return false;
            }

            client.Dispose();
            return true;
        }

        private static bool validateServerIdentity(HttpWebRequest request)
        {
            string urlString = request.RequestUri.ToString().ToLower();
            if (urlString.StartsWith("https://") && certificateWhiteList != null)
            {
                Log.trace("Validating the server's identity against a whitelist.");

                String issuerDN = request.ServicePoint.Certificate.Issuer;
                Log.trace("Server certificate issuer: " + issuerDN);
                String thumbprint = request.ServicePoint.Certificate.GetCertHashString().ToLower();
                Log.trace("Server certificate thumbprint: " + thumbprint);

                CertificatePair serverCertPair = new CertificatePair(issuerDN,
                        thumbprint);
                bool onWhiteList = false;
                foreach (CertificatePair pair in certificateWhiteList)
                {
                    if (pair.issuer.Equals(serverCertPair.issuer) &&
                        pair.thumbprint.Equals(serverCertPair.thumbprint))
                    {
                        onWhiteList = true;
                        break;
                    }
                }

                if (!onWhiteList)
                    return false;
            }

            return true;
        }
    }
}

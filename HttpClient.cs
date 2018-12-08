using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Efemerides
{
    /// <summary>
    /// Clase que se usara para obtener los las efemerides desde la wikipedia.
    /// </summary>
    public class HttpClient
    {
        // Creamos la propiedad de la clase WebClient
        private WebClient _Client;

        public Dictionary<String, String> FormData { get; set; }

        private string _url;

        // Establecemos un constructor en el que se isntanciara el objeto de la clase webclient
        
        public HttpClient()
        {
            _Client = new WebClient();
            _Client.Encoding = System.Text.Encoding.UTF8;
        }

        /// <summary>
        ///   Estableceremos las cabeceras que le mandaremos al protocolo HTTP
        /// </summary>
        private void SetHeaders()
        {
            _Client.Headers.Add("User-Agent: Mozilla / 5.0(Windows NT 10.0; WOW64; rv: 42.0) Gecko / 20100101 Firefox / 42.0");
            _Client.Headers.Add("Accept: */*");
            _Client.Headers.Add("Accept-Language: es-ES,es;q=0.8,en-US;q=0.5,en;q=0.3");
        }
        /// <summary>
        ///  Al hacer post, sabremos el origen y el destino de la pagina desde la que venimos y en la que estamos 
        ///  estableceremos las cabecceras y subiremos los datos para que el protoocolo http lo interprete.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public String Post(String url, String referer = "")
        {
            if (!String.IsNullOrEmpty(referer))
                _Client.Headers.Add($"Referer: {referer}");

            else if(!String.IsNullOrEmpty(_url))
            {
                _Client.Headers.Add($"Referer: {_url}");
            }
            
            SetHeaders();

            _Client.Headers.Add("Content-Type: application/x-www-form-urlencoded; charset=ASCII");
            _Client.Headers.Add("X-Requested-With: XMLHttpRequest");

            _url = url;

            return
                _Client.Encoding.GetString(
                    _Client.UploadData(url,
                    "POST",
                    SerializeForm()
               ));
        }
        /// <summary>
        /// TODO: Check if HTTPClient have automation way to validate certificates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private Boolean ValidateCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate
            certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        /// <summary>
        /// Obtendremos la url  de la cual descargaremos los datos. 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public String Get(String url, String referer = "")
        {
            if (!String.IsNullOrEmpty(referer))
                _Client.Headers.Add($"Referer: {referer}");
            else if (!string.IsNullOrEmpty(_url))
            {
                _Client.Headers.Add($"Referer: {_url}");
            }
            SetHeaders();
            _url = url;
            return _Client.Encoding.GetString(_Client.DownloadData(url));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private byte[] SerializeForm()
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            if (FormData == null)
                return _Client.Encoding.GetBytes("");
            try
            {
                foreach (var item in FormData?.Keys)
                {
                    sb.Append(item);
                    sb.Append("=");
                    sb.Append(FormData[item]);
                    if (count < FormData.Count)
                        sb.Append("&");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{FormData},{ex}");
            }

            return _Client.Encoding.GetBytes(sb.ToString());
        }
    }
}

using System;
using System.IO;
using System.Net;
using System.Text;

namespace MeltCalc
{
	public class RestClient
	{
		public RestClient(string endpoint)
			: this(endpoint, HttpVerb.GET, string.Empty)
		{
		}

		public RestClient(string endpoint, HttpVerb method)
			: this(endpoint, method, string.Empty)
		{
		}

		public RestClient(string endpoint, HttpVerb method, string postData)
		{
			EndPoint = endpoint;
			Method = method;
			ContentType = "application/json";
			PostData = postData;
		}

		public string ContentType { get; set; }

		public string EndPoint { get; set; }

		public HttpVerb Method { get; set; }

		public string PostData { get; set; }

		public string MakeRequest(string parameters = "")
		{
			var request = (HttpWebRequest) WebRequest.Create(EndPoint + parameters);

			request.Method = Method.ToString();
			request.ContentLength = 0;
			request.ContentType = ContentType;

			if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
			{
				var encoding = new UTF8Encoding();
				var bytes = encoding.GetBytes(PostData);
				request.ContentLength = bytes.Length;

				using (var writeStream = request.GetRequestStream())
				{
					writeStream.Write(bytes, 0, bytes.Length);
				}
			}

			using (var response = (HttpWebResponse) request.GetResponse())
			{
				var responseValue = string.Empty;

				if (response.StatusCode != HttpStatusCode.OK)
				{
					var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
					throw new ApplicationException(message);
				}

				using (var responseStream = response.GetResponseStream())
				{
					if (responseStream != null)
					{
						using (var reader = new StreamReader(responseStream))
						{
							responseValue = reader.ReadToEnd();
						}
					}
				}

				return responseValue;
			}
		}
	}
}
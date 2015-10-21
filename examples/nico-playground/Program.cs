namespace Splunk.Client.Examples
{
	using Splunk.Client.Helpers;
	using System;
	using System.Net;
	using System.Web;
	using System.Threading;
	using System.Threading.Tasks;
	using System.IO;
	using System.Net.Http;
	using System.Text;

	/// <summary>
	/// Starts a normal search and polls for completion to find out when the search has finished.
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			Run().Wait();
			Console.ReadKey();
		}

		static async Task Run()
		{

			using (StreamReader sr = new StreamReader(@"C:\Temp\1tag.xml"))
			{
				String payload = sr.ReadToEnd();

				HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
				message.Content = new StringContent(payload);

				// NH
				Response.StreamResultEncodingOverride = Encoding.GetEncoding("ISO-8859-9");

				Response response = await Response.CreateAsync(message).ConfigureAwait(false);

				using (SearchResultStream srStream = await SearchResultStream.CreateAsync(response).ConfigureAwait(false))
				{
					long count = 0;
		
					try
					{

						foreach (SearchResult result in srStream)
						{


							count++;


						}
					}
					catch (Exception e)
					{

					}
					finally
					{
						Console.WriteLine(String.Format("SearchCount process: {0}", count));
					}
				}

			}

		}
	}
}

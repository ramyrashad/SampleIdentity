using Microsoft.AspNetCore;

namespace SampleIdentity.WebAPI
{
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            webHost.Run();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
              WebHost.CreateDefaultBuilder(args)
             .UseSetting("detailedErrors", "true")
             .UseStartup<Startup>()
             .CaptureStartupErrors(true);

        #region Private Methods


        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Cireson.Core.DataAccess.Interfaces;
using Cireson.Core.Services.WorkerBaseClasses;
using MyDevPalOData.Models;
using Newtonsoft.Json;
using RestSharp;

namespace MyDevPalOData.Workers
{
    public class FocusWorker : ProcessWorkerBase
    {
        public override bool AllowMultipleInstances { get; set; } = false;

        private int _mailCount = 0;
        private Timer _timer;
        private readonly IRepository _repository;
        private readonly IRestClient _client;

        public FocusWorker(IRepository repository)
        {
            _repository = repository;
            _client = new RestClient("https://graph.microsoft.com/v1.0");
        }

        public override Task ProcessStartingAsync()
        {
            if (_timer == null)
            {
                _timer = new Timer(30000); // Default every minute
            }

            _timer.AutoReset = true;
            _timer.Elapsed += Process;

            Process(null, null);

            _timer.Start();

            return base.ProcessStartingAsync();
        }

        private void Process(object sender, ElapsedEventArgs e)
        {
            // Halt timer while running
            _timer.Stop();
            
            try
            {
                var bearer = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJodHRwczovL2dyYXBoLm1pY3Jvc29mdC5jb20vIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvMGY0NTRkMmYtOTFiNi00MDEyLThkMjAtZDFmZWE2ZDE3NGI5LyIsImlhdCI6MTQ1OTYzODQzNywibmJmIjoxNDU5NjM4NDM3LCJleHAiOjE0NTk2NDIzMzcsImFjciI6IjEiLCJhbXIiOlsicHdkIl0sImFwcGlkIjoiNGQ3MmFlZDctOTQyYy00ZWQ3LTg0MzgtYTZhODFkZDZjMWRjIiwiYXBwaWRhY3IiOiIxIiwiZmFtaWx5X25hbWUiOiJKb3JkYW4iLCJnaXZlbl9uYW1lIjoiS2F0aWUiLCJpcGFkZHIiOiIxNjcuMjIwLjI1LjExMyIsIm5hbWUiOiJLYXRpZSBKb3JkYW4iLCJvaWQiOiI1MzQ5Y2UwOC05ZDkwLTQzZTQtODkwOS1iOTZmYzBhYTE5N2QiLCJwdWlkIjoiMTAwMzAwMDA5NkM4RTk4QyIsInNjcCI6Ik1haWwuUmVhZCBVc2VyLlJlYWQiLCJzdWIiOiI2U3JLVjBMUDc3blc4al9HbGtCbUZsZEJYZ2ZsY3pMdnRyeEhVdVN5bm44IiwidGlkIjoiMGY0NTRkMmYtOTFiNi00MDEyLThkMjAtZDFmZWE2ZDE3NGI5IiwidW5pcXVlX25hbWUiOiJLYXRpZUpAQnVpbGREZW1vNjkyLm9ubWljcm9zb2Z0LmNvbSIsInVwbiI6IkthdGllSkBCdWlsZERlbW82OTIub25taWNyb3NvZnQuY29tIiwidmVyIjoiMS4wIn0.SxmtSIaR7qK3Qg-iWY6YeXQ-sUzaC0LyPSguVT1l8ABLjg54TLNFsevVhXz5LkpNgsrJmSG54OW_uYZf5W-chcRFBZj-wH3WOrlZr2X33E_-KwFc6NU50py0pL2h1XJrjJtINXtxpzQEEyFFonD3REkxDpqGyChn0Z6RtjvAV6kIF9kUmCM-U3SleHmQYkt9ObSUZiKT_ZKp2C3lhi-tuLzfjtWM-R38z188-EBWH5aqjwLYBn6hIluPdEZH6X-IflnmJ6IZZktR7rqN4S2IHxvcl07N1xSwT1E9krWxNwXjS0BQOW0NrJJY1ssBWV-DmstqLuZVndhcyiEc7DHZKQ";
                var request = new RestRequest("/me/messages", Method.GET);
                request.AddHeader("Authorization", $"Bearer {bearer}");

                var response = _client.Execute(request);
                var jsonContent = response.Content;
                var obj = JsonConvert.DeserializeObject<Payload>(jsonContent);
                var count = obj.value.Count;
                var focus = Math.Abs(_mailCount - count);

                _mailCount = count; // reset counter

                if (focus == 0) focus = -1;

                //
                var email = "lance.wynn@cireson.com";
                var devProfile = _repository
                    .Get<DevProfile>()
                    .FirstOrDefault(d => d.Email == email);

                if (devProfile == null)
                {
                    devProfile = new DevProfile { Email = email };
                    _repository.Add(devProfile);
                }

                var devStat = new DevStatAlteration
                {
                    FocusDelta = focus,
                    Mood = devProfile.CurrentMood,
                    Vitality = devProfile.CurrentVitality,
                    Focus = devProfile.CurrentFocus,
                    Source = "Outlook",
                    Message = focus > 0 ? "Looks like you're working on some emails!" : "No email activity detected"
                };
                
                devProfile.CurrentFocus += devStat.FocusDelta;

                devProfile.DevStatAlterationScores = new List<DevStatAlteration> { devStat };

                _repository.SaveChanges();
            }
            catch (Exception)
            {
                // Ignore
            }
            finally
            {
                // Resume timer now that work has finished
                _timer.Start();
            }
        }

        public override Task ProcessStoppingAsync()
        {
            _timer.Stop();

            return base.ProcessStoppingAsync();
        }
    }

    public class Payload
    {
        public List<Object> value { get; set; }
    }
}
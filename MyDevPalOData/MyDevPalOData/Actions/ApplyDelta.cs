using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Cireson.Core.Common.Attributes;
using Cireson.Core.Common.Extensions;
using Cireson.Core.DataAccess.Interfaces;
using Cireson.Core.Interfaces.ProviderInterfaces;
using Cireson.Core.Services.Actions;
using Cireson.Core.Services.MessageBroker;
using MyDevPalOData.Models;

namespace MyDevPalOData.Actions
{
    /// <summary>
    /// Class supports an Action that is unbound to specific instances of entities.
    /// </summary>
    /// <example>
    /// http://localhost/api/MyUnboundAction1
    /// </example>
    [ODataAlias]
    [AllowAnonymous]
    public class ApplyDelta : UnboundAction<bool>
    {
        private readonly IRepository _repository;

        public ApplyDelta(IRepository repository)
        {
            _repository = repository;
           
        }

        /// <summary>
        /// Asynchronously executes an action.  This is akin to post operation.
        /// </summary>
        /// <returns>Returns the execution result.</returns>
        public override async Task<bool> ExecuteAsync()
        {  
            var devProfile = await _repository.Get<DevProfile>()
                .Where(d => d.Email == Email)
                .FirstOrDefaultAsync();
            
            if (devProfile == null)
            {
                devProfile = new DevProfile
                {
                    Email = Email,
                };
                _repository.Add(devProfile);
            }

            var devStat = new DevStatAlteration
            {
                MoodDelta = MoodDelta,
                VitalityDelta = VitalityDelta,
                FocusDelta = FocusDelta,
                Mood=devProfile.CurrentMood,
                Vitality = devProfile.CurrentVitality,
                Focus = devProfile.CurrentFocus,
                Source = Source,
                Message = Message
            };

            devProfile.CurrentMood += devStat.MoodDelta;
            devProfile.CurrentVitality += devStat.VitalityDelta;
            devProfile.CurrentFocus += devStat.FocusDelta;

            devProfile.DevStatAlterationScores = new List<DevStatAlteration> {devStat};
            //devProfile.DevStatAlterationScores.Add(devStat);
            
            await _repository.SaveChangesAsync();

            return true;
        }

        public string Email { get; set; }
        public int MoodDelta { get; set; }
        public int VitalityDelta { get; set; }
        public int FocusDelta { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
    }
}
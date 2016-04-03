using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Cireson.Core.Common.Attributes;
using Cireson.Core.DataAccess.Interfaces;
using Cireson.Core.Services.Actions;
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
    public class ClearStats : UnboundAction<DevProfile>
    {
        private readonly IRepository _repository;

        public ClearStats(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Asynchronously executes an action.  This is akin to post operation.
        /// </summary>
        /// <returns>Returns the execution result.</returns>
        public override async Task<DevProfile> ExecuteAsync()
        {
            // Replace with your awaited code here.
            var devProfile = await _repository.Get<DevProfile>()
                .Where(d => d.Email == Email)
                .FirstOrDefaultAsync();

            devProfile.CurrentFocus = 0;
            devProfile.CurrentMood = 0;
            devProfile.CurrentVitality = 0;
            await _repository.SaveChangesAsync();
            return devProfile;
        }

        public string Email { get; set; }
    }
}
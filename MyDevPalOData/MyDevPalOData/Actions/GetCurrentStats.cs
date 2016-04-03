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
    /// Class supports a Function that is unbound to specific instances of entities.
    /// </summary>
    /// <example>
    /// http://localhost/api/MyUnboundFunction1
    /// </example>
    [ODataAlias]
    [AllowAnonymous]
    public class GetCurrentStats : UnboundFunction<DevProfile>
    {
        private readonly IRepository _repository;

        public GetCurrentStats(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Asynchronously executes a function.  This is akin to get operation.
        /// </summary>
        /// <returns>Returns the execution result.</returns>
        public override async Task<DevProfile> ExecuteAsync()
        {
            // Replace with your awaited code here.
            // Replace with your awaited code here.
            var devProfile = await _repository.Get<DevProfile>()
                .Where(d => d.Email == Email)
                .FirstOrDefaultAsync();
            return devProfile;
        }

        public string Email { get; set; }
    }
}

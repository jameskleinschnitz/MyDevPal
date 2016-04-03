using System;
using Cireson.Core.Interfaces.DataAccess;
using Cireson.Core.DataAccess.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Http;
using Cireson.Core.Common.Attributes;
using Cireson.Core.DataAccess.Models;

namespace MyDevPalOData.Models
{
    /// <summary>
    /// Base Implementation of a Cireson Platform Entity model.
    /// </summary>
    [ODataAlias]
    [AllowAnonymous]
    [AuthorizeOData("Post", "PUT", "PATCH", "DELETE")]
    public partial class DevStatAlteration : IPlatformEntity
    {
        public DevProfile DevProfile { get; set; }
        [ForeignKey("DevProfile")]

        public long DevProfileId { get; set; }
        
        public int MoodDelta { get; set; }

        public int VitalityDelta { get; set; }

        public int FocusDelta { get; set; }
        
        public int Mood { get; set; }

        public int Vitality { get; set; }

        public int Focus { get; set; }

        public string Source { get; set; }

        public string Message { get; set; }

        #region IPlatformEntity

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [AlwaysAllowPropertyRead]
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        /// <value>The modified date.</value>
        [Column(Order = 2)]
        [AlwaysAllowPropertyRead]
        public virtual DateTimeOffset? ModifiedDate { get; set; }


        /// <summary>
        /// Gets or sets the modified by identifier.
        /// </summary>
        /// <value>The modified by identifier.</value>
        [Column(Order = 3)]
        public virtual long? ModifiedById { get; set; }


        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        [Column(Order = 4)]
        public virtual DateTimeOffset? CreatedDate { get; set; }


        /// <summary>
        /// Gets or sets the created by identifier.
        /// </summary>
        /// <value>The created by identifier.</value>
        [Column(Order = 5)]
        public virtual long? CreatedById { get; set; }


        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>The unique identifier.</value>
        [Column(Order = 6)]
        public virtual Guid? Guid { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value><c>true</c> if this instance is deleted; otherwise, <c>false</c>.</value>
        [Column(Order = 7)]
        public virtual bool IsDeleted { get; set; }

        #endregion  // IPlatformEntity
    }
}

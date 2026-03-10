/*
Copyright © Upendo Ventures, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES 
OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Upendo.Modules.UserManager.Models.DnnModel
{
    /// <summary>
    /// Represents the association between users and roles within the DNN (DotNetNuke) framework.
    /// </summary>
    /// <remarks>
    /// The <see cref="UserRoles"/> class defines the relationship between users and their assigned roles.
    /// It includes details such as role assignment dates, status, and ownership information.
    /// This class is part of the DNN framework's data model and is used to manage user role assignments.
    /// </remarks>
    public partial class UserRoles
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user role.
        /// </summary>
        /// <remarks>
        /// This property represents the primary key for the <see cref="UserRoles"/> entity.
        /// It is used to uniquely identify a specific user role within the DNN framework.
        /// </remarks>
        [Key]
        public int UserRoleId { get; set; }
        
        /// <summary>
        /// Gets or sets the unique identifier of the user associated with the role.
        /// </summary>
        /// <remarks>
        /// This property establishes a relationship between the <see cref="UserRoles"/> entity and the <see cref="Users"/> entity.
        /// It represents the user to whom the role is assigned within the DNN (DotNetNuke) framework.
        /// </remarks>
        public int UserId { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the role associated with the user.
        /// </summary>
        /// <remarks>
        /// This property represents the unique identifier for a role within the DNN framework.
        /// It is used to establish the relationship between a user and a specific role.
        /// </remarks>
        public int RoleId { get; set; }
        
        /// <summary>
        /// Gets or sets the expiry date of the user's role.
        /// </summary>
        /// <remarks>
        /// This property indicates the date and time when the user's role assignment will expire. 
        /// A <c>null</c> value signifies that the role does not have an expiration date.
        /// </remarks>
        public DateTime? ExpiryDate { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the trial period for the role has been used.
        /// </summary>
        /// <value>
        /// A nullable boolean value where <c>true</c> indicates that the trial period has been used,
        /// <c>false</c> indicates it has not been used, and <c>null</c> indicates an unknown state.
        /// </value>
        /// <remarks>
        /// This property is used to track the usage of trial periods for roles associated with a user.
        /// </remarks>
        public bool? IsTrialUsed { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the user role becomes effective.
        /// </summary>
        /// <remarks>
        /// This property represents the starting date of the user's role assignment.
        /// It can be <c>null</c> if no specific effective date is set.
        /// </remarks>
        public DateTime? EffectiveDate { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the user who created this user role.
        /// </summary>
        /// <remarks>
        /// This property holds the ID of the user responsible for creating the associated user role.
        /// It is nullable to accommodate scenarios where the creator's information might not be available.
        /// </remarks>
        public int? CreatedByUserId { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time when the user role was created.
        /// </summary>
        /// <remarks>
        /// This property represents the timestamp of when the user role entry was created in the system.
        /// It is nullable to account for scenarios where the creation date might not be recorded.
        /// </remarks>
        public DateTime? CreatedOnDate { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier of the user who last modified this user role.
        /// </summary>
        /// <remarks>
        /// This property holds the user ID of the individual who made the most recent changes
        /// to the <see cref="UserRoles"/> entity. It is nullable to account for scenarios where
        /// no modifications have been made or the information is unavailable.
        /// </remarks>
        public int? LastModifiedByUserId { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time when the user role was last modified.
        /// </summary>
        /// <remarks>
        /// This property indicates the most recent modification timestamp for the user role.
        /// It is nullable to account for scenarios where no modifications have been made.
        /// </remarks>
        public DateTime? LastModifiedOnDate { get; set; }
        
        /// <summary>
        /// Gets or sets the status of the user role.
        /// </summary>
        /// <remarks>
        /// This property represents the current state or condition of the user role within the system.
        /// It may be used to indicate whether the role is active, inactive, pending, or in another state.
        /// </remarks>
        public int Status { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the user associated with this role is the owner.
        /// </summary>
        /// <value>
        /// <c>true</c> if the user is the owner; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This property is used to determine ownership status within the context of the associated role.
        /// </remarks>
        public bool IsOwner { get; set; }

        /// <summary>
        /// Gets or sets the role associated with the user role.
        /// </summary>
        /// <remarks>
        /// This property represents the relationship between a user role and its corresponding role entity.
        /// It provides access to detailed information about the role, such as its name, description, and other attributes.
        /// </remarks>
        public virtual Roles Role { get; set; }
        
        /// <summary>
        /// Gets or sets the user associated with this user role.
        /// </summary>
        /// <remarks>
        /// This property establishes a relationship between the <see cref="UserRoles"/> entity
        /// and the <see cref="Users"/> entity, representing the user to whom this role is assigned.
        /// </remarks>
        public virtual Users User { get; set; }
    }
}

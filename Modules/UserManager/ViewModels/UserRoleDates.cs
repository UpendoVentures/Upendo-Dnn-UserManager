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

namespace Upendo.Modules.UserManager.ViewModels
{
    /// <summary>
    /// Represents a view model for managing user role dates, including the effective and expiry dates of a role.
    /// </summary>
    public class UserRoleDates
    {
        /// <summary>
        /// Gets or sets the date and time when the user role becomes effective.
        /// </summary>
        /// <value>
        /// A <see cref="DateTime"/> representing the effective date and time of the user role.
        /// </value>
        public DateTime EffectiveDate { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time when the user's role expires.
        /// </summary>
        /// <value>
        /// A <see cref="DateTime"/> representing the expiration date and time of the user's role.
        /// </value>
        public DateTime ExpiryDate { get; set; }
    }
}
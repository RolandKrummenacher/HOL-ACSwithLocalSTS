// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

//-----------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//
//-----------------------------------------------------------------------------

using System;
using Microsoft.IdentityModel.Protocols.OAuth;

namespace CustomersService
{
    /// <summary>
    /// This exception is thrown when there is an error validating the incoming token.
    /// </summary>
    public class InvalidTokenReceivedException : Exception
    {
        static string _errorCode = OAuthConstants.ErrorCode.InvalidToken;
        
        string _errorDescription;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTokenReceivedException"/> class.
        /// </summary>
        /// <param name="errorDescription">A description of the error.</param>
        public InvalidTokenReceivedException( string errorDescription )
            : base()
        {
            _errorDescription = errorDescription;
        }

        /// <summary>
        /// Gets the OAuth error code corresponding to this exception.
        /// </summary>
        /// <value>The OAuth error code.</value>
        public string ErrorCode
        {
            get { return _errorCode; }
        }

        /// <summary>
        /// Gets the description of the error which caused this exception.
        /// </summary>
        /// <value>A description of the error that occured.</value>
        public string ErrorDescription
        {
            get { return _errorDescription; }
        }
    }
}
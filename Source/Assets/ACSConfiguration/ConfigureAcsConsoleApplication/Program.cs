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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACSConfigurationTool
{   
    class Program
    {
        //
        // Relying Party Configuration.
        //
        private const string RelyingPartyApplicationName = "Customers Service";

        // Client Configuration
        private const string ClientIdentity = "FabrikamClient";

        //
        // The Uri the client is redirected to after user authentication.
        //
        private const string RedirectUrlAfterEndUserConsent = "https://localhost/FabrikamWebSite/OAuthHandler.ashx";

        //
        // Identity Provider Configuration.
        //
        public const string IdentityProvider = "https://sampleidentityprovider";

        public const string AcsHostUrl = "accesscontrol.windows.net";
        public const string AcsManagementServicesRelativeUrl = "v2/mgmt/service/";

        public const string ManagementServiceIdentityName = "ManagementClient";

        static void Main(string[] args)
        {
            Console.Title = "ACS Configuration Tool";

            PrintStartupMessage();

            // Prompt for ACS data
            Console.WriteLine("Enter your service namespace (eg: foo)");
            string serviceNamespace = Console.ReadLine();

            Console.WriteLine("Enter your Management Service Identity Key (eg: A3THQ1Cgj56ZuZXjcWTAL266lftW2+9tJB7HLWXmNYY=)");
            string managementServiceIdentityKey = Console.ReadLine();

            // create helper
            ManagementServiceHelper managementHelper = new ManagementServiceHelper(serviceNamespace, AcsHostUrl, AcsManagementServicesRelativeUrl, ManagementServiceIdentityName, managementServiceIdentityKey);

            // Step 1
            Console.WriteLine();
            Console.WriteLine("1 - Updating Service Identity Redirect Address ...");
            managementHelper.UpdateServiceIdentityRedirectAddress(ClientIdentity, RedirectUrlAfterEndUserConsent);
            Console.WriteLine("Done!");

            // Step 2
            Console.WriteLine();
            Console.WriteLine("2 - Updating RelyingParty rules");
            managementHelper.UpdateRelyingPartyRule(RelyingPartyApplicationName, IdentityProvider);
            Console.WriteLine("Done!");
                       
            // Process completed       
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void PrintStartupMessage()
        {
            Console.WriteLine("ACS Configuration Tool");
            Console.WriteLine("----------------------");
            Console.WriteLine();
            Console.WriteLine("This tool will perform the following tasks:");
            Console.WriteLine("\t 1- Set the Redirect Address of the '{0}' Service Identity to '{1}'", ClientIdentity, RedirectUrlAfterEndUserConsent);
            Console.WriteLine("\t 2- Update '{0}' RelyingParty rules", RelyingPartyApplicationName);
            Console.WriteLine();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}

To run the end solution of this exercise the following steps are required:

1. Execute all the steps performed to the Windows Azure Portal. 
2. Delete the WebSiteAdvancedACS Virtual directory from IIS.
2. Replace the following Place Holders:
	-  {yourServiceNamespace} with the service namespace you created.
	-  {yourManagementServiceKey} with the password of your management service.
3. Add STS Reference to the WebSiteAdvancedACS project. For the Application URI field use the following URL: 'https://localhost/WebSiteAdvancedACS'.
4. Run an instance of the IdentityProviderSetup project.
5. Execute and start SelfSTS1 and SelfSTS2	 

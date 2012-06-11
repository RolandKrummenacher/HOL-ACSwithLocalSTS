@ECHO OFF
ECHO Enabling Load User Profile for the Application Pool...

%windir%\system32\inetsrv\appcmd set config -section:applicationPools /[name='DefaultAppPool'].processModel.loadUserProfile:true

%windir%\system32\inetsrv\appcmd set config -section:applicationPools /[name='"ASP.NET v4.0"'].processModel.loadUserProfile:true

iisreset
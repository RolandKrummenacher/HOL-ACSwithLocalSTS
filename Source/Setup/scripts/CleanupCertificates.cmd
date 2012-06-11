@echo off

echo Removing certificates...
echo.

certutil -delstore My "localhost"

certutil -delstore Root "Root Agency"

certutil -delstore My "IdentityTKStsCert"

certutil -delstore TrustedPeople "IdentityTKStsCert"

echo.
echo Clean up finished!
echo.
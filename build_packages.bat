mkdir %~dp0Packages
..\PackageManager\EleWise.ELMA.Packaging.Console.exe packSln %~dp0WebApplication\ELMA.WebApplication.sln -v 1.0.0.1000 -out %~dp0Packages -NoSelfUpdate
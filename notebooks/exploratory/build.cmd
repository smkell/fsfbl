@echo off
cls

if "%1" == "quickrun" (
  ..\..\packages\FAKE\tools\FAKE.exe run --fsiargs -d:NO_FSI_ADDPRINTER build.fsx
) else (
  ..\..\..\fsfbl\.paket\paket.bootstrapper.exe
  if errorlevel 1 (
    exit /b %errorlevel%
  )
  if not exist paket.lock (
    ..\..\..\fsfbl\.paket\paket.exe install
  ) else (
    ..\..\..\fsfbl\.paket\paket.exe restore
  )
  if errorlevel 1 (
    exit /b %errorlevel%
  )
  ..\..\packages\FAKE\tools\FAKE.exe %* --fsiargs -d:NO_FSI_ADDPRINTER build.fsx
)

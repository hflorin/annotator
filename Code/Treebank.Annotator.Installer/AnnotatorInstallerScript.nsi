; AnnotatorInstallerScript.nsi
;
; This script is used to generate the Treebank Annotator installer application based on the NSIS framework

;--------------------------------

; The name of the installer
Name "Treebank Annotator"

; The file to write
OutFile "TreebankAnnotatorInstaller.exe"

LoadLanguageFile "${NSISDIR}\Contrib\Language files\English.nlf"
;--------------------------------
;Version Information

; VIProductVersion "1.0.0"
 ; VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "Treebank Annotator"
;  VIAddVersionKey /LANG=${LANG_ENGLISH} "Comments" "Treebank Annotator Application"
;  VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "Alexandru Ioan Cuza Universitym Faculty of Computer Science"
 ; VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalTrademarks" "Treebank Annotator Application"
;  VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "Copyright"
 ; VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "Treebank Annotator Application"
; VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "1.0.0"

;--------------------------------


; The default installation directory
InstallDir $PROGRAMFILES\TreebankAnnotator

; Registry key to check for directory (so if you install again, it will
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\TreebankAnnotator" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "TreebankAnnotator Application (required)"

  SectionIn RO

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR\Application

  ; Copy application files there
  File /nonfatal /a /r "..\Treebank.Annotator\bin\Release\"

  ;Copy configurations files
  SetOutPath $INSTDIR\Configurations
  File /nonfatal /a /r "..\..\Resources\Configurations\New\"

   ;Copy samples files
  SetOutPath $INSTDIR\Samples
  File /nonfatal /a /r "..\..\Resources\Treebanks\Samples\"


  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\TreebankAnnotator "Install_Dir" "$INSTDIR"

  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TreebankAnnotator" "DisplayName" "TreebankAnnotator"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TreebankAnnotator" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TreebankAnnotator" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TreebankAnnotator" "NoRepair" 1
  WriteUninstaller "$INSTDIR\uninstall.exe"

SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\TreebankAnnotator"
  CreateShortcut "$SMPROGRAMS\TreebankAnnotator\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortcut "$SMPROGRAMS\TreebankAnnotator\TreebankAnnotator.lnk" "$INSTDIR\Application\Treebank.Annotator.exe" "" "$INSTDIR\Application\Treebank.Annotator.exe" 0

SectionEnd



; Uninstaller

Section "Uninstall"

  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TreebankAnnotator"
  DeleteRegKey HKLM SOFTWARE\TreebankAnnotator

  ; Remove files and uninstaller
  Delete $INSTDIR\Application
  Delete $INSTDIR\Configurations
  Delete $INSTDIR\Samples
  Delete $INSTDIR\uninstall.exe

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\TreebankAnnotator\*.*"

  ; Remove directories used
  RMDir /r "$SMPROGRAMS\TreebankAnnotator"
  RMDir /r "$INSTDIR"

SectionEnd


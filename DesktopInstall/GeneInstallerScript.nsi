

OutFile "Gene Installer.exe"
InstallDir $PROGRAMFILES32\Gene
RequestExecutionLevel admin

Section "Install"

    SetOutPath $INSTDIR
    WriteUninstaller "$INSTDIR\Uninstaller.exe"

    CreateDirectory "$SMPROGRAMS\Gene"
    CreateShortCut "$SMPROGRAMS\Gene\Gene.lnk" "$INSTDIR\GeneA.Desktop.exe"

    File /r /x *.pdb "..\GeneA.Desktop\bin\Release\net8.0\*" 

    # Write uninstall information to the registry
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gene" "DisplayName" "Gene"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gene" "UninstallString" "$INSTDIR\Uninstaller.exe"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gene" "InstallLocation" "$INSTDIR"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gene" "DisplayIcon" "$INSTDIR\GeneA.Desktop.exe"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gene" "Publisher" "My Company"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gene" "DisplayVersion" "1.0"
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gene" "NoModify" 1
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gene" "NoRepair" 1


SectionEnd


Section "Uninstall"
    Delete "$SMPROGRAMS\Gene\Gene.lnk"
    RMDir "$SMPROGRAMS\Gene"

    RMDir /r $INSTDIR

    # Remove uninstall information from the registry
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gene"

SectionEnd
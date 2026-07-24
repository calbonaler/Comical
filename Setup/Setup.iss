#define DotNetMajor 8
#define DotNetMinor 0
#dim DotNetFrameworks[2] { "Microsoft.NETCore.App", "Microsoft.WindowsDesktop.App" }
#define AppName "Comical"
#define AppPublisher "calbonaler"
#define AppFileName AppName + ".exe"
#define AppProjectDir "..\" + AppName
#define AppResourcesDir AppProjectDir + "\Resources"
#define AppOutputDir AppProjectDir + "\bin\Release\net" + Str(DotNetMajor) + "." + Str(DotNetMinor) + "-windows7.0"
#define AppFilePath AppOutputDir + "\" + AppFileName
#define AppDefaultProgId "Comical.ImageCollection.1"
#define AppDefaultProgIdDescription "Comical Image Collection File"
#define AppDefaultExt ".cic"
#define ShellExtName "ComicFileHandler"
#define ShellExtFileName ShellExtName + ".dll"
#define ShellExtProjectDir "..\" + ShellExtName
#define ShellExtOutputDir ShellExtProjectDir + "\bin\x64\Release"
#define ShellExtFilePath ShellExtOutputDir + "\" + ShellExtFileName
#define ShellExtPropertyHandlerClsid "001823E8-247E-4685-BD84-350347B0460C"
#define ShellExtPropertyHandlerDescription "Comic Property Handler"
#define ShellExtThumbnailProviderClsid "4423CDF9-0C1B-4F23-8CC4-BA634252CD6A"
#define ShellExtThumbnailProviderDescription "Comic Thumbnail Provider"
#define SetupResourcesDir "Resources"

#define GetVersionStringWithoutBuild(str filename) \
  GetVersionComponents(filename, Local[0], Local[1], Local[2], Local[3]), \
  Str(Local[0]) + "." + Str(Local[1]) + "." + Str(Local[2])
#define AppVersion GetVersionStringWithoutBuild(AppFilePath)

[Setup]
; アプリケーション情報
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
UninstallDisplayName={#AppName}
UninstallDisplayIcon={app}\{#AppFileName}
; アプリケーション要件
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
; インストール先
DefaultDirName={autopf}\{#AppPublisher}\{#AppName}
; UI/ウィザード
DisableWelcomePage=no
DisableDirPage=yes
DisableProgramGroupPage=yes
DisableReadyPage=yes
SetupIconFile={#AppResourcesDir}\{#AppName}.ico
WizardImageFile={#SetupResourcesDir}\WizardLarge100.png,{#SetupResourcesDir}\WizardLarge150.png,{#SetupResourcesDir}\WizardLarge200.png
WizardSmallImageFile={#SetupResourcesDir}\WizardSmall100.png,{#SetupResourcesDir}\WizardSmall200.png
; ビルド出力
OutputDir=bin
OutputBaseFilename={#AppName}-{#AppVersion}-Setup
VersionInfoVersion={#AppVersion}

[Languages]
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"

[CustomMessages]
japanese.DotNetNotInstalled=%1 をインストールするには .NET デスクトップ ランタイム %2.%3 以上が必要です。「OK」をクリックすると、ダウンロードページを開きます。
japanese.DotNetRuntimeUrl=https://dotnet.microsoft.com/ja-jp/download/dotnet/%1.%2/runtime
japanese.NextPure=「次へ」
japanese.InstallPure=「インストール」

[Files]
Source: "{#AppOutputDir}\*"; Excludes: "*.pdb"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
Source: "{#ShellExtFilePath}"; DestDir: "{app}"; Flags: ignoreversion restartreplace uninsrestartdelete

[Registry]
#define AppDefaultProgIdKey "HKA; Subkey: SOFTWARE\Classes\" + AppDefaultProgId

; ----------------------------------------------------------------
; アプリケーション用の基本設定
; ----------------------------------------------------------------
; ProgID登録
Root: {#AppDefaultProgIdKey}; ValueType: string; ValueData: {#AppDefaultProgIdDescription}; Flags: uninsdeletekey
Root: {#AppDefaultProgIdKey}; ValueType: string; ValueName: FriendlyTypeName; ValueData: "@""{app}\{#ShellExtFileName}"",-101"; Flags: uninsdeletevalue
Root: {#AppDefaultProgIdKey}\DefaultIcon; ValueType: string; ValueData: {app}\{#AppFileName},0; Flags: uninsdeletekey
Root: {#AppDefaultProgIdKey}\shell\open\command; ValueType: string; ValueData: """{app}\{#AppFileName}"" /View ""%1"""; Flags: uninsdeletekey
Root: {#AppDefaultProgIdKey}\shell\edit\command; ValueType: string; ValueData: """{app}\{#AppFileName}"" ""%1"""; Flags: uninsdeletekey
; 拡張子とProgIDの関連付け設定
Root: HKA; Subkey: SOFTWARE\Classes\{#AppDefaultExt}; ValueType: string; ValueData: {#AppDefaultProgId}; Flags: uninsdeletekey

; ----------------------------------------------------------------
; シェル拡張登録
; ----------------------------------------------------------------
#define ClassRegistration(str clsid, str description) \
  "Root: HKA; SubKey: SOFTWARE\Classes\CLSID\{{" + clsid + "}; ValueType: string; ValueData: " + description + "; Flags: uninsdeletekey" + NewLine + \
  "Root: HKA; SubKey: SOFTWARE\Classes\CLSID\{{" + clsid + "}\InprocServer32; ValueType: string; ValueData: {app}\" + ShellExtFileName + "; Flags: uninsdeletekey" + NewLine + \
  "Root: HKA; SubKey: SOFTWARE\Classes\CLSID\{{" + clsid + "}\InprocServer32; ValueType: string; ValueName: ThreadingModel; ValueData: Apartment; Flags: uninsdeletevalue"

; Property Handler登録
; ----------------------------------------------------------------
; クラス登録
#emit ClassRegistration(ShellExtPropertyHandlerClsid, ShellExtPropertyHandlerDescription)
; シェルに表示するプロパティ項目のカスタマイズ
Root: {#AppDefaultProgIdKey}; ValueType: string; ValueName: InfoTip; \
  ValueData: "prop:System.ItemType;System.Author;System.Size;System.DateModified"; \
  Flags: uninsdeletevalue
Root: {#AppDefaultProgIdKey}; ValueType: string; ValueName: FullDetails; \
  ValueData: \
    "{#"prop:System.PropGroup.Description;System.Title;System.Author;System.Document.DateCreated;System.Keywords;System.FileVersion;" + \
    "System.PropGroup.FileSystem;System.ItemNameDisplay;System.ItemType;System.ItemFolderPathDisplay;System.Size;System.DateCreated;System.DateModified;" + \
    "System.DateAccessed;System.FileAttributes;System.OfflineAvailability;System.OfflineStatus;System.SharedWith;System.FileOwner;System.ComputerName"}"; \
  Flags: uninsdeletevalue
Root: {#AppDefaultProgIdKey}; ValueType: string; ValueName: PreviewDetails; \
  ValueData: \
    "{#"prop:System.Title;System.Author;System.Document.DateCreated;System.Keywords;System.DateModified;System.Size;System.FileVersion;System.DateCreated;" + \
    "System.SharedWith"}"; \
  Flags: uninsdeletevalue
Root: {#AppDefaultProgIdKey}; ValueType: string; ValueName: ContentViewModeForBrowse; \
  ValueData: "prop:~System.ItemNameDisplay;~System.Author;System.FileVersion;~System.LayoutPattern.PlaceHolder;System.Document.DateCreated;System.Size"; \
  Flags: uninsdeletevalue
Root: {#AppDefaultProgIdKey}; ValueType: string; ValueName: ContentViewModeLayoutPatternForBrowse; ValueData: delta; Flags: uninsdeletevalue
Root: {#AppDefaultProgIdKey}; ValueType: string; ValueName: ContentViewModeForSearch; \
  ValueData: "prop:~System.ItemNameDisplay;System.Document.DateCreated;~System.ItemFolderPathDisplay;~System.Keywords;System.Size;System.Author"; \
  Flags: uninsdeletevalue
Root: {#AppDefaultProgIdKey}; ValueType: string; ValueName: ContentViewModeLayoutPatternForSearch; ValueData: alpha; Flags: uninsdeletevalue
; 拡張子に対してProperty Handlerを関連付け
Root: HKA; Subkey: SOFTWARE\Microsoft\Windows\CurrentVersion\PropertySystem\PropertyHandlers\{#AppDefaultExt}; \
  ValueType: string; ValueData: {{{#ShellExtPropertyHandlerClsid}}; Flags: uninsdeletekey

; Thumbnail Provider登録
; ----------------------------------------------------------------
; クラス登録
#emit ClassRegistration(ShellExtThumbnailProviderClsid, ShellExtThumbnailProviderDescription)
; ProgIDに対してThumbnailProviderを関連付け
Root: {#AppDefaultProgIdKey}\ShellEx\{{e357fccd-a995-4576-b01f-234630154e96}; ValueType: string; ValueData: {{{#ShellExtThumbnailProviderClsid}}; Flags: uninsdeletekey

#undef ClassRegistration
#undef AppDefaultProgIdKey

[Run]
Filename: "{sys}\taskkill.exe"; Parameters: "/IM dllhost.exe /F"; Flags: runhidden waituntilterminated; AfterInstall: NotifyToExplorer

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppFileName}"

[Code]
// ----------------------------------------------------------------
// Record 定義
// ----------------------------------------------------------------
type TVersion = record
  Major: Integer;
  Minor: Integer;
end;
// ----------------------------------------------------------------
// 定数値
// ----------------------------------------------------------------
function GetRequiredFrameworks(): TArrayOfString;
begin
  Result := [
    #define result
    #define i
    #for {i = 0; i < DimOf(DotNetFrameworks); i++} result += "    '" + DotNetFrameworks[i] + "'," + NewLine
    #undef i
    #expr Delete(result, Len(result) - 1, 1)
    #emit result
    #undef result
  ];
end;
function GetRequiredVersion(): TVersion;
begin
  Result.Major := {#DotNetMajor};
  Result.Minor := {#DotNetMinor};
end;
// ----------------------------------------------------------------
// Win32 API
// ----------------------------------------------------------------
const
  SHCNE_ASSOCCHANGED = $08000000;
  SHCNF_IDLIST = $0000;
procedure SHChangeNotify(const wEventId: Integer; const uFlags: Cardinal; const dwItem1, dwItem2: Integer);
  external 'SHChangeNotify@shell32.dll stdcall';
// ----------------------------------------------------------------
// Utility: .NET Root Path
// ----------------------------------------------------------------
function GetDotNetRootPath(): String;
begin
  Result := '';
  if not RegQueryStringValue(
    HKLM,
    'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost',
    'Path',
    Result) then Exit;
  Result := RemoveBackslash(Result);
end;
// ----------------------------------------------------------------
// Utility: バージョン比較
// ----------------------------------------------------------------
function IsHigherOrEqualVersion(const left, right: TVersion): Boolean;
begin
  Result := (left.Major > right.Major) or
    (left.Major = right.Major) and (left.Minor >= right.Minor);
end;
// ----------------------------------------------------------------
// Utility: バージョン文字列 → TVersion
// ----------------------------------------------------------------
function ParseVersion(const s: String): TVersion;
var
  parts: TArrayOfString;
begin
  parts := StringSplit(s, ['.'], stExcludeEmpty);
  Result.Major := 0;
  Result.Minor := 0;
  if GetArrayLength(parts) > 0 then
    Result.Major := StrToIntDef(parts[0], 0);
  if GetArrayLength(parts) > 1 then
    Result.Minor := StrToIntDef(parts[1], 0);
end;
// ----------------------------------------------------------------
// フレームワークごとの判定（フォルダ単位）
// ----------------------------------------------------------------
function IsCompatibleFrameworkInstalled(const basePath: String; const version: TVersion): Boolean;
var
  findRec: TFindRec;
begin
  Result := False;
  if not DirExists(basePath) then Exit;
  if not FindFirst(basePath + '\*', findRec) then Exit;
  try
    repeat
      if ((findRec.Attributes and FILE_ATTRIBUTE_DIRECTORY) <> 0) and
         (findRec.Name <> '.') and (findRec.Name <> '..') then
      begin
        if IsHigherOrEqualVersion(ParseVersion(findRec.Name), version) then
        begin
          Result := True;
          Exit;
        end
      end;
    until not FindNext(findRec);
  finally
    FindClose(findRec);
  end;
end;
// ----------------------------------------------------------------
// ランタイム全体の総合判定
// ----------------------------------------------------------------
function IsCompatibleRuntimeInstalled(const frameworks: TArrayOfString; const version: TVersion): Boolean;
var
  root: String;
  i: LongInt;
begin
  Result := False;
  root := GetDotNetRootPath();
  if root = '' then Exit;
  for i := 0 to GetArrayLength(frameworks) - 1 do
  begin
    if not IsCompatibleFrameworkInstalled(root + '\shared\' + frameworks[i], version) then Exit;
  end;
  Result := True;
end;
// ----------------------------------------------------------------
// エクスプローラーへの変更通知
// ----------------------------------------------------------------
procedure NotifyToExplorer();
begin
  SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, 0, 0);
end;
// ----------------------------------------------------------------
// セットアップ初期化時
// ----------------------------------------------------------------
function InitializeSetup(): Boolean;
var
  version: TVersion;
  errorCode: Integer;
begin
  Result := False;
  version := GetRequiredVersion();
  if not IsCompatibleRuntimeInstalled(GetRequiredFrameworks(), version) then
  begin
    MsgBox(FmtMessage(CustomMessage('DotNetNotInstalled'), ['{#AppName}', IntToStr(version.Major), IntToStr(version.Minor)]), mbInformation, MB_OK);
    ShellExec('', FmtMessage(CustomMessage('DotNetRuntimeUrl'), [IntToStr(version.Major), IntToStr(version.Minor)]), '', '', SW_SHOWNORMAL, ewNoWait, errorCode);
    Exit;
  end;
  Result := True;
end;
// ----------------------------------------------------------------
// 現在ページ変更時
// ----------------------------------------------------------------
procedure CurPageChanged(CurPageID: Integer);
var
  str: String;
begin
  if CurPageID = wpWelcome then
  begin
    str := WizardForm.WelcomeLabel2.Caption;
    StringChange(str, CustomMessage('NextPure'), CustomMessage('InstallPure'));
    WizardForm.WelcomeLabel2.Caption := str;
    WizardForm.NextButton.Caption := SetupMessage(msgButtonInstall);
  end
  else if CurPageID = wpFinished then
    WizardForm.NextButton.Caption := SetupMessage(msgButtonFinish)
  else
    WizardForm.NextButton.Caption := SetupMessage(msgButtonNext);
end;
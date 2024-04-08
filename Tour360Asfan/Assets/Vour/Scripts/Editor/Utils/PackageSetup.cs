using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
#if VOUR_XRPLUGINMANAGEMENT
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine.XR.Management;
#endif

[assembly: CrizGames.Vour.OptionalDependency("UnityEngine.XR.Management.XRGeneralSettings", "VOUR_XRPLUGINMANAGEMENT")]

namespace CrizGames.Vour.Editor
{
    [InitializeOnLoad]
    public static  class PackageSetup
    {
        private const string SetupWebVRTitle = "WebVR Setup";
        
        private const string XRPluginManagementUrl = "com.unity.xr.management";
        private const string WebXRExportUrl = "https://github.com/De-Panther/unity-webxr-export.git?path=/Packages/webxr";
        private const string BurstUrl = "com.unity.burst";
        private const string GlTFastUrl = "https://github.com/atteneder/glTFast.git";
        private const string WebXRInputProfilesLoaderUrl = "https://github.com/De-Panther/webxr-input-profiles-loader.git?path=/Packages/webxr-input-profiles-loader";
        
        private static string _currentSetup;
        private static AddRequest _addRequest;
        private static int _currentStep;
        
        private static ListRequest _listRequest;
        private static HashSet<string> _installedPackages = new HashSet<string>();

        static PackageSetup()
        {
            EditorApplication.update -= Progress;
            EditorApplication.update += Progress;
            
            _installedPackages.Clear();
            _listRequest = Client.List();

            EnableWebXRLoader();
        }

        private static void EnableWebXRLoader()
        {
#if VOUR_XRPLUGINMANAGEMENT && VOUR_WEBXR
            XRGeneralSettings target = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.WebGL);
            if (target.AssignedSettings == null)
            {
                XRManagerSettings instance = ScriptableObject.CreateInstance<XRManagerSettings>();
                target.AssignedSettings = instance;
                EditorUtility.SetDirty(target);
            }

            string loaderName = "WebXR.WebXRLoader";
            if (!XRPackageMetadataStore.IsLoaderAssigned(loaderName, BuildTargetGroup.WebGL))
            {
                if (XRPackageMetadataStore.AssignLoader(target.AssignedSettings, loaderName, BuildTargetGroup.WebGL))
                    Debug.Log("Enabled WebXRLoader for XR Plug-in Management");
                else
                {
                    Debug.LogError($"Unable to assign com.de-panther.webxr for build target {BuildTargetGroup.WebGL}. " +
                                   "Please try to enable it manually by checking WebXR Export in Project Settings > XR Plug-in Management > WebGL > Plug-in Providers.");
                }
            }
#endif
        }

        [MenuItem("Tools/Vour/Setup/Install WebVR Packages && Set Up")]
        public static void SetupWebVRPackages()
        {
            _currentStep = 0;
            _currentSetup = SetupWebVRTitle;
            WebVRSetupProgress(StatusCode.Success);
        }

        private static void WebVRSetupProgress(StatusCode status)
        {
            if (status >= StatusCode.Failure)
            {
                var docs = EditorUtility.DisplayDialog(SetupWebVRTitle, 
                    "There was an error while installing a package. Try installing the remaining packages manually.", 
                    "Documentation", "OK");
                if (docs)
                    Application.OpenURL("https://crizgames.gitbook.io/vour/#2.3.-installation-with-web-vr-support");
                return;
            }
            
            string progressInfo = "";
            float progress = _currentStep / 6f;
            
            void ProgressBar() => EditorUtility.DisplayProgressBar(SetupWebVRTitle, progressInfo, progress);
            
            switch (_currentStep)
            {
                case 0:
                    progressInfo = "Checking if WebGL is available...";
                    ProgressBar();
                    
                    // Check if WebGL platform is installed
                    if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL))
                    {
                        EditorUtility.DisplayDialog(SetupWebVRTitle, "The WebGL platform is not installed!", "OK");
                        EndSetup();
                        return;
                    }
                    
                    _currentStep++;
                    WebVRSetupProgress(StatusCode.Success);
                    break;
                
                case 1:
                    // Install XR Plugin Management
                    progressInfo = "Installing XR Plugin Management package...";
                    ProgressBar();
                    InstallPackage(XRPluginManagementUrl);
                    break;
                
                case 2:
                    // Install WebXRExport
                    progressInfo = "Installing WebXR Export package...";
                    ProgressBar();
                    InstallPackage(WebXRExportUrl);
                    break;
                
                case 3:
                    // Install Burst
                    progressInfo = "Installing Burst package...";
                    ProgressBar();
                    InstallPackage(BurstUrl);
                    break;
                
                case 4:
                    // Install glTFast
                    progressInfo = "Installing glTFast package...";
                    ProgressBar();
                    InstallPackage(GlTFastUrl);
                    break;
                
                case 5:
                    // Install WebXR Input Profiles Loader
                    progressInfo = "Installing WebXR Input Profiles Loader package...";
                    ProgressBar();
                    InstallPackage(WebXRInputProfilesLoaderUrl);
                    break;
                
                /*case 6:
                    progressInfo = "Copying WebGLTemplates folder to Assets...";
                    ProgressBar();
                    
                    // Copy WebGLTemplates folder
                    string sourceDir = Application.dataPath + "/Vour/WebGLTemplates";
                    string targetDir = Application.dataPath + "/WebGLTemplates";
                    if (!Directory.Exists(targetDir))
                    {
                        try
                        {
                            CopyDirectory(sourceDir, targetDir);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Couldn't move WebGLTemplates folder to Assets folder");
                            Debug.LogException(e);
                        }
                        AssetDatabase.Refresh();
                    }
                    
                    _currentStep++;
                    WebVRSetupProgress(StatusCode.Success);
                    break;*/
                
                case 6:
                    // Setup complete dialog
                    EndSetup();
                    EditorUtility.DisplayDialog(SetupWebVRTitle, "WebVR Setup completed successfully!", "OK");
                    break;
            }
        }

        private static void Progress()
        {
            // ListRequest
            if (_listRequest != null && _listRequest.IsCompleted)
            {
                if (_listRequest.Status == StatusCode.Success)
                    foreach (var package in _listRequest.Result)
                        _installedPackages.Add(package.packageId);
                else if (_listRequest.Status >= StatusCode.Failure)
                    Debug.Log(_listRequest.Error.message);
                
                _listRequest = null;
            }
            
            if(_installedPackages.Count == 0)
                return;
            
            // AddRequest
            if (_addRequest == null || !_addRequest.IsCompleted) return;
            
            var status = _addRequest.Status;
            if (status == StatusCode.Success)
            {
                _currentStep++;
                _installedPackages.Add(_addRequest.Result.packageId);
            }
            else if (status >= StatusCode.Failure)
            {
                Debug.Log(_addRequest.Error.message);
                EndSetup();
            }
            _addRequest = null;

            CallCurrentSetupProgress(status);
        }

        private static void CallCurrentSetupProgress(StatusCode status)
        {
            switch (_currentSetup)
            {
                case SetupWebVRTitle:
                    WebVRSetupProgress(status);
                    break;
            }
        }

        private static void EndSetup()
        {
            _currentStep = 0;
            EditorUtility.ClearProgressBar();
        }

        private static void InstallPackage(string identifier)
        {
            if (!_installedPackages.Any(x => x.Contains(identifier)))
                _addRequest = Client.Add(identifier);
            else
            {
                _currentStep++;
                CallCurrentSetupProgress(StatusCode.Success);
            }
        }
        
        /*private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            var dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                if(file.Name.Contains(".meta"))
                    continue;
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // Copy subdirectories
            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir);
            }
        }*/
    }
}

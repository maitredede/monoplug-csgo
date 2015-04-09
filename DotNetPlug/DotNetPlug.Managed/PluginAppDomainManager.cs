using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Policy;

namespace DotNetPlug
{
    [Guid("6EDB860E-363E-4D22-BBBE-2868C475217B"), ComVisible(true)]
    public sealed class PluginAppDomainManager : AppDomainManager, IAppDomainManager
    {
        public override void InitializeNewDomain(AppDomainSetup appDomainInfo)
        {
            this.InitializationFlags = AppDomainManagerInitializationOptions.RegisterWithHost;
        }

        private void Trace(string location)
        {
            System.Diagnostics.Trace.WriteLine(string.Format("AppDomain: {0}, {2}::{1}", AppDomain.CurrentDomain.FriendlyName, location, this.GetType().Name));
        }

        public override AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
        {
            //AppDomainSetup appDomainInfo = new AppDomainSetup();

            // Prevent loading from current dir
            appDomainInfo.PrivateBinPathProbe = "*";

            // Set base dir. Mandatory if PrivateBinPath is used
            var baseDir = System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location);
            appDomainInfo.ApplicationBase = baseDir;

            // Load assemblies from plugins and bin subfolders.
            var appDir = System.IO.Path.Combine(baseDir, "bin");
            var pluginDir = System.IO.Path.Combine(baseDir, "friendlyName");

            appDomainInfo.PrivateBinPath = pluginDir + ";" + appDir;

            AppDomain ad = AppDomain.CreateDomain(friendlyName, null, appDomainInfo);
            //AppDomainManager appDomainManager = ad.DomainManager;
            return ad;

            //Trace(string.Format("CreateDomain({0})", friendlyName));
            ////appDomainInfo.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            //var appDomain = base.CreateDomain(friendlyName, securityInfo, appDomainInfo);
            //System.Diagnostics.Trace.WriteLine("ApplicationBase: " + appDomain.SetupInformation.ApplicationBase);
            //System.Diagnostics.Trace.WriteLine("PrivateBinPath: " + appDomain.SetupInformation.PrivateBinPath);

            //var enumerator = appDomain.Evidence.GetAssemblyEnumerator();
            //while (enumerator.MoveNext())
            //{
            //    System.Diagnostics.Trace.WriteLine(enumerator.Current);
            //}

            //return appDomain;
        }

        public void Run(string assemblyFilename, string friendlyName)
        {
            throw new NotSupportedException();
            //    Trace(string.Format("Run({0})", System.IO.Path.GetFileName(assemblyFilename)));

            //    if (!System.IO.File.Exists(assemblyFilename))
            //    {
            //        const string message = "Application cannot be found";
            //        System.Diagnostics.Trace.WriteLine(message);
            //        System.Console.Error.WriteLine(message);
            //        return;
            //    }

            //    var appDomainInfo = new AppDomainSetup();
            //    appDomainInfo.ApplicationBase = new System.IO.FileInfo(assemblyFilename).DirectoryName;
            //    //var evidenceInfo = new Evidence();
            //    //evidenceInfo.AddHostEvidence(new Zone(SecurityZone.Internet));

            //    //PermissionSet permSet = SecurityManager.GetStandardSandbox(evidenceInfo);

            //    PermissionSet permSet = new PermissionSet(PermissionState.None);
            //    permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            //    permSet.AddPermission(new UIPermission(PermissionState.Unrestricted));
            //    permSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery, AccessControlActions.View, appDomainInfo.ApplicationBase));
            //    permSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read, AccessControlActions.View, appDomainInfo.ApplicationBase));
            //    AppDomain ad = null;
            //    try
            //    {
            //        try
            //        {
            //            var strongNames = new StrongName[0];
            //            ad = AppDomain.CreateDomain(friendlyName, AppDomain.CurrentDomain.Evidence, appDomainInfo, permSet,
            //                                        strongNames);
            //        }
            //        catch (Exception ex)
            //        {
            //            string message = string.Format("Failed to create AppDomain for {0}",
            //                                           System.IO.Path.GetFileNameWithoutExtension(assemblyFilename));
            //            Trace(message);
            //            System.Diagnostics.Trace.WriteLine(ex);
            //            System.Console.Error.WriteLine(message);
            //            return;
            //        }

            //        int exitCode = ad.ExecuteAssembly(assemblyFilename);
            //        System.Diagnostics.Trace.WriteLine(string.Format("ExitCode={0}", exitCode));
            //    }
            //    catch (System.Security.SecurityException se)
            //    {
            //        string message = string.Format("Security Exception in {0}",
            //                                       System.IO.Path.GetFileNameWithoutExtension(assemblyFilename));
            //        Trace(message);
            //        System.Diagnostics.Trace.WriteLine(se);
            //        System.Console.Error.WriteLine(message);
            //    }
            //    catch (System.Exception ex)
            //    {
            //        string message = string.Format("Unhandled Exception in {0}",
            //                                       System.IO.Path.GetFileNameWithoutExtension(assemblyFilename));
            //        Trace(message);
            //        System.Diagnostics.Trace.WriteLine(ex);
            //        System.Console.Error.WriteLine(message);
            //    }
            //    finally
            //    {
            //        if (ad != null)
            //            AppDomain.Unload(ad);
            //    }
        }
    }
}

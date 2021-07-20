﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Smartstore.Core.Packaging;
using Smartstore.Core.Security;
using Smartstore.Core.Theming;
using Smartstore.Engine.Modularity;
using Smartstore.Web.Controllers;

namespace Smartstore.Controllers
{
    public class PackagingController : AdminControllerBase
    {
        private readonly IPackageManager _packageManager;
        private readonly IThemeRegistry _themeRegistry;

        public PackagingController(IPackageManager packageManager, IThemeRegistry themeRegistry)
        {
            _packageManager = packageManager;
            _themeRegistry = themeRegistry;
        }

        [HttpPost]
        public async Task<IActionResult> UploadPackage(string returnUrl = "")
        {
            var isTheme = false;
            var success = false;
            var message = (string)null;
            var tempFile = string.Empty;

            try
            {
                var file = Request.Form.Files.FirstOrDefault();
                if (file != null)
                {
                    if (!Path.GetExtension(file.FileName).EqualsNoCase(".zip"))
                    {
                        return Json(new { success, file.FileName, T("Admin.Packaging.NotAPackage").Value, returnUrl });
                    }

                    var zip = new ZipArchive(file.OpenReadStream(), ZipArchiveMode.Read, false);
                    var package = new ExtensionPackage(zip);

                    var requiredPermission = (isTheme = package.Descriptor.ExtensionType == ExtensionType.Theme)
                        ? Permissions.Configuration.Theme.Upload
                        : Permissions.Configuration.Module.Upload;

                    if (!await Services.Permissions.AuthorizeAsync(requiredPermission))
                    {
                        message = T("Admin.AccessDenied.Description").Value;
                        return Json(new { success, file.FileName, message });
                    }

                    var appContext = Services.ApplicationContext;
                    var location = appContext.AppDataRoot.Root;
                    var appPath = appContext.ContentRoot.Root;

                    if (isTheme)
                    {
                        // Avoid getting terrorized by IO events.
                        _themeRegistry.StopMonitoring();
                    }

                    await _packageManager.InstallAsync(package);

                    //if (isTheme)
                    //{
                    //    // Create descriptor.
                    //    if (packageInfo != null)
                    //    {
                    //        var descriptor = ThemeDescriptor.Create(packageInfo.ExtensionDescriptor.Name, appContext.ThemesRoot);
                    //        if (descriptor != null)
                    //        {
                    //            _themeRegistry.AddThemeDescriptor(descriptor);
                    //        }
                    //    }

                    //    // SOFT start IO events again.
                    //    _themeRegistry.StartMonitoring(false);
                    //}
                }
                else
                {
                    return Json(new { success, file.FileName, T("Admin.Common.UploadFile").Value, returnUrl });
                }

                if (!isTheme)
                {
                    message = T("Admin.Packaging.InstallSuccess").Value;
                    // TODO: (core) Hmmm? Restart here or not?
                    //Services.WebHelper.RestartAppDomain();
                    //return RedirectToAction("RestartApplication", "Common", new { returnUrl });
                }
                else
                {
                    message = T("Admin.Packaging.InstallSuccess.Theme").Value;
                }

                success = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                Logger.Error(ex);
            }

            return Json(new { success, tempFile, message, returnUrl });
        }
    }
}

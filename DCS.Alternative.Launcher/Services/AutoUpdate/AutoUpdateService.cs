using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;

namespace DCS.Alternative.Launcher.Services.AutoUpdate
{
    public class AutoUpdateService : IAutoUpdateService
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public Task<bool> CheckAsync()
        {
            return Task.Run(async () =>
            {
                if (!await _semaphore.WaitAsync(1))
                {
                    return false;
                }

                try
                {
                    Tracer.Info("Checking for application updates.");

                    var extractionPath = Path.Combine(Directory.GetCurrentDirectory(), "_update");
                    var downloadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "_downloads");
                    var versionPath = Path.Combine(downloadDirectory, "version.json");

                    if (Directory.Exists(extractionPath) && Directory.GetFileSystemEntries(extractionPath).Length > 0)
                    {
                        var c = File.ReadAllText(versionPath);
                        var v = JsonConvert.DeserializeObject<AutoUpdateVersionInfo>(c);

                        Tracer.Info($"Update {v.Version} is currently waiting to be installed.");
                        return true;
                    }

                    if (Directory.Exists(downloadDirectory))
                    {
                        Directory.Delete(downloadDirectory, true);
                    }

                    Directory.CreateDirectory(downloadDirectory);

                    Tracer.Info("Downloading version file.");

                    var file = await SafeAsync.RunAsync(
                        () => DownloadFileAsync("https://drive.google.com/open?id=1Njnp1Zy_Ed4hzQ5DVStTJG88BjvMLbpm", versionPath),
                        e => { Tracer.Error("An error occured while trying to download the version file.", e); });

                    if (!(file?.Exists ?? false))
                    {
                        Tracer.Warn("Unable to download version info.  File was not found.");
                        return false;
                    }

                    var contents = File.ReadAllText(file.FullName);
                    var versionInfo = JsonConvert.DeserializeObject<AutoUpdateVersionInfo>(contents);
                    var assembly = Assembly.GetExecutingAssembly();
                    var version = assembly.GetName().Version;

                    if (version >= versionInfo.ConcreteVersion)
                    {
                        Tracer.Info("Application is up to date.");
                        return false;
                    }

                    var zipPath = Path.Combine(downloadDirectory, "update.zip");

                    Tracer.Info("Downloading update.");

                    file = await SafeAsync.RunAsync(
                        () => DownloadFileAsync(versionInfo.Url, zipPath),
                        e => { Tracer.Error("An error occured while trying to download the the update.", e); });

                    if (!(file?.Exists ?? false))
                    {
                        Tracer.Warn("Unable to download update archive.");
                        return false;
                    }

                    if (Directory.Exists(extractionPath))
                    {
                        Directory.Delete(extractionPath, true);
                    }

                    Directory.CreateDirectory(extractionPath);

                    try
                    {
                        Tracer.Info("Extracting update.");

                        ExtractZipFile(zipPath, extractionPath);

                        try
                        {
                            Tracer.Info("Removing downloaded archive file.");

                            File.Delete(zipPath);

                            MoveFile(extractionPath, Directory.GetCurrentDirectory(), "AutoUpdate.exe");
                            MoveFile(extractionPath, Directory.GetCurrentDirectory(), "AutoUpdate.exe.config");
                            MoveFile(extractionPath, Directory.GetCurrentDirectory(), "AutoUpdate.pdb");
                        }
                        catch (Exception e)
                        {
                            Tracer.Error("Unable to update AutoUpdate.exe", e);
                        }
                    }
                    catch (Exception e)
                    {
                        Tracer.Error(e);
                        Directory.Delete(extractionPath, true);
                        return false;
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Tracer.Error(e);
                    return false;
                }
                finally
                {
                    _semaphore.Release(1);
                }
            });
        }

        private void MoveFile(string sourcePath, string destinationPath, string file)
        {
            Tracer.Info($"Updating {file}.");

            var sourceFile = Path.Combine(sourcePath, file);
            var destinationFile = Path.Combine(destinationPath, file);

            if (!File.Exists(sourceFile))
            {
                return;
            }

            if (File.Exists(destinationFile))
            {
                File.Delete(destinationFile);
            }

            File.Move(sourceFile, destinationFile);
        }

        public void ExtractZipFile(string archivePath, string outFolder)
        {
            using (Stream fs = File.OpenRead(archivePath))
            using (var zf = new ZipFile(fs))
            {
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;
                    }

                    var entryFileName = zipEntry.Name;
                    var fullZipToPath = Path.Combine(outFolder, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);

                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    var buffer = new byte[4096];

                    using (var zipStream = zf.GetInputStream(zipEntry))
                    using (var fsOutput = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, fsOutput, buffer);
                    }
                }
            }
        }

        private async Task<FileInfo> DownloadFileAsync(string url, string path)
        {
            // You can comment the statement below if the provided url is guaranteed to be in the following format:
            // https://drive.google.com/uc?id=FILEID&export=download
            url = GetGoogleDriveDownloadLinkFromUrl(url);

            using (var webClient = new CookieAwareWebClient())
            {
                FileInfo downloadedFile;

                // Sometimes Drive returns an NID cookie instead of a download_warning cookie at first attempt,
                // but works in the second attempt
                for (var i = 0; i < 2; i++)
                {
                    downloadedFile = await DownloadFileAsync(url, path, webClient);

                    if (downloadedFile == null)
                    {
                        return null;
                    }

                    // Confirmation page is around 50KB, shouldn't be larger than 60KB
                    if (downloadedFile.Length > 60000)
                    {
                        return downloadedFile;
                    }

                    // Downloaded file might be the confirmation page, check it
                    string content;

                    using (var reader = downloadedFile.OpenText())
                    {
                        // Confirmation page starts with <!DOCTYPE html>, which can be preceeded by a newline
                        var header = new char[20];
                        var readCount = reader.ReadBlock(header, 0, 20);

                        if (readCount < 20 || !new string(header).Contains("<!DOCTYPE html>"))
                        {
                            return downloadedFile;
                        }

                        content = reader.ReadToEnd();
                    }

                    var linkIndex = content.LastIndexOf("href=\"/uc?");
                    if (linkIndex < 0)
                    {
                        return downloadedFile;
                    }

                    linkIndex += 6;
                    var linkEnd = content.IndexOf('"', linkIndex);

                    if (linkEnd < 0)
                    {
                        return downloadedFile;
                    }

                    url = "https://drive.google.com" + content.Substring(linkIndex, linkEnd - linkIndex).Replace("&amp;", "&");
                }

                downloadedFile = await DownloadFileAsync(url, path, webClient);

                return downloadedFile;
            }
        }

        // Handles 3 kinds of links (they can be preceeded by https://):
        // - drive.google.com/open?id=FILEID
        // - drive.google.com/file/d/FILEID/view?usp=sharing
        // - drive.google.com/uc?id=FILEID&export=download
        private string GetGoogleDriveDownloadLinkFromUrl(string url)
        {
            var index = url.IndexOf("id=");
            var closingIndex = 0;

            if (index > 0)
            {
                index += 3;
                closingIndex = url.IndexOf('&', index);

                if (closingIndex < 0)
                {
                    closingIndex = url.Length;
                }
            }
            else
            {
                index = url.IndexOf("file/d/");
                if (index < 0) // url is not in any of the supported forms
                {
                    return string.Empty;
                }

                index += 7;

                closingIndex = url.IndexOf('/', index);

                if (closingIndex < 0)
                {
                    closingIndex = url.IndexOf('?', index);

                    if (closingIndex < 0)
                    {
                        closingIndex = url.Length;
                    }
                }
            }

            return $"https://drive.google.com/uc?id={url.Substring(index, closingIndex - index)}&export=download";
        }

        private async Task<FileInfo> DownloadFileAsync(string url, string path, WebClient webClient)
        {
            try
            {
                if (webClient == null)
                {
                    using (webClient = new WebClient())
                    {
                        await webClient.DownloadFileTaskAsync(url, path);
                        return new FileInfo(path);
                    }
                }

                await webClient.DownloadFileTaskAsync(url, path);

                return new FileInfo(path);
            }
            catch (WebException)
            {
                return null;
            }
        }

        // Web client used for Google Drive
        public class CookieAwareWebClient : WebClient
        {
            private readonly CookieContainer cookies;

            public CookieAwareWebClient()
            {
                cookies = new CookieContainer();
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);

                if (request is HttpWebRequest)
                {
                    var cookie = cookies[address];

                    if (cookie != null)
                    {
                        ((HttpWebRequest) request).Headers.Set("cookie", cookie);
                    }
                }

                return request;
            }

            protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
            {
                var response = base.GetWebResponse(request, result);
                var cookies = response.Headers.GetValues("Set-Cookie");

                if (cookies != null && cookies.Length > 0)
                {
                    var cookie = "";

                    foreach (var c in cookies)
                    {
                        cookie += c;
                    }

                    this.cookies[response.ResponseUri] = cookie;
                }

                return response;
            }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                var response = base.GetWebResponse(request);
                var cookies = response.Headers.GetValues("Set-Cookie");

                if (cookies != null && cookies.Length > 0)
                {
                    var cookie = "";

                    foreach (var c in cookies)
                    {
                        cookie += c;
                    }

                    this.cookies[response.ResponseUri] = cookie;
                }

                return response;
            }

            private class CookieContainer
            {
                private readonly Dictionary<string, string> _cookies;

                public CookieContainer()
                {
                    _cookies = new Dictionary<string, string>();
                }

                public string this[Uri url]
                {
                    get
                    {
                        string cookie;

                        if (_cookies.TryGetValue(url.Host, out cookie))
                        {
                            return cookie;
                        }

                        return null;
                    }
                    set { _cookies[url.Host] = value; }
                }
            }
        }
    }
}
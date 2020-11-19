// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Installer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using Exiled.Installer.Properties;

    using ICSharpCode.SharpZipLib.GZip;
    using ICSharpCode.SharpZipLib.Tar;

    using Octokit;

    internal enum PathResolution
    {
        UNDEFINED,
        /// <summary>
        ///     Absolute path that is routed to AppData.
        /// </summary>
        ABSOLUTE,
        /// <summary>
        ///     The path that goes through the path of the game passing through the subfolders.
        /// </summary>
        GAME
    }

    internal static class Program
    {
        private const long REPO_ID = 231269519;
        private const string EXILED_ASSET_NAME = "exiled.tar.gz";
        internal const string TARGET_FILE_NAME = "Assembly-CSharp.dll";

        private static readonly string[] TargetSubfolders = { "SCPSL_Data", "Managed" };
        private static readonly string LinkedSubfolders = string.Join(Path.DirectorySeparatorChar, TargetSubfolders);
        private static readonly Version VersionLimit = new Version(2, 0, 0);
        private static readonly uint SecondsWaitForDownload = 480;

        private static readonly string Header = $"{Assembly.GetExecutingAssembly().GetName().Name}-{Assembly.GetExecutingAssembly().GetName().Version}";
        private static readonly GitHubClient GitHubClient = new GitHubClient(
            new ProductHeaderValue(Header));

        // Force use of LF because the file uses LF
        private static readonly Dictionary<string, string> Markup = Resources.Markup.Trim().Split('\n').ToDictionary(s => s.Split(':')[0], s => s.Split(':', 2)[1]);

        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = new UTF8Encoding(false, false);
            await CommandSettings.Parse(args).ConfigureAwait(false);
        }

        internal static async Task MainSafe(CommandSettings args)
        {
            try
            {
                Console.WriteLine(Header);

                if (args.GetVersions)
                {
                    var releases1 = await GetReleases().ConfigureAwait(false);
                    Console.WriteLine("--- AVAILABLE VERSIONS ---");
                    foreach (var r in releases1)
                        Console.WriteLine(FormatRelease(r, true));

                    if (args.Exit)
                        Environment.Exit(0);
                }

                Console.WriteLine($"AppData folder: {args.AppData.FullName}");

                if (!ValidateServerPath(args.Path.FullName, out var targetFilePath))
                {
                    Console.WriteLine($"Couldn't find '{TARGET_FILE_NAME}' in '{targetFilePath}'");
                    throw new FileNotFoundException("Check the validation of the path parameter");
                }

                if (!(args.GitHubToken is null))
                {
                    Console.WriteLine("Token detected! Using the token...");
                    GitHubClient.Credentials = new Credentials(args.GitHubToken, AuthenticationType.Bearer);
                }

                Console.WriteLine("Receiving releases...");
                Console.WriteLine($"Prereleases included - {args.PreReleases}");
                Console.WriteLine($"Target release version - {(string.IsNullOrEmpty(args.TargetVersion) ? "(null)" : args.TargetVersion)}");

                var releases = await GetReleases().ConfigureAwait(false);
                Console.WriteLine("Searching for the latest release that matches the parameters...");

                if (!TryFindRelease(args, releases, out var targetRelease))
                {
                    Console.WriteLine("--- RELEASES ---");
                    Console.WriteLine(string.Join(Environment.NewLine, releases.Select(FormatRelease)));
                    throw new InvalidOperationException("Couldn't find release");
                }

                Console.WriteLine("Release found!");
                Console.WriteLine(FormatRelease(targetRelease!));

                var exiledAsset = targetRelease!.Assets.FirstOrDefault(a => a.Name.Equals(EXILED_ASSET_NAME, StringComparison.OrdinalIgnoreCase));
                if (exiledAsset is null)
                {
                    Console.WriteLine("--- ASSETS ---");
                    Console.WriteLine(string.Join(Environment.NewLine, targetRelease.Assets.Select(FormatAsset)));
                    throw new InvalidOperationException("Couldn't find asset");
                }

                Console.WriteLine("Asset found!");
                Console.WriteLine(FormatAsset(exiledAsset));

                using var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(SecondsWaitForDownload)
                };
                httpClient.DefaultRequestHeaders.Add("User-Agent", Header);

                using var downloadResult = await httpClient.GetAsync(exiledAsset.BrowserDownloadUrl).ConfigureAwait(false);
                using var downloadArchiveStream = await downloadResult.Content.ReadAsStreamAsync().ConfigureAwait(false);

                using var gzInputStream = new GZipInputStream(downloadArchiveStream);
                using var tarInputStream = new TarInputStream(gzInputStream);

                TarEntry entry;
                while (!((entry = tarInputStream.GetNextEntry()) is null))
                {
                    entry.Name = entry.Name.Replace('/', Path.DirectorySeparatorChar);
                    ProcessTarEntry(args, targetFilePath, tarInputStream, entry);
                }

                Console.WriteLine("Installation complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Read the exception message, read the readme, and if you still don't understand what to do, then contact #support in our discord server with the attached screenshot of the full exception");
                if (!args.Exit)
                    Console.Read();
            }

            if (args.Exit)
                Environment.Exit(0);
        }

        private static async Task<IEnumerable<Release>> GetReleases() =>
            (await GitHubClient.Repository.Release.GetAll(REPO_ID).ConfigureAwait(false))
                .Where(r => SemVer2Version.TryParse(r.TagName, out var version)
                && VersionComparer.CustomVersionGreaterOrEquals(version.Backwards, VersionLimit))
                .OrderByDescending(r => r.CreatedAt.Ticks);

        private static string FormatRelease(Release r)
            => FormatRelease(r, false);

        private static string FormatRelease(Release r, bool includeAssets)
        {
            var builder = new StringBuilder(30);
            builder.AppendLine($"PRE: {r.Prerelease} | ID: {r.Id} | TAG: {r.TagName}");
            if (includeAssets)
            {
                foreach (var asset in r.Assets)
                    builder.Append("   - ").AppendLine(FormatAsset(asset));
            }

            return builder.ToString().Trim('\r', '\n');
        }

        private static string FormatAsset(ReleaseAsset a)
        {
            return $"ID: {a.Id} | NAME: {a.Name} | SIZE: {a.Size} | URL: {a.Url} | DownloadURL: {a.BrowserDownloadUrl}";
        }

        private static void ResolvePath(CommandSettings args, string filePath, out string path)
        {
            path = Path.Combine(args.AppData.FullName, filePath);
        }

        private static void ProcessTarEntry(CommandSettings args, string targetFilePath, TarInputStream tarInputStream, TarEntry entry)
        {
            if (entry.IsDirectory)
            {
                var entries = entry.GetDirectoryEntries();
                for (int z = 0; z < entries.Length; z++)
                {
                    ProcessTarEntry(args, targetFilePath, tarInputStream, entries[z]);
                }
            }
            else
            {
                Console.WriteLine($"Processing '{entry.Name}'");

                if (entry.Name.Contains("example", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Extract for {entry.Name} is disabled");
                    return;
                }

                switch (ResolveEntry(entry))
                {
                    case PathResolution.ABSOLUTE:
                        ResolvePath(args, entry.Name, out var path);
                        ExtractEntry(tarInputStream, entry, path);
                        break;
                    case PathResolution.GAME:
                        ExtractEntry(tarInputStream, entry, targetFilePath);
                        break;
                    default:
                        Console.WriteLine($"Couldn't resolve path for '{entry.Name}', update installer");
                        break;
                }
            }
        }

        private static void ExtractEntry(TarInputStream tarInputStream, TarEntry entry, string path)
        {
            Console.WriteLine($"Extracting '{Path.GetFileName(entry.Name)}' into '{path}'...");

            EnsureDirExists(Path.GetDirectoryName(path)!);

            FileStream? fs = null;
            try
            {
                fs = new FileStream(path, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);
                tarInputStream.CopyEntryContents(fs);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while trying to extract a file");
                Console.WriteLine(ex);
            }
            finally
            {
                fs?.Dispose();
            }
        }

        internal static bool ValidateServerPath(string serverPath, out string targetFilePath)
        {
            targetFilePath = Path.Combine(serverPath, LinkedSubfolders, TARGET_FILE_NAME);
            return File.Exists(targetFilePath);
        }

        private static void EnsureDirExists(string pathToDir)
        {
#if DEBUG
            Console.WriteLine($"Ensuring directory path: {pathToDir}");
            Console.WriteLine($"Does it exist? - {Directory.Exists(pathToDir)}");
#endif
            if (!Directory.Exists(pathToDir))
                Directory.CreateDirectory(pathToDir);
        }

        private static PathResolution ResolveEntry(TarEntry entry)
        {
            static PathResolution TryParse(string s)
            {
                // We'll get UNDEFINED if it cannot be determined
                Enum.TryParse<PathResolution>(s, true, out var result);
                return result;
            }

            var fileName = entry.Name;
            var fileInFolder = !string.IsNullOrEmpty(Path.GetDirectoryName(fileName));
            foreach (var pair in Markup)
            {
                var isFolder = pair.Key.EndsWith('\\');
                if (fileInFolder && isFolder &&
                    pair.Key[0..^1].Equals(fileName.Substring(0, fileName.IndexOf(Path.DirectorySeparatorChar)), StringComparison.OrdinalIgnoreCase))
                {
                    return TryParse(pair.Value);
                }
                else if (!fileInFolder && !isFolder &&
                    pair.Key.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                {
                    return TryParse(pair.Value);
                }
            }

            return PathResolution.UNDEFINED;
        }

        private static bool TryFindRelease(CommandSettings args, IEnumerable<Release> releases, out Release? release)
        {
            foreach (var r in releases)
            {
                release = r;

                if (args.TargetVersion != null && r.TagName.Equals(args.TargetVersion, StringComparison.OrdinalIgnoreCase))
                    return true;

                if ((r.Prerelease && args.PreReleases) || !r.Prerelease)
                    return true;
            }

            release = null;
            return false;
        }
    }
}

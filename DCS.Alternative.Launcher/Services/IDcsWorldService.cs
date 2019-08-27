using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Models;

namespace DCS.Alternative.Launcher.Services
{
    public interface IDcsWorldService
    {
        Task<Module[]> GetInstalledAircraftModulesAsync();

        Task<string> GetLatestYoutubeVideoUrlAsync();

        Task<NewsArticleModel[]> GetLatestNewsArticlesAsync(int count = 10);

        Task<ReadOnlyDictionary<string, Version>> GetLatestVersionsAsync();

        Task PatchViewportsAsync();

        Task UpdateAdvancedOptionsAsync();

        Task WriteOptionsAsync(bool isVREnabled);
    }
}
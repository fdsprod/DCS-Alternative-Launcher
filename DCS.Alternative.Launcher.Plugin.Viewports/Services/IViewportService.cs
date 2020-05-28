using System.Collections.Generic;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Plugin.Viewports.DomainObjects;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Plugin.Viewports.Services
{
    public interface IViewportService
    {
        Task PatchViewportsAsync();

        Task WriteViewportOptionsAsync();

        ModuleViewportTemplate[] GetViewportTemplates();

        ModuleViewportTemplate GetViewportTemplateByModule(string moduleId);

        void RemoveViewportTemplate(string moduleId);

        void RemoveViewport(string moduleId, Viewport viewport);

        void UpsertViewport(string name, string moduleId, Screen screen, Viewport viewport);

        void ClearViewports(string name, string moduleId);

        ModuleViewportTemplate[] GetDefaultViewportTemplates();

        ViewportDevice[] GetViewportDevices(string moduleId);

        ViewportOption[] GetViewportOptionsByModuleId(string moduleId);

        Dictionary<string, ViewportOption[]> GetAllViewportOptions();
    }
}
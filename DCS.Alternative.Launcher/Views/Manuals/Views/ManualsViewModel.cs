using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.ServiceModel;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Manuals.Views
{
    public class ManualsViewModel : NavigationAwareBase
    {
        private readonly ManualsController _controller;

        public ManualsViewModel(IContainer container)
        {
            _controller = container.Resolve<ManualsController>();

            OpenDocumentCommand.Subscribe(OnOpenDocument);
            OpenUrlCommand.Subscribe(OnOpenUrl);
        }

        public ReactiveCommand<FileModel> OpenDocumentCommand
        {
            get;
        } = new ReactiveCommand<FileModel>();

        public ReactiveCommand<string> OpenUrlCommand
        {
            get;
        } = new ReactiveCommand<string>();

        public ReactiveProperty<bool> IsLoading
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveCollection<ModuleDocumentModel> ModuleDocuments
        {
            get;
        } = new ReactiveCollection<ModuleDocumentModel>();

        private void OnOpenUrl(string value)
        {
            var ps = new ProcessStartInfo(value)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        private void OnOpenDocument(FileModel value)
        {
            if (AdobeAcrobatHelper.IsDCVersionInstalled() && !AdobeAcrobatHelper.IsProtectedModeDisabled())
            {
                if(MessageBoxEx.Show($"The version of Acrobat Reader you have installed requires a registry change to allow this application to launch PDFs.{Environment.NewLine}Do you want to allow DCS Alternative Launcher to make this change?", "Disable Protected Mode", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                {
                    return;
                }

                AdobeAcrobatHelper.ApplyProtectedModeFix();
            }

            var ps = new ProcessStartInfo(value.Path)
            {
                UseShellExecute = true,
            };

            Process.Start(ps);
        }

        protected override async Task InitializeAsync()
        {
            IsLoading.Value = true;

            try
            {
                await Task.Run(async () =>
                {
                    var modules = await _controller.GetModulesAsync();

                    foreach (var module in modules.GroupBy(m => m.DocumentationPath).Select(g => g.First()))
                    {
                        var model = new ModuleDocumentModel();

                        model.Module = module;

                        foreach (var file in Directory.GetFiles(module.DocumentationPath, "*.pdf"))
                        {
                            var fileName = Path.GetFileNameWithoutExtension(file);

                            model.Documents.Add(new FileModel
                            {
                                Name = fileName,
                                Path = file
                            });
                        }

                        var resources = _controller.GetAdditionResources(module.ModuleId);

                        model.AdditionalResources.AddRange(resources);

                        ModuleDocuments.Add(model);
                    }
                });
            }
            finally
            {
                IsLoading.Value = false;
            }

            await base.InitializeAsync();
        }
    }

    public class ModuleDocumentModel
    {
        public Module Module
        {
            get;
            set;
        }

        public List<FileModel> Documents
        {
            get;
        } = new List<FileModel>();

        public List<AdditionalResource> AdditionalResources
        {
            get;
        } = new List<AdditionalResource>();
    }

    public class FileModel
    {
        public string Name
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }
    }
}
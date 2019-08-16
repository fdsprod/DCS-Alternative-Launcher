using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Plugins.Game.Views;
using DCS.Alternative.Launcher.ServiceModel;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Manuals.Views
{
    public class ManualsViewModel : NavigationAwareBase
    {
        private readonly IContainer _container;
        private readonly ManualsController _controller;

        public ManualsViewModel(IContainer container)
        {
            _container = container;
            _controller = container.Resolve<ManualsController>();

            OpenDocumentCommand.Subscribe(onOpenDocument);
        }

        private void onOpenDocument(FileModel value)
        {
            Process.Start(value.Path);
        }

        protected override async Task InitializeAsync()
        {
            IsLoading.Value = true;

            try
            {
                await Task.Run(async () =>
                {
                    var modules = await _controller.GetModulesAsync();

                    foreach (var module in modules)
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

        public ReactiveCommand<FileModel> OpenDocumentCommand
        {
            get;
        } = new ReactiveCommand<FileModel>();

        public ReactiveProperty<bool> IsLoading
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveCollection<ModuleDocumentModel> ModuleDocuments
        {
            get;
        } = new ReactiveCollection<ModuleDocumentModel>();
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
    }

    public class  FileModel
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
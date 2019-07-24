using System.Collections.Generic;
using DCS.Alternative.Launcher.Modules;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public class ModuleViewportModel
    {
        public ModuleViewportModel(Module module, IEnumerable<Viewport> viewports)
        {
            Module.Value = module;

            if (viewports != null)
            {
                foreach (var viewport in viewports)
                {
                    Viewports.Add(viewport);
                }
            }
        }

        public ReactiveProperty<Module> Module
        {
            get;

        } = new ReactiveProperty<Module>();

        public ReactiveCollection<Viewport> Viewports
        {
            get; 

        } = new ReactiveCollection<Viewport>();
    }
}
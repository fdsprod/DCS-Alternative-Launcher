using System.Collections.Generic;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Modules;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Models
{
    public class ModuleViewportModel
    {
        public ModuleViewportModel(string name, string imageUrl, Module module, IEnumerable<Viewport> viewports)
        {
            Name.Value = name;
            ImageUrl.Value = imageUrl;
            Module.Value = module;

            if (viewports != null)
            {
                foreach (var viewport in viewports)
                {
                    Viewports.Add(viewport);
                }
            }
        }

        public ReactiveProperty<bool> IsSelected
        {
            get;

        } = new ReactiveProperty<bool>();

        public ReactiveProperty<string> Name
        {
            get;

        } = new ReactiveProperty<string>();

        public ReactiveProperty<string> ImageUrl
        {
            get;

        } = new ReactiveProperty<string>();

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
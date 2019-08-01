using System;
using DCS.Alternative.Launcher.ComponentModel;

namespace DCS.Alternative.Launcher.Controls
{
    public interface IWizardStep : IActivate, IDeactivate, IDisposable
    {
        bool OnNext();

        bool Commit();

        bool Validate();

        bool OnBack();

        bool OnComplete();
    }
}
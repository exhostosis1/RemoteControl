using System;

namespace Shared
{
    public interface IUserInterface
    {
        public Func<ViewModel>? StartEvent { get; set; }
        public Func<ViewModel>? StopEvent { get; set; }
        public Func<ViewModel>? AutostartEvent { get; set; }

        public void ShowUI();
    }
}
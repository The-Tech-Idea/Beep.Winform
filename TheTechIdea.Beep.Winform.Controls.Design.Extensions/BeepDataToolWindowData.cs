using Microsoft.VisualStudio.Extensibility.UI;
using System.Runtime.Serialization;
using TheTechIdea.Beep.Container.Services;

namespace  TheTechIdea.Beep.Desktop.Design.Extensions
{
    /// <summary>
    /// ViewModel for the BeepDataToolWindowContent remote user control.
    /// </summary>
    [DataContract]
    internal class BeepDataToolWindowData : NotifyPropertyChangedObject
    {
        public BeepDataToolWindowData(IBeepService beepService)
        {
            _beepservice= beepService;
            HelloCommand = new AsyncCommand((parameter, clientContext, cancellationToken) =>
            {
                Text = $"Hello {parameter as string}!";
                return Task.CompletedTask;
            });
        }

        private string _name = string.Empty;
        [DataMember]
        public string Name
        {
            get => _name;
            set => SetProperty(ref this._name, value);
        }

        private string _text = string.Empty;
        [DataMember]
        public string Text
        {
            get => _text;
            set => SetProperty(ref this._text, value);
        }

        private readonly IBeepService _beepservice;

        [DataMember]
        public AsyncCommand HelloCommand { get; }
    }
}

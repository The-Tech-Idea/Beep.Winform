using Microsoft.VisualStudio.Extensibility.UI;
using System.Runtime.Serialization;
using TheTechIdea.Beep.Container.Services;

namespace TheTechIdea.Beep.Desktop.Design.Extensions
{
    /// <summary>
    /// ViewModel for the BeepNuggetDataConnectersWindowContent remote user control.
    /// </summary>
    [DataContract]
    internal class BeepNuggetDataConnectersWindowData : NotifyPropertyChangedObject
    {
        public BeepNuggetDataConnectersWindowData(IBeepService beepService)
        {
            _beepService = beepService;
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

        private readonly IBeepService _beepService;

        [DataMember]
        public AsyncCommand HelloCommand { get; }
    }
}

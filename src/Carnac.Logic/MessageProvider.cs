using System;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using Carnac.Logic.Models;
using SettingsProviderNet;

namespace Carnac.Logic
{
    [Export(typeof(IMessageProvider))]
    public class MessageProvider : IMessageProvider
    {
        readonly IKeyProvider keyProvider;
        private readonly IShortcutProvider shortcutProvider;
        private readonly PopupSettings settings;
        readonly MessageMerger messageMerger;

        [ImportingConstructor]
        public MessageProvider(IKeyProvider keyProvider, IShortcutProvider shortcutProvider, ISettingsProvider settingsProvider)
        {
            this.keyProvider = keyProvider;
            this.shortcutProvider = shortcutProvider;
            settings = settingsProvider.GetSettings<PopupSettings>();
            messageMerger = new MessageMerger();
        }

        public IObservable<Message> GetMessageStream()
        {
            return keyProvider.GetKeyPressStream()
                .Scan(new ShortcutAccumulator(), (acc, key) => acc.ProcessKey(shortcutProvider, key))
                .Where(c => c.HasCompletedValue)
                .SelectMany(c => c.GetMessages())
                .Scan(new Message(), (previousMessage, newMessage) => messageMerger.MergeIfNeeded(previousMessage, newMessage))
                .Where(m => !settings.DetectShortcutsOnly || m.IsShortcut);
            ;
        }
    }
}
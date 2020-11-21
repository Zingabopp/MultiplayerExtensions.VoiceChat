using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Codecs
{
    public class CodecFactory : ICodecFactory
    {
        private Dictionary<string, Func<ICodecSettings, IEncoder>> _encoderFactories = new Dictionary<string, Func<ICodecSettings, IEncoder>>();
        private Dictionary<string, Func<ICodecSettings, IDecoder>> _decoderFactories = new Dictionary<string, Func<ICodecSettings, IDecoder>>();
        private Dictionary<string, ICodecSettings> _defaultSettings = new Dictionary<string, ICodecSettings>();

        public IDecoder CreateDecoder(string codecId)
        {
            if (_decoderFactories.TryGetValue(codecId, out Func<ICodecSettings, IDecoder>? factory))
            {
                if (_defaultSettings.TryGetValue(codecId, out ICodecSettings codecSettings))
                {
                    return factory(codecSettings ?? throw new InvalidOperationException($"CodecId '{codecId}' has null default settings."));
                }
                throw new InvalidOperationException($"CodecId '{codecId}' has no registered default settings.");
            }
            throw new InvalidOperationException($"CodecId '{codecId}' is not registered.");
        }

        public IDecoder CreateDecoder(string codecId, ICodecSettings codecSettings)
        {
            if (_decoderFactories.TryGetValue(codecId, out Func<ICodecSettings, IDecoder>? factory))
            {
                return factory(codecSettings ?? throw new InvalidOperationException($"CodecId '{codecId}' has null default settings."));
            }
            throw new InvalidOperationException($"CodecId '{codecId}' is not registered.");
        }

        public IEncoder CreateEncoder(string codecId)
        {
            if (_encoderFactories.TryGetValue(codecId, out Func<ICodecSettings, IEncoder>? factory))
            {
                if (_defaultSettings.TryGetValue(codecId, out ICodecSettings codecSettings))
                {
                    return factory(codecSettings ?? throw new InvalidOperationException($"CodecId '{codecId}' has null default settings."));
                }
                throw new InvalidOperationException($"CodecId '{codecId}' has no registered default settings.");
            }
            throw new InvalidOperationException($"CodecId '{codecId}' is not registered.");
        }

        public IEncoder CreateEncoder(string codecId, ICodecSettings codecSettings)
        {
            if (_encoderFactories.TryGetValue(codecId, out Func<ICodecSettings, IEncoder>? factory))
            {
                return factory(codecSettings ?? throw new InvalidOperationException($"CodecId '{codecId}' has null default settings."));
            }
            throw new InvalidOperationException($"CodecId '{codecId}' is not registered.");
        }

        public ICodecSettings? GetDefaultSettings(string codecId)
        {
            if (_defaultSettings.TryGetValue(codecId, out ICodecSettings settings))
                return settings;
            return null;
        }

        public bool HasCodec(string codecId)
        {
            return _defaultSettings.ContainsKey(codecId);
        }

        public void RegisterCodec(string codecId, ICodecSettings defaultSettings, Func<ICodecSettings, IEncoder> encoderFactory, Func<ICodecSettings, IDecoder> decoderFactory)
        {
            if (string.IsNullOrEmpty(codecId)) throw new ArgumentNullException(nameof(codecId));
            if (defaultSettings == null) throw new ArgumentNullException(nameof(defaultSettings));
            if (encoderFactory == null) throw new ArgumentNullException(nameof(encoderFactory));
            if (decoderFactory == null) throw new ArgumentNullException(nameof(decoderFactory));
            _defaultSettings[codecId] = defaultSettings;
            _encoderFactories[codecId] = encoderFactory;
            _decoderFactories[codecId] = decoderFactory;
        }
    }
}

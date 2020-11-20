namespace MultiplayerExtensions.VoiceChat.Codecs.Opus
{
    public class OpusCodecFactory : ICodecFactory
    {
        public string CodecId => "Opus";
        public OpusSettings DefaultSettings { get; }
        public OpusCodecFactory()
        {
            DefaultSettings = new OpusSettings();
        }

        public OpusCodecFactory(ICodecSettings codecSettings)
        {
            DefaultSettings = OpusSettings.CloneSettings(codecSettings);
        }

        public OpusDecoder CreateDecoder(OpusSettings opusSettings)
        {
            if (opusSettings == null)
                opusSettings = DefaultSettings;
            return new OpusDecoder(opusSettings.SampleRate, opusSettings.Channels);
        }

        public OpusEncoder CreateEncoder(OpusSettings opusSettings)
        {
            if (opusSettings == null)
                opusSettings = DefaultSettings;
            OpusEncoder encoder = new OpusEncoder(opusSettings.SampleRate, opusSettings.Channels, opusSettings.FrameDuration, opusSettings.Bitrate);
            return encoder;
        }
        public OpusDecoder CreateDecoder(int sampleRate, int channels, OpusSettings? opusSettings = null)
        {
            return new OpusDecoder(sampleRate, channels);
        }

        public OpusEncoder CreateEncoder(int sampleRate, int channels, OpusSettings? opusSettings = null)
        {
            if (opusSettings == null)
                opusSettings = DefaultSettings;
            OpusEncoder encoder = new OpusEncoder(sampleRate, channels, opusSettings.FrameDuration, opusSettings.Bitrate);
            return encoder;
        }

        ICodecSettings ICodecFactory.DefaultSettings => DefaultSettings;
        IEncoder ICodecFactory.CreateEncoder() => CreateEncoder(DefaultSettings);

        IEncoder ICodecFactory.CreateEncoder(ICodecSettings codecSettings) => CreateEncoder(OpusSettings.CloneSettings(codecSettings));

        IDecoder ICodecFactory.CreateDecoder() => CreateDecoder(DefaultSettings);

        IDecoder ICodecFactory.CreateDecoder(ICodecSettings codecSettings) => CreateDecoder(OpusSettings.CloneSettings(codecSettings));

        //IEncoder ICodecFactory.CreateEncoder(int sampleRate, int channels, ICodecSettings codecSettings)
        //    => CreateEncoder(sampleRate, channels, OpusSettings.CloneSettings(codecSettings));

        //IDecoder ICodecFactory.CreateDecoder(int sampleRate, int channels, ICodecSettings codecSettings)
        //    => CreateDecoder(sampleRate, channels, OpusSettings.CloneSettings(codecSettings));
    }
}

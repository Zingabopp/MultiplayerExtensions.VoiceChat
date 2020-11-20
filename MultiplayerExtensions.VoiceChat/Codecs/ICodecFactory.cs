namespace MultiplayerExtensions.VoiceChat.Codecs
{
    public interface ICodecFactory
    {
        string CodecId { get; }
        ICodecSettings DefaultSettings { get; }
        IEncoder CreateEncoder();
        IEncoder CreateEncoder(ICodecSettings codecSettings);
        IEncoder CreateEncoder(int sampleRate, int channels);
        IEncoder CreateEncoder(int sampleRate, int channels, ICodecSettings? codecSettings);
        IDecoder CreateDecoder();
        IDecoder CreateDecoder(ICodecSettings codecSettings);
        IDecoder CreateDecoder(int sampleRate, int channels);
        IDecoder CreateDecoder(int sampleRate, int channels, ICodecSettings? codecSettings);
    }

    public interface ICodecSettings
    {
        string CodecId { get; }
        int SampleRate { get; }
        int Channels { get; }

        ICodecSettings Clone();
    }
}

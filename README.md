# MultiplayerExtensions.VoiceChat
Adds Voice Chat to MultiplayerExtensions for Beat Saber.

# Packet Information
All packets for this mod use MessageType `128`. Current flow is `Player n` joins lobby -> Players in the lobby send `Player n` `VoiceInfoRequest`packets -> `Player n` responds to each with `VoiceMetadata` packets -> Players can then send `VoiceData` packets to eachother.

### SubTypes
|Name|Type|Description|
|---|---|---|
|VoiceData|`0`|Contains the encoded audio data.|
|InfoRequest|`1`|Not currently used. Intended for requesting the data encoding information (codec, channel count, sample rate, etc)|
|VoiceMetaData|`2`|Not currently used. Intended for describing the data encoding (codec, channel count, sample rate, etc)|

## Packet Structure
### VoiceData v1
Packets containing the encoded voice data.
|Name|Type|Description|
|---|---|---|
|PacketVersion|byte|Version of the VoiceData packet.|
|Index|int|Index of the packet.|
|Checksum|int|Sum of the bytes in the data (overflow wraps).|
|DataLength|int|Length of the data in bytes.|
|Data|byte[`DataLength`]|Encoded audio data.|

### VoiceInfoRequest v1
Packet sent by the receiver requesting the sender's codec information.
|Name|Type|Description|
|---|---|---|
|PacketVersion|byte|Version of the VoiceData packet.|
|PreferredCodec|string|CodecId of the codec preferred by the receiver.|
|SupportedCodecs|string[]|Codecs supported by the receiver.|

### VoiceMetadata v1
Packet sent by the voice sender in response to a `VoiceInfoRequest`.
|Name|Type|Description|
|---|---|---|
|PacketVersion|byte|Version of the VoiceData packet.|
|SampleRate|int|SampleRate (in Hz) used by the voice sender.|
|Channels|byte|Number of channels in the voice sender's encoded packets.|
|Codec|string|CodecId of the codec used by the sender. Empty string if the sender doesn't support any codecs in the receiver's VoiceInfoRequest.|

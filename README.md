# MultiplayerExtensions.VoiceChat
Adds Voice Chat to MultiplayerExtensions for Beat Saber.

# Packet Information
All packets for this mod use MessageType `128`.
### SubTypes
|Name|Type|Description|
|---|---|---|
|VoiceData|`0`|Contains the encoded audio data.|
|InfoRequest|`1`|Not currently used. Intended for requesting the data encoding information (codec, channel count, sample rate, etc)|
|VoiceMetaData|`2`|Not currently used. Intended for describing the data encoding (codec, channel count, sample rate, etc)|

## Packet Structure
### VoiceData
|Name|Type|Description|
|---|---|---|
|PlayerId|string|UserId of the player creating this packet (likely to be removed).|
|Index|int|Index of the packet, also probably unnecessary.|
|DataLength|int|Length of the data in bytes.|
|Data|byte[]|Encoded audio data.|

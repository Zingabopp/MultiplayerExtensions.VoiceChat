///////////////////////////////////////////////////////////////////////////////////////////////
// This file is from https://github.com/DwayneBull/UnityVOIP/blob/master/AudioUtils.cs
//
// The MIT License (MIT)
// 
// Copyright(c) 2016 Dwayne Bull
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MultiplayerExtensions.VoiceChat.Utilities
{
    public enum BandMode
    {
        Narrow,
        Medium,
        Wide,
        SuperWide,
        Full
    }

    public static class AudioUtils
    {

        public static WaveInDevice GetMic()
        {
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            MMDevice mmDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
            var waveInDevices = WaveInDevice.EnumerateDevices().ToArray();
            WaveInDevice device = waveInDevices.FirstOrDefault(w => mmDevice.FriendlyName.StartsWith(w.Name));
            return device;
        }
        public static void ApplyGain(float[] samples, float gain)
        {
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] *= gain;
            }
        }

        public static float GetMaxAmplitude(float[] samples)
        {
            return samples.Max();
        }

        public static int GetFrequency(BandMode mode)
        {
            return mode switch
            {
                BandMode.Narrow => 8000,
                BandMode.Medium => 12000,
                BandMode.Wide => 16000,
                BandMode.SuperWide => 24000,
                BandMode.Full => 48000,
                _ => 16000
            };
        }

        public static void Resample(float[] source, float[] target, int inputSampleRate, int outputSampleRate, int outputChannelsNum = 1)
        {
            Resample(source, target, source.Length, target.Length, inputSampleRate, outputSampleRate, outputChannelsNum);
        }

        public static void Resample(float[] source, float[] target, int inputNum, int outputNum, int inputSampleRate, int outputSampleRate, int outputChannelsNum)
        {
            float ratio = inputSampleRate / (float)outputSampleRate;
            if (ratio % 1f <= float.Epsilon)
            {
                int intRatio = Mathf.RoundToInt(ratio);
                for (int i = 0; i < (outputNum / outputChannelsNum) && (i * intRatio) < inputNum; i++)
                {
                    for (int j = 0; j < outputChannelsNum; j++)
                        target[i * outputChannelsNum + j] = source[i * intRatio];
                }
            }
            else
            {
                if (ratio > 1f)
                {
                    for (int i = 0; i < (outputNum / outputChannelsNum) && Mathf.CeilToInt(i * ratio) < inputNum; i++)
                    {
                        for (int j = 0; j < outputChannelsNum; j++)
                            target[i * outputChannelsNum + j] = Mathf.Lerp(source[Mathf.FloorToInt(i * ratio)], source[Mathf.CeilToInt(i * ratio)], ratio % 1);
                    }
                }
                else
                {
                    for (int i = 0; i < (outputNum / outputChannelsNum) && Mathf.FloorToInt(i * ratio) < inputNum; i++)
                    {
                        for (int j = 0; j < outputChannelsNum; j++)
                        {
                            target[i * outputChannelsNum + j] = source[Mathf.FloorToInt(i * ratio)];
                        }
                    }
                }
            }
        }

        public static int GetFreqForMic(string? deviceName = null)
        {
            int minFreq;
            int maxFreq;

            Microphone.GetDeviceCaps(deviceName, out minFreq, out maxFreq);

            if (minFreq >= 12000)
            {
                if (FindClosestFreq(minFreq, maxFreq) != 0)
                {
                    return FindClosestFreq(minFreq, maxFreq);
                }
                else
                {
                    return minFreq;
                }
            }
            else
            {
                return maxFreq;
            }
        }

        public static int[] possibleSampleRates = new int[] { 8000, 12000, 16000, 24000, 48000 };

        public static int FindClosestFreq(int minFreq, int maxFreq)
        {
            foreach (int sampleRate in possibleSampleRates)
            {
                if (sampleRate >= minFreq && sampleRate <= maxFreq)
                {
                    return sampleRate;
                }
            }
            return 0;
        }

        public static int Convert(short[] data, int dataLength, float[] target)
        {
            for (int i = 0; i < dataLength; i++)
            {
                target[i] = data[i] / (float)short.MaxValue;
            }
            return dataLength;
        }

        public static int Convert(byte[] data, int dataLength, short[] target)
        {

            int lastIndex = 0;
            for (int i = 0; i < dataLength / 2; i++)
            {
                target[i] = (short)((data[i * 2 + 1] << 8) | data[i * 2]);
                lastIndex = i;
            }
            return lastIndex;
        }

        public static int Convert(float[] data, int dataLength, short[] target)
        {
            for (int i = 0; i < dataLength; i++)
            {
                target[i] = (short)(data[i] * short.MaxValue);
            }
            return dataLength;
        }

        public static int Convert(byte[] data, int dataLength, float[] target)
        {
            for (int i = 0; i < dataLength / 2; i++)
            {
                target[i] = ((short)((data[i * 2 + 1] << 8) | data[i * 2])) / (float)short.MaxValue;
            }
            return dataLength / 2;
        }

        public static int Convert(float[] data, int dataLength, byte[] target)
        {
            for (int i = 0; i < dataLength; i++)
            {
                short shortVal = ((short)(data[i] * short.MaxValue));
                byte low = (byte)(shortVal);
                byte high = (byte)((shortVal >> 8) & 0xFF);
                target[i * 2] = low;
                target[i * 2 + 1] = high;
            }
            return dataLength * 2;
        }
    }
}

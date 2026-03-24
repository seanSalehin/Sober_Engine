using System;
using System.IO;
using OpenTK.Audio.OpenAL;

namespace Sober.Audio
{
    public static class WavLoader
    {
        public static AudioClip LoadWav(string path)
        {
            using var fs = File.OpenRead(path);
            using var br = new BinaryReader(fs);

            string riff = new string(br.ReadChars(4));
            int fileSize = br.ReadInt32();
            string wave = new string(br.ReadChars(4));

            if (riff != "RIFF" || wave != "WAVE")
                throw new Exception("Not a valid WAV file");

            short channels = 0;
            int sampleRate = 0;
            short bitsPerSample = 0;
            byte[] data = null;

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                string chunkId = new string(br.ReadChars(4));
                int chunkSize = br.ReadInt32();

                if (chunkId == "fmt ")
                {
                    short audioFormat = br.ReadInt16();
                    if (audioFormat != 1)
                        throw new Exception("Only PCM WAV is supported");

                    channels = br.ReadInt16();
                    sampleRate = br.ReadInt32();
                    br.ReadInt32(); 
                    br.ReadInt16(); 
                    bitsPerSample = br.ReadInt16();

                    if (chunkSize > 16)
                        br.BaseStream.Position += (chunkSize - 16);
                }
                else if (chunkId == "data")
                {
                    data = br.ReadBytes(chunkSize);
                }
                else
                {
                    br.BaseStream.Position += chunkSize;
                }
            }

            if (data == null)
                throw new Exception("WAV file has no data chunk");

            ALFormat format;

            if (channels == 1 && bitsPerSample == 8)
                format = ALFormat.Mono8;
            else if (channels == 1 && bitsPerSample == 16)
                format = ALFormat.Mono16;
            else if (channels == 2 && bitsPerSample == 8)
                format = ALFormat.Stereo8;
            else if (channels == 2 && bitsPerSample == 16)
                format = ALFormat.Stereo16;
            else
                throw new Exception("Unsupported WAV format");

            int buffer = AL.GenBuffer();
            AL.BufferData(buffer, format, ref data[0], data.Length, sampleRate);

            return new AudioClip(buffer);
        }
    }
}
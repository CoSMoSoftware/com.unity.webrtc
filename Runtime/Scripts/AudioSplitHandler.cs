using System.Collections;
using System.Collections.Generic;
namespace Unity.WebRTC
{
    public class AudioSplitHandler
    {
        public Dictionary<int, AudioTrackFilter> audioTrackDictionary = new Dictionary<int, AudioTrackFilter>();
        private int channelCount;
        private int audioBufferSize;

        public AudioSplitHandler(int count, int bufferSize)
        {
                channelCount = count;
                audioBufferSize = count * bufferSize;
        }

        public void AddTrack(int index)
        {
                audioTrackDictionary.Add(index, new AudioTrackFilter(audioBufferSize));
            
        }
        public void SetAudioTrack(int index, float[] data)
        {
                audioTrackDictionary[index].SetAudioTrack(data);
        }
        public bool isBufferEmpty()
        {
            foreach(var key in audioTrackDictionary.Keys)
            {
                if(audioTrackDictionary[key] != null && audioTrackDictionary[key].buffereAvailable)
                    return false;
            }
            return true;

        }
        public float[] GetAudioTrack(int index)
        {
                if(audioTrackDictionary[index] != null)
                    return audioTrackDictionary[index].GetAudioTrack();
                return null;
        }

        private float[] GetChannelData(float[] data, int channelIndex)
        {
            List<float> filteredData = new List<float>();
            for(int i = channelIndex; i < data.Length; i+=channelCount)//0,6,12,...
            {
                filteredData.Add(data[i]);
            }
            return filteredData.ToArray();//1048
        }

        public void SetfilteredData(float[] data)
        {
            foreach (var key in audioTrackDictionary.Keys)
            {
                float[] list = GetChannelData(data, key);
                int index = 0;
                float[] cachebufer = new float[audioBufferSize];
                for(int i = 0; i< list.Length && index < audioBufferSize-1 ; i++)
                {
                    cachebufer[index] = list[i];
                    cachebufer[index+1] = list[i];
                    index +=2;
                }
                SetAudioTrack(key, cachebufer);
            }
        }
    }

    public class AudioTrackFilter
    {
        public bool buffereAvailable;
        float[] data;

        public AudioTrackFilter(int length)
        {
            this.data = new float[length];
        }
        public void SetAudioTrack(float[] data)
        {
            System.Array.Copy(data, this.data, data.Length);
            buffereAvailable = true;
        }
        public float[] GetAudioTrack()
        {
            if(buffereAvailable)
            {
                buffereAvailable = false;
                return data;
            }
            return null;
            
        }
    }
}

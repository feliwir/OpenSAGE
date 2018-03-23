﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenAL;
using OpenSage.Data.Wav;

namespace OpenSage.Audio
{
    public sealed class AudioSystem : GameSystem
    {
        private IntPtr _device;
        private IntPtr _context;

        private List<AudioSource> _sources;
        private Dictionary<string, AudioBuffer> _files;
        private Dictionary<string, AudioStream> _streams;

        internal static void alCheckError()
        {
            int error;
            error = AL10.alGetError();

            if (error != AL10.AL_NO_ERROR)
            {
                throw new InvalidOperationException("AL error!");
            }
        }

        internal void alcCheckError()
        {
            int error;
            error = ALC10.alcGetError(_device);

            if (error != ALC10.ALC_NO_ERROR)
            {
                throw new InvalidOperationException("ALC error!");
            }
        }

        public AudioSystem(Game game) : base(game)
        {
            _device = ALC10.alcOpenDevice("");
            alcCheckError();

            _context = ALC10.alcCreateContext(_device, null);
            alcCheckError();

            ALC10.alcMakeContextCurrent(_context);
            alcCheckError();

            _sources = new List<AudioSource>();
            _files = new Dictionary<string, AudioBuffer>();
            _streams = new Dictionary<string, AudioStream>();
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            base.Dispose(disposeManagedResources);
            _sources.Clear();
            _files.Clear();


            ALC10.alcMakeContextCurrent(IntPtr.Zero);
            ALC10.alcDestroyContext(_context);
            ALC10.alcCloseDevice(_device);
          
        }

        public AudioStream PlayStream(string fileName, bool loop = false)
        {
            AudioStream stream = null;
            if (!_streams.ContainsKey(fileName))
            {
                stream = Game.ContentManager.Load<AudioStream>(fileName);
                _streams[fileName] = stream;
            }
            else
            {
                stream = _streams[fileName];
            }

            return stream;
        }

        public AudioSource PlaySound(string fileName,bool loop=false)
        {
            AudioBuffer buffer = null;
            if (!_files.ContainsKey(fileName))
            {
                var file = Game.ContentManager.Load<WavFile>(fileName);
                buffer = AddDisposable(new AudioBuffer(file));
                _files[fileName] = buffer;
            }
            else
            {
                buffer = _files[fileName];
            }

            var source = AddDisposable(new AudioSource(buffer));

            if(loop)
            {
                source.Looping = true;
            }

            _sources.Add(source);

            return source;
        }
    }
}
